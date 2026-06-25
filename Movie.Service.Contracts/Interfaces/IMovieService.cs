using Movie.Core.DTOs.Movie;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
using Movie.Service.Contracts.Results;
namespace Movie.Service.Contracts.Interfaces;

public interface IMovieService
{
    Task<PagedResult<MovieDto>> GetMoviesAsync(
        MovieQueryParameters queryParameters,
        CancellationToken cancellationToken = default
    );

    Task<MovieDto?> GetMovieByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<MovieDetailDto?> GetMovieDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<MovieDto?> CreateMovieAsync(
        MovieCreateDto movieCreateDto,
        CancellationToken cancellationToken = default
    );

    Task<UpdateMovieResult> UpdateMovieAsync(
        Guid id,
        MovieUpdateDto movieUpdateDto,
        CancellationToken cancellationToken = default
    );

    Task<bool> DeleteMovieAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );
}

