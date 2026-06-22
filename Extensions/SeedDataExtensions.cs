using Microsoft.EntityFrameworkCore;
using Movie.Data.Context;
using Movie.Data.Seed;

namespace MovieApi.Extensions;

public static class SeedDataExtensions
{
    public static void SeedData(this WebApplication application)
    {
        ArgumentNullException.ThrowIfNull(application);

        using IServiceScope scope = application.Services.CreateScope();

        MovieApiContext context =
            scope.ServiceProvider.GetRequiredService<MovieApiContext>();

        context.Database.Migrate();

        MovieSeeder.SeedData(context);
    }
}
