using Microsoft.EntityFrameworkCore;
using Movie.Core.DomainContracts;
using Movie.Core.Entities;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
using Movie.Data.Context;

namespace Movie.Data.Repositories;

public sealed class ReviewRepository : IReviewRepository
{
    private readonly MovieApiContext _context;

    public ReviewRepository(MovieApiContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }
    public async Task<PagedResult<Review>> GetAllAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paginationParameters);

        IQueryable<Review> query = _context
            .Set<Review>()
            .AsNoTracking();

        int totalItems = await query.CountAsync(cancellationToken);

        IReadOnlyList<Review> items = await query
            .OrderBy(review => review.Id)
            .Skip((paginationParameters.Page - 1) * paginationParameters.PageSize)
            .Take(paginationParameters.PageSize)
            .ToListAsync(cancellationToken);

        int totalPages = (int)Math.Ceiling(
            totalItems / (double)paginationParameters.PageSize
        );

        return new PagedResult<Review>
        {
            Items = items,
            TotalItems = totalItems,
            CurrentPage = paginationParameters.Page,
            TotalPages = totalPages,
            PageSize = paginationParameters.PageSize
        };
    }

    public async Task<Review?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context
            .Set<Review>()
            .FirstOrDefaultAsync(review => review.Id == id);
    }

    public async Task<bool> AnyAsync(Guid id)
    {
        return await _context
            .Set<Review>()
            .AnyAsync(review => review.Id == id);
    }

    public void Add(Review review)
    {
        ArgumentNullException.ThrowIfNull(review);

        _context.Set<Review>().Add(review);
    }

    public void Update(Review review)
    {
        ArgumentNullException.ThrowIfNull(review);

        _context.Set<Review>().Update(review);
    }

    public void Remove(Review review)
    {
        ArgumentNullException.ThrowIfNull(review);

        _context.Set<Review>().Remove(review);
    }

    public async Task<IReadOnlyList<Review>> GetByMovieIdAsync(Guid movieId, CancellationToken cancellationToken = default)
    {
        return await _context
            .Set<Review>()
            .AsNoTracking()
            .Where(review => review.MovieId == movieId)
            .ToListAsync(cancellationToken);
    }
}
