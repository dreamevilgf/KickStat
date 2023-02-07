﻿using System.Security.Claims;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using KickStat.Data;
using KickStat.Data.Domain;
using KickStat.Models;
using KickStat.Models.GameEvents;
using KickStat.Models.Players;
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
            query = query.Where(x => x.OpposingTeam == request.Filter.Query);

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
            Player = x.Player.FullName
        }).ToListAsync();

        return new PagedResult<GameListModel>(result, totalCount, request.Filter.Skip, request.Filter.Take);
    }

    [HttpGet("{id}")]
    public async Task<GameModel> Get(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var events = await _eventDetailService.All();
        var model = await _dbContext.Games.AsNoTracking()
            .Where(x => x.Id == id && x.OwnerId == new Guid(userId) && !x.IsDeleted)
            .Select(x => new GameModel()
            {
                Id = x.Id,
                OpposingTeam = x.OpposingTeam,
                PlayerId = x.PlayerId,
                Date = x.Date,
                Events = x.Events.Any()
                    ? x.Events.Select(y => new GameEventModel()
                    {
                        Id = y.Id,
                        Value = y.Value,
                        GameId = y.GameId,
                        EventDetail = new EventDetailModel()
                        {
                            Id = y.EventDetailId
                        }
                    }).ToList()
                    : new List<GameEventModel>()
            })
            .FirstOrDefaultAsync();

        if (model.Events.Any())
        {
            foreach (var e in model.Events)
            {
                var eventDetail = events.FirstOrDefault(x => x.Id == e.EventDetail.Id);
                if (eventDetail == null)
                    continue;

                e.EventDetail.Title = eventDetail.Title;
                e.EventDetail.Group = eventDetail.Group;
                e.EventDetail.DisplayOrder = eventDetail.DisplayOrder;
            }
        }
        else
        {
            foreach (var e in events)
                model.Events.Add(new GameEventModel()
                {
                    EventDetail = new EventDetailModel
                        {Id = e.Id, Group = e.Group, Title = e.Title, DisplayOrder = e.DisplayOrder}
                });
        }

        model.Events = model.Events.OrderBy(x => x.EventDetail.DisplayOrder).ToList();


        if (model == null)
            throw new ApiException("Игра не найдена");

        return model;
    }

    [HttpPost("save")]
    public async Task<GameModel> Save(GameModel model)
    {
        Game? entity;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (model.IsNew)
            entity = new Game();
        else
        {
            entity = await _dbContext.Games
                .Include(x => x.Events)
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.OwnerId == new Guid(userId) && !x.IsDeleted);
            if (entity == null)
                throw new ApiException("Игра не найдена");
        }

        model.ToEntity(entity);
        entity.OwnerId = new Guid(userId);
        if (model.IsNew)
            _dbContext.Games.Add(entity);
        else
            _dbContext.Games.Update(entity);

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