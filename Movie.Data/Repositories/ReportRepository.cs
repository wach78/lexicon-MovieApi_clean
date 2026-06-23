using Microsoft.EntityFrameworkCore;
using Movie.Core.DomainContracts;
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Movie;
using Movie.Core.DTOs.Report;
using Movie.Data.Context;
using MovieEntity = Movie.Core.Entities.Movie;

namespace Movie.Data.Repositories;

public sealed class ReportRepository : IReportRepository
{
    private readonly MovieApiContext _context;

    public ReportRepository(MovieApiContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    public async Task<IReadOnlyList<MovieAverageRatingDto>>
        GetAverageRatingsByGenreAsync(
            CancellationToken cancellationToken = default)
    {
        return await _context
            .Set<MovieEntity>()
            .AsNoTracking()
            .Where(movie => movie.Reviews.Any())
            .GroupBy(movie => new
            {
                movie.GenreId,
                GenreName = movie.Genre != null
                    ? movie.Genre.Name
                    : "No genre"
            })
            .Select(group => new MovieAverageRatingDto
            {
                GenreId = group.Key.GenreId,
                GenreName = group.Key.GenreName,
                AverageRating = Math.Round(
                    group
                        .SelectMany(movie => movie.Reviews)
                        .Average(review => review.Rating),
                    2
                ),
                ReviewCount = group
                    .SelectMany(movie => movie.Reviews)
                    .Count()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TopMoviesPerGenreDto>>
        GetTopMoviesPerGenreAsync(
            CancellationToken cancellationToken = default)
    {
        var movieRatings = await _context
            .Set<MovieEntity>()
            .AsNoTracking()
            .Where(movie => movie.Reviews.Any())
            .Select(movie => new
            {
                MovieId = movie.Id,
                movie.Title,
                movie.Year,
                movie.GenreId,
                GenreName = movie.Genre != null
                    ? movie.Genre.Name
                    : "No genre",
                AverageRating = movie.Reviews.Average(
                    review => review.Rating
                ),
                ReviewCount = movie.Reviews.Count
            })
            .ToListAsync(cancellationToken);

        return movieRatings
            .GroupBy(movie => new
            {
                movie.GenreId,
                movie.GenreName
            })
            .Select(group => new TopMoviesPerGenreDto
            {
                GenreId = group.Key.GenreId,
                GenreName = group.Key.GenreName,
                Movies = group
                    .OrderByDescending(movie => movie.AverageRating)
                    .ThenByDescending(movie => movie.ReviewCount)
                    .Take(5)
                    .Select(movie => new TopRatedMovieDto
                    {
                        MovieId = movie.MovieId,
                        Title = movie.Title,
                        Year = movie.Year,
                        AverageRating = Math.Round(
                            movie.AverageRating,
                            2
                        ),
                        ReviewCount = movie.ReviewCount
                    })
                    .ToList()
            })
            .OrderBy(result => result.GenreName)
            .ToList();
    }

    public async Task<IReadOnlyList<MostActiveActorDto>>
        GetMostActiveActorsAsync(
            CancellationToken cancellationToken = default)
    {
        return await _context.Actors
            .AsNoTracking()
            .Where(actor => actor.Movies.Any())
            .Select(actor => new MostActiveActorDto
            {
                ActorId = actor.Id,
                Name = actor.Name,
                MovieCount = actor.Movies.Count
            })
            .OrderByDescending(actor => actor.MovieCount)
            .ThenBy(actor => actor.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<MovieWithMostReviewsDto?>
        GetMovieWithMostReviewsAsync(
            CancellationToken cancellationToken = default)
    {
        return await _context
            .Set<MovieEntity>()
            .AsNoTracking()
            .Where(movie => movie.Reviews.Any())
            .Select(movie => new MovieWithMostReviewsDto
            {
                MovieId = movie.Id,
                Title = movie.Title,
                ReviewCount = movie.Reviews.Count
            })
            .OrderByDescending(movie => movie.ReviewCount)
            .ThenBy(movie => movie.Title)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PopularGenreDto>>
        GetPopularGenresAsync(
            CancellationToken cancellationToken = default)
    {
        return await _context
            .Set<MovieEntity>()
            .AsNoTracking()
            .Where(movie => movie.GenreId != null)
            .GroupBy(movie => new
            {
                movie.GenreId,
                GenreName = movie.Genre != null
                    ? movie.Genre.Name
                    : "Unknown"
            })
            .Select(group => new PopularGenreDto
            {
                GenreId = group.Key.GenreId,
                GenreName = group.Key.GenreName,
                MovieCount = group.Count()
            })
            .OrderByDescending(genre => genre.MovieCount)
            .ThenBy(genre => genre.GenreName)
            .ToListAsync(cancellationToken);
    }
}
