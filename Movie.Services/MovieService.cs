using Movie.Core.DomainContracts;
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Movie;
using Movie.Core.DTOs.Review;
using Movie.Core.Entities;
using Movie.Core.Models.Pagination;
using Movie.Core.Parameters;
using Movie.Service.Contracts.Interfaces;
using Movie.Service.Contracts.Results;
using MovieEntity = Movie.Core.Entities.Movie;

namespace Movie.Services;

public class MovieService : IMovieService
{
    private readonly IUnitOfWork _unitOfWork;

    public MovieService(IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);

        _unitOfWork = unitOfWork;
    }

    public async Task<MovieDto?> CreateMovieAsync(MovieCreateDto movieCreateDto, CancellationToken cancellationToken)
    {
        Genre? genre = null;

        if (movieCreateDto.GenreId.HasValue)
        {
            genre = await _unitOfWork.Genres.GetAsync(
            movieCreateDto.GenreId.Value,
            cancellationToken
        );

            if (genre is null)
            {
                return null;
            }
        }

        MovieEntity movie = new(
            movieCreateDto.Title,
            movieCreateDto.Year,
            movieCreateDto.Duration,
            genre
        );

        _unitOfWork.Movies.Add(movie);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return new MovieDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Year = movie.Year,
            Duration = movie.Duration,
            GenreId = movie.GenreId,
            GenreName = genre?.Name
        };
    }

    public async Task<bool> DeleteMovieAsync(Guid id, CancellationToken cancellationToken)
    {
        MovieEntity? movie = await _unitOfWork.Movies.GetAsync(
            id,
            cancellationToken
        );

        if (movie is null)
        {
            return false;
        }

        _unitOfWork.Movies.Remove(movie);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return true;
    }

    public async Task<MovieDto?> GetMovieByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        MovieEntity? movie = await _unitOfWork.Movies.GetWithGenreAsync(id, cancellationToken
        );

        if (movie is null)
        {
            return null;
        }

        return new MovieDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Year = movie.Year,
            Duration = movie.Duration,
            GenreId = movie.GenreId,
            GenreName = movie.Genre?.Name
        };
    }

    public async Task<MovieDetailDto?> GetMovieDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        MovieEntity? movie = await _unitOfWork.Movies.GetWithDetailsAsync(id, cancellationToken);

        if (movie is null)
        {
            return null;
        }

        return new MovieDetailDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Year = movie.Year,
            Duration = movie.Duration,
            GenreId = movie.GenreId,
            GenreName = movie.Genre?.Name,

            MovieDetails = movie.MovieDetails is null
                ? null
                : new MovieDetailsDto
                {
                    Id = movie.MovieDetails.Id,
                    Synopsis = movie.MovieDetails.Synopsis,
                    Language = movie.MovieDetails.Language,
                    Budget = movie.MovieDetails.Budget
                },

            Reviews = movie.Reviews
                .Select(review => new ReviewDto
                {
                    Id = review.Id,
                    ReviewerName = review.ReviewerName,
                    Comment = review.Comment,
                    Rating = review.Rating
                })
                .ToList(),

            Actors = movie.Actors
                .Select(actor => new ActorDto
                {
                    Id = actor.Id,
                    Name = actor.Name,
                    BirthYear = actor.BirthYear
                })
                .ToList()
        };
    }

    public async Task<PagedResult<MovieDto>> GetMoviesAsync(MovieQueryParameters queryParameters, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(queryParameters);

        PagedResult<MovieEntity> pagedMovies =
            await _unitOfWork.Movies.GetFilteredAsync(
                queryParameters,
                cancellationToken
            );

        IReadOnlyList<MovieDto> movieDtos = pagedMovies.Items
            .Select(movie => new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Year = movie.Year,
                Duration = movie.Duration,
                GenreId = movie.GenreId,
                GenreName = movie.Genre?.Name
            })
            .ToList();

        return new PagedResult<MovieDto>
        {
            Items = movieDtos,
            TotalItems = pagedMovies.TotalItems,
            CurrentPage = pagedMovies.CurrentPage,
            TotalPages = pagedMovies.TotalPages,
            PageSize = pagedMovies.PageSize
        };
    }

    public async Task<UpdateMovieResult> UpdateMovieAsync(Guid id, MovieUpdateDto movieUpdateDto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(movieUpdateDto);

        MovieEntity? movie = await _unitOfWork.Movies.GetAsync(
            id,
            cancellationToken
        );

        if (movie is null)
        {
            return UpdateMovieResult.MovieNotFound;
        }

        Genre? genre = null;

        if (movieUpdateDto.GenreId.HasValue)
        {
            genre = await _unitOfWork.Genres.GetAsync(
                movieUpdateDto.GenreId.Value,
                cancellationToken
            );

            if (genre is null)
            {
                return UpdateMovieResult.GenreNotFound;
            }
        }

        movie.Update(
            movieUpdateDto.Title,
            movieUpdateDto.Year,
            movieUpdateDto.Duration,
            genre
        );

        await _unitOfWork.CompleteAsync(cancellationToken);

        return UpdateMovieResult.Updated;
    }
}
