using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Review;
using Movie.Core.Entities;
using Movie.Service.Contracts.Interfaces;
using Movie.Services;
using MovieEntity = Movie.Core.Entities.Movie;
namespace MovieApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    // GET: api/Reviws
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReview(CancellationToken cancellationToken)
    {
        IReadOnlyList<ReviewDto> reviews = await _reviewService.GetReviewsAsync(cancellationToken);

        return Ok(reviews);
    }

    //GET /api/movies/{movieId}/reviews
    [HttpGet("/api/movies/{movieId:guid}/reviews")]
    public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetMovieReviews([FromRoute] Guid movieId, CancellationToken cancellationToken)
    {
        IReadOnlyList<ReviewDto>? reviews = await _reviewService.GetReviewsByMovieIdAsync(movieId, cancellationToken);

        if (reviews is null)
        {
            return NotFound();
        }

        return Ok(reviews);
    }

    //POST /api/movies/{movieId}/reviews
    [HttpPost("/api/movies/{movieId:guid}/reviews")]
    public async Task<ActionResult<ReviewDto>> PostReview([FromRoute] Guid movieId, [FromBody] ReviewCreateDto reviewCreateDto, CancellationToken cancellationToken = default)
    {
        ReviewDto? reviewDto = await _reviewService.CreateReviewAsync(
        movieId,
        reviewCreateDto,
        cancellationToken
    );

        if (reviewDto is null)
        {
            return NotFound();
        }

        return Created($"/api/reviews/{reviewDto.Id}", reviewDto);
    }

    // DELETE: api/Reviews/5
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteMovie([FromRoute] System.Guid id, CancellationToken cancellationToken = default)
    {
        bool isDelted = await _reviewService.DeleteReviewAsync(id, cancellationToken);

        if (!isDelted)
        {     
            return NoContent();
        }

        return NotFound();
    }
}
