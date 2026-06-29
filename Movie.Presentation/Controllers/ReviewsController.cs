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

    /// <summary>
    /// Gets a paginated list of reviews.
    /// </summary>
    /// <param name="paginationParameters">
    /// The pagination parameters used to retrieve reviews.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A paginated collection of reviews.
    /// </returns>
    /// <response code="200">
    /// Returns the paginated list of reviews.
    /// </response>
    /// <response code="400">
    /// The supplied pagination parameters are invalid.
    /// </response>
    /// 
    [HttpGet("reviews")]
    [ProducesResponseType(typeof(PagedResult<ReviewDto>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Gets a paginated list of reviews for a specific movie.
    /// </summary>
    /// <param name="movieId">
    /// The unique identifier of the movie.
    /// </param>
    /// <param name="paginationParameters">
    /// The pagination parameters used to retrieve reviews.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A paginated collection of reviews belonging to the movie.
    /// </returns>
    /// <response code="200">
    /// Returns the paginated list of reviews for the movie.
    /// </response>
    /// <response code="400">
    /// The supplied pagination parameters are invalid.
    /// </response>
    /// <response code="404">
    /// The movie was not found.
    ///</response>

    [HttpGet("movies/{movieId:guid}/reviews")]
    [ProducesResponseType(typeof(PagedResult<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Creates a new review for a specific movie.
    /// </summary>
    /// <param name="movieId">
    /// The unique identifier of the movie.
    /// </param>
    /// <param name="reviewCreateDto">
    /// The information required to create the review.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// The newly created review.
    /// </returns>
    /// <response code="201">
    /// The review was created successfully.
    /// </response>
    /// <response code="400">
    /// The supplied review data is invalid.
    /// </response>
    /// <response code="404">
    /// The movie was not found.
    /// </response>
    
    [HttpPost("movies/{movieId:guid}/reviews")]
    [ProducesResponseType(typeof(ReviewDto),StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        string version = RouteData.Values["version"]?.ToString() ?? "1";

        return CreatedAtAction(
            nameof(GetMovieReviews),
            new
            {
                version,
                movieId
            },
            reviewDto
        );

    }

    /// <summary>
    /// Deletes a review by its unique identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the review to delete.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A result indicating whether the review was deleted.
    /// </returns>
    /// <response code="204">
    /// The review was deleted successfully.
    /// </response>
    /// <response code="404">
    /// The review was not found.
    /// </response>
    
    [HttpDelete("reviews/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReview([FromRoute] System.Guid id, CancellationToken cancellationToken = default)
    {
        bool isDelted = await _serviceManager.Reviews.DeleteReviewAsync(id, cancellationToken);

        if (!isDelted)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Partially updates an existing review.
    /// </summary>
    /// <remarks>
    /// This endpoint is available only in API version 2.
    /// Only supplied properties are updated.
    /// </remarks>
    /// <param name="movieId">
    /// The unique identifier of the movie.
    /// </param>
    /// <param name="reviewId">
    /// The unique identifier of the review to update.
    /// </param>
    /// <param name="reviewPatchDto">
    /// The review properties that should be updated.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A result indicating whether the review was updated.
    /// </returns>
    /// <response code="204">
    /// The review was updated successfully.
    /// </response>
    /// <response code="400">
    /// The supplied review data is invalid.
    /// </response>
    /// <response code="404">
    /// The movie or review was not found.
    /// </response>
    
    [HttpPatch("movies/{movieId:guid}/reviews/{reviewId:guid}")]
    [MapToApiVersion(2.0)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails),StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
