using System.Security.Claims;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using KickStat.Data;
using KickStat.Data.Domain;
using KickStat.Data.Domain.Enums;
using KickStat.Models;
using KickStat.Models.GameEvents;
using KickStat.Models.Games;
using KickStat.Models.Players;
using KickStat.Services;
using KickStat.UI.SiteApi.Framework;
using KickStat.UI.SiteApi.Framework.Extensions.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KickStat.UI.SiteApi.Controllers;

[Route("api/event-details")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class EventDetailsController : ManagementApiController
{
    private readonly KickStatDbContext _dbContext;
    private readonly EventDetailService _eventDetailService;

    public EventDetailsController(KickStatDbContext dbContext, EventDetailService eventDetailService)
    {
        _dbContext = dbContext;
        _eventDetailService = eventDetailService;
    }

    [HttpGet]
    public async Task<List<EventDetailModel>> List(EventDetailType type)
    {

        var eventDetails = await _eventDetailService.All();

        var model = eventDetails.Where(x => x.Type == type).Select(x => new EventDetailModel
        {
            Id = x.Id,
            Title = x.Title,
            Group = x.Group,
            DisplayOrder = x.DisplayOrder
        }).OrderBy(x => x.DisplayOrder).ToList();

        return model;
    }

}