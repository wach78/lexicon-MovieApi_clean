using System.ComponentModel.DataAnnotations;
using Movie.Core.Constants;

namespace Movie.Core.DTOs.Review;

public sealed class ReviewPatchDto
{
    [StringLength(ReviewValidationConstants.ReviewerNameMaxLength)]
    public string? ReviewerName { get; set; }

    [StringLength(ReviewValidationConstants.CommentMaxLength)]
    public string? Comment { get; set; }

    [Range(
        ReviewValidationConstants.MinimumRating,
        ReviewValidationConstants.MaximumRating
    )]
    public int? Rating { get; set; }
}
