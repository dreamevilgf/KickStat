using System.Security.Claims;
using KickStat.Data;
using KickStat.Data.Domain;
using KickStat.Models;
using KickStat.Models.Games;
using KickStat.Services;
using KickStat.UI.SiteApi.Framework;
using KickStat.UI.SiteApi.Framework.Extensions.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KickStat.UI.SiteApi.Controllers;

[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class GamesController : ManagementApiController
{
    private readonly KickStatDbContext _dbContext;
    private readonly EventDetailService _eventDetailService;

    public GamesController(KickStatDbContext dbContext, EventDetailService eventDetailService)
    {
        _dbContext = dbContext;
        _eventDetailService = eventDetailService;
    }

    [HttpPost]
    public async Task<PagedResult<GameListModel>> List(GameListRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var query = _dbContext.Games.AsNoTracking().Where(x => x.OwnerId == new Guid(userId) && !x.IsDeleted);

        if (!string.IsNullOrEmpty(request.Filter.Query))
        {
            query = query.Where(x => EF.Functions.ILike(x.OpposingTeam, $"%{request.Filter.Query.Trim()}%")
            || EF.Functions.ILike(x.Player.FullName, $"%{request.Filter.Query.Trim()}%"));
        }

        if (request.Sort.OrderBy == GameSortOptions.Id)
            query = request.Sort.IsAscending ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
        else if (request.Sort.OrderBy == GameSortOptions.Date)
            query = request.Sort.IsAscending ? query.OrderBy(x => x.Date) : query.OrderByDescending(x => x.Date);

        var totalCount = await query.CountAsync();

        if (request.Filter.Skip.HasValue)
            query = query.Skip(request.Filter.Skip.Value);

        if (request.Filter.Take.HasValue)
            query = query.Take(request.Filter.Take.Value);

        var result = await query.Select(x => new GameListModel()
        {
            Id = x.Id,
            OpposingTeam = x.OpposingTeam,
            Date = x.Date,
            Player = x.Player!.FullName
        }).ToListAsync();

        return new PagedResult<GameListModel>(result, totalCount, request.Filter.Skip, request.Filter.Take);
    }

    [HttpGet("{id:int}")]
    public async Task<GameEditModel> Get(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var eventDetails = await _eventDetailService.All();
        var model = new GameEditModel();
        if (id == 0)
        {
           model.ToModel(new Game(), eventDetails);
           return model;
        }
        
        var entity = await _dbContext.Games.AsNoTracking()
            .Where(x => x.Id == id && x.OwnerId == new Guid(userId) && !x.IsDeleted)
            .Select(x => new Game()
            {
                Id = x.Id,
                OpposingTeam = x.OpposingTeam,
                PlayerId = x.PlayerId,
                Date = x.Date,
                Meta = x.Meta,
                Events = x.Events.Any()
                    ? x.Events.Select(y => new GameEvent()
                    {
                        Id = y.Id,
                        PositiveValue = y.PositiveValue,
                        NegativeValue = y.NegativeValue,
                        GameId = y.GameId,
                        EventDetailId = y.EventDetailId
                    }).ToList()
                    : new List<GameEvent>()
            })
            .FirstOrDefaultAsync();

        if (entity == null)
            throw new ApiException("Не удалось найти игру");

        model.ToModel(entity, eventDetails);

        return model;
    }
    
    [HttpGet("{id:int}/stats")]
    public async Task<GameModel> Stats(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var eventDetails = await _eventDetailService.All();
        var model = new GameModel();
        if (id == 0)
        {
            model.ToModel(new Game(), eventDetails);
            return model;
        }
        
        var entity = await _dbContext.Games.AsNoTracking()
            .Where(x => x.Id == id && x.OwnerId == new Guid(userId) && !x.IsDeleted)
            .Select(x => new Game()
            {
                Id = x.Id,
                OpposingTeam = x.OpposingTeam,
                PlayerId = x.PlayerId,
                Date = x.Date,
                Meta = x.Meta,
                Player = new Player()
                {
                    Id = x.Player!.Id,
                    FullName = x.Player!.FullName,
                },
                Events = x.Events.Any()
                    ? x.Events.Select(y => new GameEvent()
                    {
                        Id = y.Id,
                        PositiveValue = y.PositiveValue,
                        NegativeValue = y.NegativeValue,
                        GameId = y.GameId,
                        EventDetailId = y.EventDetailId
                    }).ToList()
                    : new List<GameEvent>()
            })
            .FirstOrDefaultAsync();

        if (entity == null)
            throw new ApiException("Не удалось найти игру");

        model.ToModel(entity, eventDetails);

        return model;
    }

    [HttpPost("save")]
    public async Task<GameEditModel> Save(GameEditModel model)
    {
        Game? entity;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (model.IsNew)
            entity = new Game();
        else
        {
            entity = await _dbContext.Games.Include(x => x.Events)
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.OwnerId == new Guid(userId) && !x.IsDeleted);
            if (entity == null)
                throw new ApiException("Игра не найдена");
        }

        model.ToEntity(entity);
        entity.OwnerId = new Guid(userId);
        if (model.IsNew)
        {
            entity.Events.AddRange(model.Events.SelectMany(x => x.Value).Select(x => new GameEvent()
            {
                EventDetailId = x.EventDetailId,
                NegativeValue = x.NegativeValue,
                PositiveValue = x.PositiveValue,
            }).ToList());
            _dbContext.Games.Add(entity);
        }

        else
        {
            entity.Events.Clear();
            entity.Events.AddRange(model.Events.SelectMany(x => x.Value).Select(x => new GameEvent()
            {
                EventDetailId = x.EventDetailId,
                GameId = x.GameId,
                NegativeValue = x.NegativeValue,
                PositiveValue = x.PositiveValue,
            }));
            _dbContext.Games.Update(entity);
            _dbContext.Entry(entity).Property(x => x.Meta).IsModified = true;
        }


        await _dbContext.SaveChangesAsync();

        return await Get(entity.Id);
    }


    [HttpDelete("{id}")]
    public async Task Delete(int id)
    {
        var entity = await _dbContext.Games.AsNoTracking().Where(x => x.Id == id).Select(x => new Game()
        {
            Id = x.Id,
            IsDeleted = x.IsDeleted
        }).FirstOrDefaultAsync();

        if (entity == null)
            throw new ApiException("Игра не найдена");

        _dbContext.Entry(entity).Property(x => x.IsDeleted).IsModified = true;
        await _dbContext.SaveChangesAsync();
    }
}