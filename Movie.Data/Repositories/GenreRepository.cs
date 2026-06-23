using Microsoft.EntityFrameworkCore;
using Movie.Core.DomainContracts;
using Movie.Core.Entities;
using Movie.Data.Context;

namespace Movie.Data.Repositories;

public sealed class GenreRepository : IGenreRepository
{
    private readonly MovieApiContext _context;

    public GenreRepository(MovieApiContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        return await _context
            .Set<Genre>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Genre?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context
            .Set<Genre>()
            .FirstOrDefaultAsync(genre => genre.Id == id, cancellationToken);
    }

    public async Task<bool> AnyAsync(Guid id)
    {
        return await _context
            .Set<Genre>()
            .AnyAsync(genre => genre.Id == id);
    }

    public void Add(Genre genre)
    {
        ArgumentNullException.ThrowIfNull(genre);

        _context.Set<Genre>().Add(genre);
    }

    public void Update(Genre genre)
    {
        ArgumentNullException.ThrowIfNull(genre);

        _context.Set<Genre>().Update(genre);
    }

    public void Remove(Genre genre)
    {
        ArgumentNullException.ThrowIfNull(genre);

        _context.Set<Genre>().Remove(genre);
    }
}
