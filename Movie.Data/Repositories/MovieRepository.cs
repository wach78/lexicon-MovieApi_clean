using Microsoft.EntityFrameworkCore;
using Movie.Core.DomainContracts;
using Movie.Data.Context;
using MovieEntity = Movie.Core.Entities.Movie;

namespace Movie.Data.Repositories;

public sealed class MovieRepository : IMovieRepository
{
    private readonly MovieApiContext _context;

    public MovieRepository(MovieApiContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    public async Task<IEnumerable<MovieEntity>> GetAllAsync()
    {
        return await _context
            .Set<MovieEntity>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<MovieEntity?> GetAsync(Guid id)
    {
        return await _context
            .Set<MovieEntity>()
            .FirstOrDefaultAsync(movie => movie.Id == id);
    }

    public async Task<bool> AnyAsync(Guid id)
    {
        return await _context
            .Set<MovieEntity>()
            .AnyAsync(movie => movie.Id == id);
    }

    public void Add(MovieEntity movie)
    {
        ArgumentNullException.ThrowIfNull(movie);

        _context.Set<MovieEntity>().Add(movie);
    }

    public void Update(MovieEntity movie)
    {
        ArgumentNullException.ThrowIfNull(movie);

        _context.Set<MovieEntity>().Update(movie);
    }

    public void Remove(MovieEntity movie)
    {
        ArgumentNullException.ThrowIfNull(movie);

        _context.Set<MovieEntity>().Remove(movie);
    }
}
