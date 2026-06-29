using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning.OpenApi;
using Microsoft.EntityFrameworkCore;
using Movie.Core.DomainContracts;
using Movie.Data;
using Movie.Data.Context;
using Movie.Data.Repositories;
using Movie.Presentation.Controllers;
using Movie.Service.Contracts.Interfaces;
using Movie.Services;
using MovieApi.Extensions;
using Scalar.AspNetCore;
using Asp.Versioning.OpenApi.Transformers;

namespace MovieApi;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder =
            WebApplication.CreateBuilder(args);

        string connectionString =
            builder.Configuration.GetConnectionString(
                "MovieApiContext"
            )
            ?? throw new InvalidOperationException(
                "Connection string 'MovieApiContext' not found."
            );

        builder.Services.AddDbContext<MovieApiContext>(
            options => options.UseSqlServer(connectionString)
        );

        builder.Services.AddScoped<IMovieApiContext>(
            serviceProvider =>
                serviceProvider.GetRequiredService<MovieApiContext>()
        );

        // Services
        builder.Services.AddScoped<IMovieService, MovieService>();
        builder.Services.AddScoped<IActorService, ActorService>();
        builder.Services.AddScoped<IReviewService, ReviewService>();
        builder.Services.AddScoped<IGenreService, GenreService>();
        builder.Services.AddScoped<IReportsService, ReportsService>();
        builder.Services.AddScoped<IServiceManager, ServiceManager>();

        // Repositories and Unit of Work
        builder.Services.AddScoped<IMovieRepository, MovieRepository>();
        builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
        builder.Services.AddScoped<IActorRepository, ActorRepository>();
        builder.Services.AddScoped<IMovieDetailsRepository,MovieDetailsRepository>();
        builder.Services.AddScoped<IGenreRepository, GenreRepository>();
        builder.Services.AddScoped<IReportRepository, ReportRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Controllers
        builder.Services
            .AddControllers()
            .AddApplicationPart(
                typeof(MoviesController).Assembly
            );

        // API versioning and versioned OpenAPI documents
        builder.Services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion =
                    new ApiVersion(1, 0);

                options.ReportApiVersions = true;

                options.ApiVersionReader =
                    new UrlSegmentApiVersionReader();
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";

                options.SubstituteApiVersionInUrl = true;
            })
           .AddOpenApi(options =>
           {
               string xmlFileName =
                   $"{typeof(MoviesController).Assembly.GetName().Name}.xml";

               string xmlFilePath =
                   Path.Combine(AppContext.BaseDirectory, xmlFileName);

               XmlCommentsTransformer xmlCommentsTransformer =
                   new(xmlFilePath);

               options.Document.AddOperationTransformer(
                   xmlCommentsTransformer
               );

               options.Document.AddSchemaTransformer(
                   xmlCommentsTransformer
               );

               options.Document.AddScalarTransformers();
           });

        WebApplication app = builder.Build();

        app.SeedData();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi()
                .WithDocumentPerVersion();

            app.MapScalarApiReference(options =>
            {
                options.OperationTitleSource =
                    OperationTitleSource.Path;

                IReadOnlyList<ApiVersionDescription> descriptions =
                    app.DescribeApiVersions();

                foreach (ApiVersionDescription description in descriptions)
                {
                    bool isDefault = string.Equals(
                        description.GroupName,
                        "v1",
                        StringComparison.OrdinalIgnoreCase
                    );

                    options.AddDocument(
                        description.GroupName,
                        description.GroupName,
                        isDefault: isDefault
                    );
                }
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
