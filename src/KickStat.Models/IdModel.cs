using System.ComponentModel.DataAnnotations;

namespace KickStat.Models;

public record IdModel<T>([Required] T Id);

// {
//     [Required]
//     public T Id { get; set; } = default!;
// }