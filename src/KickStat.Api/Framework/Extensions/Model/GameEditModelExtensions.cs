using KickStat.Data.Domain;
using KickStat.Models.GameEvents;
using KickStat.Models.Games;
using KickStat.Models.Players;

namespace KickStat.UI.SiteApi.Framework.Extensions.Model;

public static class GameEditModelExtension
{
    public static void ToEntity(this GameEditModel model, Game entity)
    {
        entity.Id = model.Id;
        entity.PlayerId = model.PlayerId!.Value;
        entity.Date = model.Date!.Value;
        entity.OpposingTeam = model.OpposingTeam;
        entity.CreatedDate = DateTime.Now;

        entity.Meta.Playtime = model.Playtime;
        entity.Meta.Competition = model.Competition;
        entity.Meta.IsMain = model.IsMain;
        entity.Meta.MatchDuration = model.MatchDuration;
    }

    public static void ToModel(this GameEditModel model, Game entity, List<EventDetail> eventDetails)
    {
        model.Id = entity.Id;
        model.PlayerId = entity.PlayerId  == 0 ? null : entity.PlayerId;
        model.Date = entity.Date != DateTime.MinValue ? entity.Date : DateTime.Now;
        model.Competition = entity.Meta.Competition;
        model.Playtime = entity.Meta.Playtime;
        model.IsMain = entity.Meta.IsMain;
        model.OpposingTeam = entity.OpposingTeam;
        model.MatchDuration =  entity.Meta.MatchDuration == 0 ? 90 : entity.Meta.MatchDuration;
        model.Events = new Dictionary<string, List<GameEventModel>>();

        // Группируем игровые события
        foreach (var gr in eventDetails.OrderBy(x => x.DisplayOrder).GroupBy(x => x.Group))
        {
            if (!model.Events.ContainsKey(gr.Key))
                model.Events.Add(gr.Key, new List<GameEventModel>());

            foreach (var e in gr)
            {
                // Ищем уже существующее событие с таким типов в игре
                if (entity.Events.Any(x => x.EventDetailId == e.Id))
                {
                    var existingEvent = entity.Events.First(x => x.EventDetailId == e.Id);
                    model.Events[gr.Key].Add(new GameEventModel()
                    {
                        Id = existingEvent.Id,
                        EventDetailId = existingEvent.EventDetailId,
                        GameId = entity.Id,
                        NegativeValue = existingEvent.NegativeValue,
                        PositiveValue = existingEvent.PositiveValue,
                        HasNegative = e.HasNegative,
                        LabelPositive = e.Title,
                        LabelNegative = e.HasNegative ? "Ошибка" : null
                    });
                }
                else
                    model.Events[gr.Key].Add(new GameEventModel()
                    {
                        EventDetailId = e.Id,
                        GameId = entity.Id,
                        LabelPositive = e.Title,
                        LabelNegative = e.HasNegative ? "Ошибка" : null,
                        HasNegative = e.HasNegative,
                    });
            }
        }
    }
}