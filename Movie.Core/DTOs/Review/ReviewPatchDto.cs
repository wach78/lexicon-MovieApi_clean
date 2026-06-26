namespace Movie.Core.DTOs.Review;

public sealed class ReviewPatchDto
{
    public string? ReviewerName { get; set; }

    public string? Comment { get; set; }

    public int? Rating { get; set; }
}
