using System;
using System.Collections.Generic;
using System.Text;
using Movie.Core.Entities;
using Movie.Core.Pagination;
using Movie.Core.Parameters;

namespace Movie.Core.DomainContracts;

public interface IReviewRepository
{
    Task<PagedResult<Review>> GetAllAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken = default);
    Task<Review?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Guid id);
    void Add(Review actor);
    void Update(Review actor);
    void Remove(Review actor);
    Task<IReadOnlyList<Review>> GetByMovieIdAsync(Guid movieId, CancellationToken cancellationToken = default);
}
