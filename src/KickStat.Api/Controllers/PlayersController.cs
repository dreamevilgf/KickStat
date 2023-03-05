using System.Security.Claims;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using KickStat.Data;
using KickStat.Data.Domain;
using KickStat.Models;
using KickStat.Models.Players;
using KickStat.UI.SiteApi.Framework;
using KickStat.UI.SiteApi.Framework.Extensions.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KickStat.UI.SiteApi.Controllers;

[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PlayersController : ManagementApiController
{
    private readonly KickStatDbContext _dbContext;


    // GET: api/<PlayersController>

    public PlayersController(KickStatDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost()]
    public async Task<PagedResult<PlayerModel>> List(PlayerListRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var query = _dbContext.Players.AsNoTracking().Where(x => x.OwnerId == new Guid(userId) && !x.IsDeleted);

        if (!string.IsNullOrEmpty(request.Filter.Query?.Trim()))
            query = query.Where(x => EF.Functions.ILike(x.FullName, $"%{request.Filter.Query.Trim()}%"));

        if (request.Sort.OrderBy == PlayerSortOptions.Id)
            query = request.Sort.IsAscending ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
        else if (request.Sort.OrderBy == PlayerSortOptions.FullName)
            query = request.Sort.IsAscending ? query.OrderBy(x => x.FullName) : query.OrderByDescending(x => x.FullName);

        var totalCount = await query.CountAsync();

        if (request.Filter.Skip.HasValue)
            query = query.Skip(request.Filter.Skip.Value);

        if (request.Filter.Take.HasValue)
            query = query.Take(request.Filter.Take.Value);

        var result = await query.Select(x => new PlayerModel()
        {
            Id = x.Id,
            FullName = x.FullName,
            BirthYear = x.BirthYear
        }).ToListAsync();

        return new PagedResult<PlayerModel>(result, totalCount, request.Filter.Skip, request.Filter.Take);
    }

    // GET api/<PlayersController>/5
    [HttpGet("{id}")]
    public async Task<PlayerModel> Get(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var model = await _dbContext.Players.AsNoTracking()
            .Where(x => x.Id == id && x.OwnerId == new Guid(userId) && !x.IsDeleted)
            .Select(x => new PlayerModel()
            {
                Id = x.Id,
                FullName = x.FullName,
                BirthYear = x.BirthYear,
                Description = x.Description
            })
            .FirstOrDefaultAsync();

        if (model == null)
            throw new ApiException("Игрок не найден");

        return model;
    }

    [HttpPost("save")]
    public async Task<PlayerModel> Save(PlayerModel model)
    {
        Player? entity;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (model.IsNew)
            entity = new Player();
        else
        {
            entity = await _dbContext.Players.FirstOrDefaultAsync(x => x.Id == model.Id && x.OwnerId == new Guid(userId) && !x.IsDeleted);
            if (entity == null)
                throw new ApiException("Игрок не найден");
        }

        model.ToEntity(entity);
        entity.OwnerId = new Guid(userId);
        if (model.IsNew)
            _dbContext.Players.Add(entity);
        else
            _dbContext.Players.Update(entity);

        await _dbContext.SaveChangesAsync();

        return await Get(entity.Id);

    }


    // DELETE api/<PlayersController>/5
    [HttpDelete("{id}")]
    public async Task Delete(int id)
    {
        var entity = await _dbContext.Players.AsNoTracking().Where(x => x.Id == id).Select(x => new Player()
        {
            Id = x.Id,
            IsDeleted = x.IsDeleted
        }).FirstOrDefaultAsync();

        if (entity == null)
            throw new ApiException("Игрок не найден");

        _dbContext.Entry(entity).Property(x => x.IsDeleted).IsModified = true;
        await _dbContext.SaveChangesAsync();
    }
}