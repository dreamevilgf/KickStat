using System.ComponentModel.DataAnnotations.Schema;
using KickStat.Data.Domain.Enums;

namespace KickStat.Data.Domain;

[Table("event_details")]
public class EventDetail
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("title")]
    public string Title { get; set; }
    
    [Column("type")]
    public EventDetailType Type { get; set; }

    [Column("display_order")]
    public int DisplayOrder { get; set; }
    
    [Column("group")]
    public string Group { get; set; }
    
    [Column("has_negative")]
    public bool HasNegative { get; set; }
}