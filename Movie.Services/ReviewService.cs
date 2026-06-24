using Movie.Core.DomainContracts;
using Movie.Core.DTOs.Review;
using Movie.Core.Entities;
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

    public async Task<IReadOnlyList<ReviewDto>> GetReviewsAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Review> reviews = await _unitOfWork.Reviews.GetAllAsync(cancellationToken);

        return reviews
            .Select(review => new ReviewDto
            {
                Id = review.Id,
                ReviewerName = review.ReviewerName,
                Comment = review.Comment,
                Rating = review.Rating
            })
            .ToList();
    }

    public async Task<IReadOnlyList<ReviewDto>?> GetReviewsByMovieIdAsync(Guid movieId, CancellationToken cancellationToken = default)
    {
        bool movieExists = await _unitOfWork.Movies.AnyAsync(
            movieId,
            cancellationToken
        );

        if (!movieExists)
        {
            return null;
        }

        IReadOnlyList<Review> reviews =
            await _unitOfWork.Reviews.GetByMovieIdAsync(
                movieId,
                cancellationToken
            );

        return reviews
            .Select(review => new ReviewDto
            {
                Id = review.Id,
                ReviewerName = review.ReviewerName,
                Comment = review.Comment,
                Rating = review.Rating
            })
            .ToList();
    }

    public async Task<bool> MovieExistsAsync(Guid movieId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Movies.AnyAsync(movieId,cancellationToken);
    }
}

