using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Review;
using Movie.Core.Entities;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
using Movie.Service.Contracts.Interfaces;
using Asp.Versioning;
namespace Movie.Presentation.Controllers;

[ApiVersion(1.0)]
[ApiVersion(2.0)]
[Route("api/v{version:apiVersion}")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public ReviewsController(IServiceManager serviceManager)
    {
        ArgumentNullException.ThrowIfNull(serviceManager);

        _serviceManager = serviceManager;
    }

    // GET: api/Reviws
    [HttpGet("Reviws")]
    public async Task<ActionResult<PagedResult<ReviewDto>>> GetReview(
        [FromQuery] PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        PagedResult<ReviewDto> reviews =
            await _serviceManager.Reviews.GetReviewsAsync(
                paginationParameters,
                cancellationToken);

        return Ok(reviews);
    }

    //GET /api/movies/{movieId}/reviews
    [HttpGet("movies/{movieId:guid}/reviews")]
    public async Task<ActionResult<PagedResult<ReviewDto>>> GetMovieReviews(
        [FromRoute] Guid movieId,
        [FromQuery] PaginationParameters paginationParameters,
        CancellationToken cancellationToken)
    {
        PagedResult<ReviewDto>? reviews = await _serviceManager.Reviews.GetReviewsByMovieIdAsync(
           movieId,
           paginationParameters,
           cancellationToken
       );

        if (reviews is null)
        {
            return NotFound();
        }

        return Ok(reviews);
    }

    //POST /api/movies/{movieId}/reviews
    [HttpPost("movies/{movieId:guid}/reviews")]
    public async Task<ActionResult<ReviewDto>> PostReview([FromRoute] Guid movieId, [FromBody] ReviewCreateDto reviewCreateDto, CancellationToken cancellationToken = default)
    {
        ReviewDto? reviewDto = await _serviceManager.Reviews.CreateReviewAsync(
        movieId,
        reviewCreateDto,
        cancellationToken
    );

        if (reviewDto is null)
        {
            return NotFound();
        }

        return Created($"/api/v1/reviews/{reviewDto.Id}", reviewDto);
       
    }

    // DELETE: api/Reviews/5
    [HttpDelete("reviews/{id:guid}")]
    public async Task<IActionResult> DeleteReview([FromRoute] System.Guid id, CancellationToken cancellationToken = default)
    {
        bool isDelted = await _serviceManager.Reviews.DeleteReviewAsync(id, cancellationToken);

        if (!isDelted)
        {
            return NotFound();
        }

        return NoContent();
    }

    // PATCH /api/movies/{movieId}/reviews/{reviewId}
    [HttpPatch("movies/{movieId:guid}/reviews/{reviewId:guid}")]
    [MapToApiVersion(2.0)]
    public async Task<IActionResult> PatchReview(
        [FromRoute] Guid movieId,
        [FromRoute] Guid reviewId,
        [FromBody] ReviewPatchDto reviewPatchDto,
        CancellationToken cancellationToken = default)
    {
        bool isUpdated = await _serviceManager.Reviews.PatchReviewAsync(
            movieId,
            reviewId,
            reviewPatchDto,
            cancellationToken
        );

        if (!isUpdated)
        {
            return NotFound();
        }

        return NoContent();
    }
}
