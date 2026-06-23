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

    public async Task<MovieEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context
            .Set<MovieEntity>()
            .FirstOrDefaultAsync(movie => movie.Id == id);
    }

    public async Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context
            .Set<MovieEntity>()
            .AnyAsync(movie => movie.Id == id, cancellationToken);
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

    public async Task<MovieEntity?> GetWithGenreAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context
            .Set<MovieEntity>()
            .AsNoTracking()
            .Include(movie => movie.Genre)
            .FirstOrDefaultAsync(
                movie => movie.Id == id,
                cancellationToken
            );
    }

    public async Task<MovieEntity?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context
            .Set<MovieEntity>()
            .AsNoTracking()
            .Include(movie => movie.Genre)
            .Include(movie => movie.MovieDetails)
            .Include(movie => movie.Reviews)
            .Include(movie => movie.Actors)
            .FirstOrDefaultAsync(
                movie => movie.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<MovieEntity>> GetFilteredAsync(string? genre, int? year, string? actor, CancellationToken cancellationToken = default)
    {
        IQueryable<MovieEntity> query = _context
            .Set<MovieEntity>()
            .AsNoTracking()
            .Include(movie => movie.Genre);

        if (!string.IsNullOrWhiteSpace(genre))
        {
            string trimmedGenre = genre.Trim();

            query = query.Where(movie =>
                movie.Genre != null &&
                movie.Genre.Name == trimmedGenre);
        }

        if (year.HasValue)
        {
            query = query.Where(movie => movie.Year == year.Value);
        }

        if (!string.IsNullOrWhiteSpace(actor))
        {
            string trimmedActor = actor.Trim();

            query = query.Where(movie =>
                movie.Actors.Any(movieActor =>
                    movieActor.Name == trimmedActor));
        }

        return await query
            .OrderBy(movie => movie.Title)
            .ThenBy(movie => movie.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<MovieEntity?> GetWithActorsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context
            .Set<MovieEntity>()
            .Include(movie => movie.Actors)
            .FirstOrDefaultAsync(
                movie => movie.Id == id,
                cancellationToken
            );
    }

}
