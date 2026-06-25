using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Review;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
namespace Movie.Service.Contracts.Interfaces;

public interface IReviewService
{
    Task<PagedResult<ReviewDto>> GetReviewsAsync(
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken = default
    );

    Task<PagedResult<ReviewDto>?> GetReviewsByMovieIdAsync(
      Guid movieId,
      PaginationParameters paginationParameters,
      CancellationToken cancellationToken = default);

    Task<bool> MovieExistsAsync(
        Guid movieId,
        CancellationToken cancellationToken = default
    );

    Task<ReviewDto?> CreateReviewAsync(
        Guid movieId,
        Movie.Core.DTOs.Review.ReviewCreateDto reviewCreateDto,
        CancellationToken cancellationToken = default
    );

    Task<bool> DeleteReviewAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );
}
