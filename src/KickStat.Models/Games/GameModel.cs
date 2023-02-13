using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KickStat.Models.GameEvents;
using KickStat.Models.Players;

namespace KickStat.Models.Games;

public class GameModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Необходимо указать команду соперника")]
    public string OpposingTeam { get; set; } = null!;

    [Required(ErrorMessage = "Необходимо указать игрока")]
    public int? PlayerId { get; set; }

    [Required(ErrorMessage = "Необходимо указать дату события")]
    public DateTime? Date { get; set; }

    [Required(ErrorMessage = "Необходимо указать длительность матча")]
    [Range(1, 180, ErrorMessage = "Длительность должна быть от 1 до 180 минут")]
    public int MatchDuration { get; set; }

    public string? Competition { get; set; }

    public bool IsMain { get; set; }

    public int? Playtime { get; set; }

    public List<GameEventModel> Events { get; set; } = new();

    public PlayerModel? Player { get; set; }


    [JsonIgnore]
    public bool IsNew => Id == 0;


}