using System.ComponentModel.DataAnnotations;

namespace Movie.Core.DTO;

public class MovieActorCreateDto
{
    [Required]
    [StringLength(100)]
    public string Role { get; set; } = string.Empty;
}
