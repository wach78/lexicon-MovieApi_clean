using Microsoft.EntityFrameworkCore;
using Movie.Core.Entities;

namespace Movie.Data.Context;

public interface IMovieApiContext
{
    DbSet<Core.Entities.Movie> Movie { get; set; }
    DbSet<Actor> Actors { get; set; }
    DbSet<Review> Reviews { get; set; }
    //DbSet<Genre> Genres { get; set; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
