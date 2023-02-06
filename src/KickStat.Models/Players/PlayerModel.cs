using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KickStat.Models.Players;

public class PlayerModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Необходимо указать ФИО")]
    public string FullName { get; set; } = null!;

    public string? Description { get; set; }

    [Range(1900, 2100, ErrorMessage = "Необходимо указать корректный год")]
    public int? BirthYear { get; set; }


    [JsonIgnore]
    public bool IsNew => Id == 0;


}