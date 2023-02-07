using KickStat.Data.Domain;
using KickStat.Models.Players;

namespace KickStat.UI.SiteApi.Framework.Extensions.Model;

public static class GameModelExtension
{
    public static void ToEntity(this GameModel model, Game entity)
    {
        entity.Id = model.Id;
        entity.PlayerId = model.PlayerId;
        entity.Date = model.Date;
        entity.OpposingTeam = model.OpposingTeam;

        foreach (var e in model.Events)
        {
            if (entity.Events.Any(x => x.Id == e.Id))
            {
                var existEvent = entity.Events.First(x => x.Id == e.Id);
                existEvent.Value = e.Value;
            }
            else
                entity.Events.Add(new GameEvent() {EventDetailId = e.EventDetail.Id, Value = e.Value});
        }
    }
}