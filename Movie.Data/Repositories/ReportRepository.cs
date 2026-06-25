using Microsoft.EntityFrameworkCore;
using Movie.Core.DomainContracts;
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Movie;
using Movie.Core.DTOs.Report;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
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

    public async Task<PagedResult<MovieAverageRatingDto>>
        GetAverageRatingsByGenreAsync(
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paginationParameters);

        IQueryable<MovieAverageRatingDto> query = _context
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
            });

        int totalItems = await query.CountAsync(cancellationToken);

        IReadOnlyList<MovieAverageRatingDto> items = await query
            .OrderBy(result => result.GenreName)
            .ThenBy(result => result.GenreId)
            .Skip(
                (paginationParameters.Page - 1) *
                paginationParameters.PageSize
            )
            .Take(paginationParameters.PageSize)
            .ToListAsync(cancellationToken);

        int totalPages = (int)Math.Ceiling(
            totalItems / (double)paginationParameters.PageSize
        );

        return new PagedResult<MovieAverageRatingDto>
        {
            Items = items,
            TotalItems = totalItems,
            CurrentPage = paginationParameters.Page,
            TotalPages = totalPages,
            PageSize = paginationParameters.PageSize
        };
    }

    public async Task<PagedResult<TopMoviesPerGenreDto>>
        GetTopMoviesPerGenreAsync(
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paginationParameters);

        var genreQuery = _context
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
            .Select(group => new
            {
                group.Key.GenreId,
                group.Key.GenreName
            });

        int totalItems = await genreQuery.CountAsync(cancellationToken);

        var pagedGenres = await genreQuery
            .OrderBy(genre => genre.GenreName)
            .ThenBy(genre => genre.GenreId)
            .Skip(
                (paginationParameters.Page - 1) *
                paginationParameters.PageSize
            )
            .Take(paginationParameters.PageSize)
            .ToListAsync(cancellationToken);

        List<Guid?> selectedGenreIds = pagedGenres
            .Select(genre => genre.GenreId)
            .ToList();

        var movieRatings = await _context
            .Set<MovieEntity>()
            .AsNoTracking()
            .Where(movie =>
                movie.Reviews.Any() &&
                selectedGenreIds.Contains(movie.GenreId)
            )
            .Select(movie => new
            {
                MovieId = movie.Id,
                movie.Title,
                movie.Year,
                movie.GenreId,
                AverageRating = movie.Reviews.Average(
                    review => review.Rating
                ),
                ReviewCount = movie.Reviews.Count
            })
            .ToListAsync(cancellationToken);

        IReadOnlyList<TopMoviesPerGenreDto> items = pagedGenres
            .Select(genre => new TopMoviesPerGenreDto
            {
                GenreId = genre.GenreId,
                GenreName = genre.GenreName,
                Movies = movieRatings
                    .Where(movie =>
                        movie.GenreId == genre.GenreId
                    )
                    .OrderByDescending(movie =>
                        movie.AverageRating
                    )
                    .ThenByDescending(movie =>
                        movie.ReviewCount
                    )
                    .ThenBy(movie => movie.Title)
                    .ThenBy(movie => movie.MovieId)
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
            .ToList();

        int totalPages = (int)Math.Ceiling(
            totalItems / (double)paginationParameters.PageSize
        );

        return new PagedResult<TopMoviesPerGenreDto>
        {
            Items = items,
            TotalItems = totalItems,
            CurrentPage = paginationParameters.Page,
            TotalPages = totalPages,
            PageSize = paginationParameters.PageSize
        };
    }

    public async Task<PagedResult<MostActiveActorDto>>
        GetMostActiveActorsAsync(
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paginationParameters);

        IQueryable<MostActiveActorDto> query = _context.Actors
            .AsNoTracking()
            .Where(actor => actor.Movies.Any())
            .Select(actor => new MostActiveActorDto
            {
                ActorId = actor.Id,
                Name = actor.Name,
                MovieCount = actor.Movies.Count
            });

        int totalItems = await query.CountAsync(cancellationToken);

        IReadOnlyList<MostActiveActorDto> items = await query
            .OrderByDescending(actor => actor.MovieCount)
            .ThenBy(actor => actor.Name)
            .ThenBy(actor => actor.ActorId)
            .Skip(
                (paginationParameters.Page - 1) *
                paginationParameters.PageSize
            )
            .Take(paginationParameters.PageSize)
            .ToListAsync(cancellationToken);

        int totalPages = (int)Math.Ceiling(
            totalItems / (double)paginationParameters.PageSize
        );

        return new PagedResult<MostActiveActorDto>
        {
            Items = items,
            TotalItems = totalItems,
            CurrentPage = paginationParameters.Page,
            TotalPages = totalPages,
            PageSize = paginationParameters.PageSize
        };
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
            .ThenBy(movie => movie.MovieId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<PopularGenreDto>>
        GetPopularGenresAsync(
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paginationParameters);

        IQueryable<PopularGenreDto> query = _context
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
            });

        int totalItems = await query.CountAsync(cancellationToken);

        IReadOnlyList<PopularGenreDto> items = await query
            .OrderByDescending(genre => genre.MovieCount)
            .ThenBy(genre => genre.GenreName)
            .ThenBy(genre => genre.GenreId)
            .Skip(
                (paginationParameters.Page - 1) *
                paginationParameters.PageSize
            )
            .Take(paginationParameters.PageSize)
            .ToListAsync(cancellationToken);

        int totalPages = (int)Math.Ceiling(
            totalItems / (double)paginationParameters.PageSize
        );

        return new PagedResult<PopularGenreDto>
        {
            Items = items,
            TotalItems = totalItems,
            CurrentPage = paginationParameters.Page,
            TotalPages = totalPages,
            PageSize = paginationParameters.PageSize
        };
    }
}
