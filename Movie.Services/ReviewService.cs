using Movie.Core.DomainContracts;
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Review;
using Movie.Core.Entities;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
using Movie.Service.Contracts.Interfaces;
using MovieEntity = Movie.Core.Entities.Movie;

namespace Movie.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewService(IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);

        _unitOfWork = unitOfWork;
    }

    public async Task<ReviewDto?> CreateReviewAsync(Guid movieId, ReviewCreateDto reviewCreateDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reviewCreateDto);

        MovieEntity? movie = await _unitOfWork.Movies.GetAsync(
            movieId,
            cancellationToken
        );

        if (movie is null)
        {
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
            return false;
        }

        _unitOfWork.Reviews.Remove(review);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return true;
    }

    public async Task<PagedResult<ReviewDto>> GetReviewsAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paginationParameters);
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

        bool movieExists = await _unitOfWork.Movies.AnyAsync(
            movieId,
            cancellationToken
        );

        if (!movieExists)
        {
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

        Review? review = await _unitOfWork.Reviews.GetAsync(
            reviewId,
            cancellationToken
        );

        if (review is null || review.MovieId != movieId)
        {
            return false;
        }

        review.Update(
            reviewPatchDto.ReviewerName ?? review.ReviewerName,
            reviewPatchDto.Comment ?? review.Comment,
            reviewPatchDto.Rating ?? review.Rating
        );

        await _unitOfWork.CompleteAsync(cancellationToken);

        return true;
    }
}

