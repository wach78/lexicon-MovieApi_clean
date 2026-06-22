using Microsoft.EntityFrameworkCore;
using Movie.Core.Entities;
using Movie.Data.Context;

public class MovieApiContext(DbContextOptions<MovieApiContext> options) : DbContext(options), IMovieApiContext
{
    public DbSet<Movie.Core.Entities.Movie> Movie { get; set; } = default!;
    public DbSet<Movie.Core.Entities.Actor> Actors { get; set; } = default!;
    public DbSet<Movie.Core.Entities.Review> Reviews { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MovieDetails>()
            .Property(movieDetails => movieDetails.Budget)
            .HasPrecision(18, 2);

        base.OnModelCreating(modelBuilder);
    }
}
