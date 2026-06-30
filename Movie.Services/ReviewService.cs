using Movie.Core.DomainContracts;
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Review;
using Movie.Core.Entities;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
using Movie.Service.Contracts.Interfaces;
using MovieEntity = Movie.Core.Entities.Movie;
using Microsoft.Extensions.Logging;

namespace Movie.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(IUnitOfWork unitOfWork, ILogger<ReviewService> logger)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);

        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ReviewDto?> CreateReviewAsync(Guid movieId, ReviewCreateDto reviewCreateDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reviewCreateDto);

        MovieEntity? movie = await _unitOfWork.Movies.GetAsync(
            movieId,
            cancellationToken
        );

        _logger.LogDebug("Creating a review for movie {MovieId}",
            movieId);

        if (movie is null)
        {
            _logger.LogInformation("Review could not be created because movie {MovieId} was not found",
                movieId);
            return null;
        }

        Review review = new(
            reviewCreateDto.ReviewerName,
            reviewCreateDto.Comment,
            reviewCreateDto.Rating
        );

        movie.Reviews.Add(review);
        _unitOfWork.Reviews.Add(review);

        await _unitOfWork.CompleteAsync(cancellationToken);

        _logger.LogInformation("Review {ReviewId} was created successfully for movie {MovieId}",
            review.Id,
            movieId);

        return new ReviewDto
        {
            Id = review.Id,
            ReviewerName = review.ReviewerName,
            Comment = review.Comment,
            Rating = review.Rating
        };
    }

    public async Task<bool> DeleteReviewAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Review? review = await _unitOfWork.Reviews.GetAsync(id, cancellationToken);

        if (review is null)
        {
            _logger.LogInformation("Review with ID {ReviewId} was not found",
                id);

            return false;
        }

        _unitOfWork.Reviews.Remove(review);

        await _unitOfWork.CompleteAsync(cancellationToken);

        _logger.LogInformation("Review with ID {ReviewId} was deleted successfully",
            id);

        return true;
    }

    public async Task<PagedResult<ReviewDto>> GetReviewsAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paginationParameters);

        _logger.LogDebug("Retrieving reviews for page {PageNumber} with page size {PageSize}",
            paginationParameters.Page,
            paginationParameters.PageSize);

        PagedResult<Review> reviews = await _unitOfWork.Reviews.GetAllAsync(paginationParameters, cancellationToken);

        IReadOnlyList<ReviewDto> reviewDto = reviews.Items
            .Select(review => new ReviewDto
            {
                Id = review.Id,
                ReviewerName = review.ReviewerName,
                Comment = review.Comment,
                Rating = review.Rating
            })
            .ToList();

        _logger.LogDebug("Retrieved {ReturnedReviewCount} reviews from a total of {TotalReviewCount} on page {CurrentPage}",
            reviewDto.Count,
            reviews.TotalItems,
            reviews.CurrentPage);

        return new PagedResult<ReviewDto>
        {
            Items = reviewDto,
            TotalItems = reviews.TotalItems,
            CurrentPage = reviews.CurrentPage,
            TotalPages = reviews.TotalPages,
            PageSize = reviews.PageSize
        };
    }

    public async Task<PagedResult<ReviewDto>?> GetReviewsByMovieIdAsync(
        Guid movieId,
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paginationParameters);

        _logger.LogDebug("Retrieving reviews for movie {MovieId}, page {PageNumber}, page size {PageSize}",
            movieId,
            paginationParameters.Page,
            paginationParameters.PageSize);

        bool movieExists = await _unitOfWork.Movies.AnyAsync(
            movieId,
            cancellationToken
        );

        if (!movieExists)
        {
            _logger.LogInformation("Reviews could not be retrieved because movie {MovieId} was not found",
                movieId);

            return null;
        }

        PagedResult<Review> pagedReviews =
            await _unitOfWork.Reviews.GetByMovieIdAsync(
                movieId,
                paginationParameters,
                cancellationToken
            );

        IReadOnlyList<ReviewDto> reviewDtos = pagedReviews.Items
            .Select(review => new ReviewDto
            {
                Id = review.Id,
                ReviewerName = review.ReviewerName,
                Comment = review.Comment,
                Rating = review.Rating
            })
            .ToList();

        _logger.LogDebug("Retrieved {ReturnedReviewCount} reviews for movie {MovieId} from a total of {TotalReviewCount}",
            reviewDtos.Count,
            movieId,
            pagedReviews.TotalItems);

        return new PagedResult<ReviewDto>
        {
            Items = reviewDtos,
            TotalItems = pagedReviews.TotalItems,
            CurrentPage = pagedReviews.CurrentPage,
            TotalPages = pagedReviews.TotalPages,
            PageSize = pagedReviews.PageSize
        };
    }

    public async Task<bool> MovieExistsAsync(Guid movieId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Movies.AnyAsync(movieId,cancellationToken);
    }

    public async Task<bool> PatchReviewAsync(
        Guid movieId,
        Guid reviewId,
        ReviewPatchDto reviewPatchDto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reviewPatchDto);

        _logger.LogDebug("Updating review {ReviewId} for movie {MovieId}",
            reviewId,
            movieId);

        Review? review = await _unitOfWork.Reviews.GetAsync(
            reviewId,
            cancellationToken
        );

        if (review is null || review.MovieId != movieId)
        {
            _logger.LogInformation("Review {ReviewId} could not be updated because it was not found",
                reviewId);

            return false;
        }

        review.Update(
            reviewPatchDto.ReviewerName ?? review.ReviewerName,
            reviewPatchDto.Comment ?? review.Comment,
            reviewPatchDto.Rating ?? review.Rating
        );

        await _unitOfWork.CompleteAsync(cancellationToken);

        _logger.LogInformation("Review {ReviewId} for movie {MovieId} was updated successfully",
           reviewId,
           movieId);

        return true;
    }
}

