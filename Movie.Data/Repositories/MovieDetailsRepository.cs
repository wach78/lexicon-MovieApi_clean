using Microsoft.EntityFrameworkCore;
using Movie.Core.DomainContracts;
using Movie.Core.Entities;
using Movie.Data.Context;

namespace Movie.Data.Repositories;

public sealed class MovieDetailsRepository : IMovieDetailsRepository
{
    private readonly MovieApiContext _context;

    public MovieDetailsRepository(MovieApiContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    public async Task<IEnumerable<MovieDetails>> GetAllAsync()
    {
        return await _context
            .Set<MovieDetails>()
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<MovieDetails?> GetAsync(Guid id)
    {
        return await _context
            .Set<MovieDetails>()
            .FirstOrDefaultAsync(movieDetails => movieDetails.Id == id);
    }

    public async Task<bool> AnyAsync(Guid id)
    {
        return await _context
            .Set<MovieDetails>()
            .AnyAsync(movieDetails => movieDetails.Id == id);
    }

    public void Add(MovieDetails movieDetails)
    {
        ArgumentNullException.ThrowIfNull(movieDetails);

        _context.Set<MovieDetails>().Add(movieDetails);
    }

    public void Update(MovieDetails movieDetails)
    {
        ArgumentNullException.ThrowIfNull(movieDetails);

        _context.Set<MovieDetails>().Update(movieDetails);
    }

    public void Remove(MovieDetails movieDetails)
    {
        ArgumentNullException.ThrowIfNull(movieDetails);

        _context.Set<MovieDetails>().Remove(movieDetails);
    }
}
