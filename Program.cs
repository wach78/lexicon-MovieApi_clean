using Microsoft.EntityFrameworkCore;
using Movie.Core.DomainContracts;
using Movie.Data;
using Movie.Data.Context;
using Movie.Data.Repositories;
using MovieApi.Extensions;
using Movie.Service.Contracts.Interfaces;
using Movie.Services;
using Scalar.AspNetCore;

namespace MovieApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("MovieApiContext") ?? throw new InvalidOperationException("Connection string 'MovieApiContext' not found.");

        builder.Services.AddDbContext<MovieApiContext>(options => options.UseSqlServer(connectionString));

        builder.Services.AddScoped<IMovieApiContext>(serviceProvider => serviceProvider.GetRequiredService<MovieApiContext>());
        builder.Services.AddScoped<IMovieService, MovieService>();
        builder.Services.AddScoped<IActorService, ActorService>();
        builder.Services.AddScoped<IReviewService, ReviewService>();
        builder.Services.AddScoped<IGenreService, GenreService>();
        builder.Services.AddScoped<IReportsService, ReportsService>();

        builder.Services.AddScoped<IServiceManager, ServiceManager>();

        builder.Services.AddScoped<IMovieRepository, MovieRepository>();
        builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
        builder.Services.AddScoped<IActorRepository, ActorRepository>();
        builder.Services.AddScoped<IMovieDetailsRepository, MovieDetailsRepository>();
        builder.Services.AddScoped<IGenreRepository, GenreRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        app.SeedData();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
