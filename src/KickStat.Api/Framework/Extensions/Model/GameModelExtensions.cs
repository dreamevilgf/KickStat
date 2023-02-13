using KickStat.Data.Domain;
using KickStat.Models.GameEvents;
using KickStat.Models.Games;
using System.Linq.Expressions;

namespace KickStat.UI.SiteApi.Framework.Extensions.Model;

public static class GameModelExtension
{
    public static void ToEntity(this GameModel model, Game entity)
    {
        entity.Id = model.Id;
        entity.PlayerId = model.PlayerId!.Value;
        entity.Date = model.Date!.Value;
        entity.OpposingTeam = model.OpposingTeam;

        entity.Meta.Playtime = model.Playtime;
        entity.Meta.Competition = model.Competition;
        entity.Meta.IsMain = model.IsMain;
        entity.Meta.MatchDuration = model.MatchDuration;


        foreach (var e in model.Events)
        {
            if (entity.Events.Any(x => x.Id == e.Id))
            {
                var existEvent = entity.Events.First(x => x.Id == e.Id);
                existEvent.Value = e.Value;
            }
            else
                entity.Events.Add(new GameEvent() { EventDetailId = e.EventDetail.Id, Value = e.Value });
        }
    }

    public static void ToModel(this GameModel model, Game entity, List<EventDetail> events)
    {
        model.Id = entity.Id;
        model.PlayerId = entity.PlayerId;
        model.Date = entity.Date;
        model.Competition = entity.Meta.Competition;
        model.Playtime = entity.Meta.Playtime;
        model.IsMain = entity.Meta.IsMain;
        model.OpposingTeam = entity.OpposingTeam;
        model.MatchDuration = entity.Meta.MatchDuration;
        model.Player = new Models.Players.PlayerModel()
        {
            Id = entity.Player!.Id,
            FullName= entity.Player!.FullName,
        };

        model.Events = new List<GameEventModel>();
        if (entity.Events.Any())
        {
            foreach (var e in entity.Events)
            {
                var eventDetail = events.FirstOrDefault(x => x.Id == e.EventDetailId);
                if (eventDetail == null)
                    continue;

                model.Events.Add(new GameEventModel()
                {
                    Id = e.Id,
                    GameId = entity.Id,
                    Value = e.Value,
                    EventDetail = new EventDetailModel
                    { Id = eventDetail.Id, Group = eventDetail.Group, Title = eventDetail.Title, DisplayOrder = eventDetail.DisplayOrder }
                });
            }
        }
        else
        {
            foreach (var e in events)
                model.Events.Add(new GameEventModel()
                {
                    GameId = entity.Id,
                    EventDetail = new EventDetailModel
                    { Id = e.Id, Group = e.Group, Title = e.Title, DisplayOrder = e.DisplayOrder }
                });
        }

        model.Events = model.Events.OrderBy(x => x.EventDetail.DisplayOrder).ToList();

    }

}