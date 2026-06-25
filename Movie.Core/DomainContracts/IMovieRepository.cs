using System;
using System.Collections.Generic;
using System.Text;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
using MovieEntity = Movie.Core.Entities.Movie;
namespace Movie.Core.DomainContracts;

public interface IMovieRepository
{
    Task<IEnumerable<MovieEntity>> GetAllAsync();
    Task<MovieEntity?> GetAsync(Guid id, CancellationToken cancellationToke = default);
    Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(MovieEntity movie);
    void Update(MovieEntity movie);
    void Remove(MovieEntity movie);
    Task<MovieEntity?> GetWithGenreAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MovieEntity?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<MovieEntity>> GetFilteredAsync(MovieQueryParameters queryParameters,CancellationToken cancellationToken = default);
    Task<MovieEntity?> GetWithActorsAsync(Guid id, CancellationToken cancellationToken = default);
}
