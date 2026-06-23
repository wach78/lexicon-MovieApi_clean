using Microsoft.EntityFrameworkCore;
using Movie.Core.DomainContracts;
using Movie.Core.Entities;
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

    public async Task<IEnumerable<Review>> GetAllAsync()
    {
        return await _context
            .Set<Review>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Review?> GetAsync(Guid id)
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
}
