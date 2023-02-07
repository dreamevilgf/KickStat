using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KickStat.Models.GameEvents;

namespace KickStat.Models.Players;

public class GameModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Необходимо команду соперника")]
    public string OpposingTeam { get; set; } = null!;

    [Required(ErrorMessage = "Необходимо игрока")]
    public int PlayerId { get; set; }

    [Required(ErrorMessage = "Необходимо указать дату события")]
    public DateTime Date { get; set; }
    
    public List<GameEventModel> Events { get; set; }


    [JsonIgnore]
    public bool IsNew => Id == 0;


}