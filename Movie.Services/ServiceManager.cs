using Movie.Core.Entities;
using Movie.Service.Contracts.Interfaces;

namespace Movie.Services;

public sealed class ServiceManager : IServiceManager
{
    public ServiceManager(
       IMovieService movies,
       IActorService actors,
       IReviewService reviews,
       IReportsService reports,
       IGenreService genres)
    {
        ArgumentNullException.ThrowIfNull(movies);
        ArgumentNullException.ThrowIfNull(actors);
        ArgumentNullException.ThrowIfNull(reviews);
        ArgumentNullException.ThrowIfNull(reports);

        Movies = movies;
        Actors = actors;
        Reviews = reviews;
        Reports = reports;
        Genres = genres;
    }

    public IMovieService Movies { get; }

    public IActorService Actors { get; }

    public IReviewService Reviews { get; }

    public IReportsService Reports { get; }

    public IGenreService Genres { get; }
}
