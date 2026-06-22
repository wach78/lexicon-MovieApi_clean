using System.ComponentModel.DataAnnotations;
using Movie.Core.Constants;

namespace Movie.Core.DTOs.Review;

public class ReviewCreateDto
{
    [Required]
    [StringLength(ReviewValidationConstants.ReviewerNameMaxLength)]
    public string ReviewerName { get; set; } = string.Empty;

    [Required]
    [StringLength(ReviewValidationConstants.CommentMaxLength)]
    public string Comment { get; set; } = string.Empty;

    [Range(ReviewValidationConstants.MinimumRating, ReviewValidationConstants.MaximumRating)]
    public int Rating { get; set; }
}
