using System.ComponentModel.DataAnnotations;
using Movie.Core.Constants;

namespace Movie.Core.DTOs.Actor;

public sealed class ActorCreateDto
{
    [Required]
    [StringLength(
        ActorValidationConstants.NameMaxLength,
        MinimumLength = ActorValidationConstants.MinimumNameLength
    )]
    public string Name { get; init; } = string.Empty;

    [Required]
    [Range(
        ActorValidationConstants.MinimumBirthYear,
        ActorValidationConstants.MaximumBirthYear
    )]
    public int BirthYear { get; init; }
}
