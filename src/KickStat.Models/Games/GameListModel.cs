using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KickStat.Models.Players;

public class GameListModel
{
    public int Id { get; set; }

    public string OpposingTeam { get; set; } = null!;

    public string Player { get; set; }

    public DateTime Date { get; set; }

}