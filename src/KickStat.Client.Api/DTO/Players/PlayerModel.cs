using System.ComponentModel.DataAnnotations;
using KickStat.Data.Domain;

namespace KickStat.Client.Api.DTO.Players;

public class PlayerModel
{
    public int Id { get; set; }

    [Required]
    public string FullName { get; set; } = null!;

    public string? Description { get; set; }

    public int? BirthYear { get; set; }

    public PlayerModel()
    {
    }

    public PlayerModel(Player entity)
    {
        Id = entity.Id;
        FullName = entity.FullName;
        Description = entity.Description;
        BirthYear = entity.BirthYear;
    }


    public void ToEntity(Player entity)
    {
        entity.Id = this.Id;
        entity.FullName = this.FullName;
        entity.Description = this.Description;
        entity.BirthYear = this.BirthYear;
    }

    public static Func<Player, PlayerModel> ToModelForList => entity =>
        new PlayerModel()
        {
            Id = entity.Id,
            FullName = entity.FullName, 
        };

    public bool IsNew => Id > 0;


}