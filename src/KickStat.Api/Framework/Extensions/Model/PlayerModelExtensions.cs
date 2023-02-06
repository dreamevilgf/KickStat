using KickStat.Data.Domain;
using KickStat.Models.Players;

namespace KickStat.UI.SiteApi.Framework.Extensions.Model;

public static class PlayerModelExtension
{
    public static void ToEntity(this PlayerModel model, Player entity)
    {
        entity.Id = model.Id;
        entity.BirthYear = model.BirthYear;
        entity.FullName = model.FullName;
        entity.Description = model.Description;
    }
}