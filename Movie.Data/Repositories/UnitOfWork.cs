using Movie.Core.DomainContracts;
using Movie.Data.Context;

namespace Movie.Data;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly MovieApiContext _context;

    public UnitOfWork(
        MovieApiContext context,
        IMovieRepository movies,
        IReviewRepository reviews,
        IActorRepository actors,
        IMovieDetailsRepository movieDetails,
        IGenreRepository genres,
        IReportRepository reports)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(movies);
        ArgumentNullException.ThrowIfNull(reviews);
        ArgumentNullException.ThrowIfNull(actors);
        ArgumentNullException.ThrowIfNull(movieDetails);
        ArgumentNullException.ThrowIfNull(genres);

        _context = context;

        Movies = movies;
        Reviews = reviews;
        Actors = actors;
        MovieDetails = movieDetails;
        Genres = genres;
        Reports = reports;
    }

    public IMovieRepository Movies { get; }

    public IReviewRepository Reviews { get; }

    public IActorRepository Actors { get; }

    public IMovieDetailsRepository MovieDetails { get; }

    public IGenreRepository Genres { get; }

    public IReportRepository Reports { get; }

    public async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
