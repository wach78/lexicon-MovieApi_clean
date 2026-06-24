using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Movie;
using Movie.Core.DTOs.Report;
using Movie.Presentation.Controllers;
using Movie.Service.Contracts.Interfaces;
using Movie.Services;

namespace MovieApi.Tests.Unit.Controllers;

public sealed class ReportsControllerTest
{
    private readonly Mock<IReportsService> _reportsServiceMock;
    private readonly Mock<IServiceManager> _serviceManagerMock;
    private readonly ReportsController _controller;

    public ReportsControllerTest()
    {
        _reportsServiceMock =
            new Mock<IReportsService>(MockBehavior.Strict);

        _serviceManagerMock =
            new Mock<IServiceManager>(MockBehavior.Strict);

        _serviceManagerMock
            .SetupGet(serviceManager => serviceManager.Reports)
            .Returns(_reportsServiceMock.Object);

        _controller = new ReportsController(
            _serviceManagerMock.Object
        );
    }

    [Fact]
    public async Task GetAverageRatingGenre_WhenServiceReturnsRatings_ReturnsOkWithRatings()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        IReadOnlyList<MovieAverageRatingDto> averageRatings =
        [
            new MovieAverageRatingDto
        {
            GenreId = Guid.CreateVersion7(),
            GenreName = "Action",
            AverageRating = 4.5,
            ReviewCount = 10
        },
        new MovieAverageRatingDto
        {
            GenreId = Guid.CreateVersion7(),
            GenreName = "Drama",
            AverageRating = 3.8,
            ReviewCount = 5
        }
        ];

        _reportsServiceMock
            .Setup(service =>
                service.GetAverageRatingsByGenreAsync(cancellationToken))
            .ReturnsAsync(averageRatings);

        ActionResult<IReadOnlyList<MovieAverageRatingDto>> actionResult =
            await _controller.GetAverageRatingGenre(cancellationToken);

        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(actionResult.Result);

        IReadOnlyList<MovieAverageRatingDto> returnedRatings =
            Assert.IsAssignableFrom<IReadOnlyList<MovieAverageRatingDto>>(
                okResult.Value);

        Assert.Same(averageRatings, returnedRatings);

        _serviceManagerMock.VerifyGet(
            serviceManager => serviceManager.Reports,
            Times.Once);

        _reportsServiceMock.Verify(
            service =>
                service.GetAverageRatingsByGenreAsync(cancellationToken),
            Times.Once);

        _serviceManagerMock.VerifyNoOtherCalls();
        _reportsServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetTop5MoviesPerGenre_WhenServiceReturnsMovies_ReturnsOkWithMovies()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        IReadOnlyList<TopMoviesPerGenreDto> movies =
        [
            new TopMoviesPerGenreDto
        {
            GenreId = Guid.CreateVersion7(),
            GenreName = "Action",
            Movies =
            [
                new TopRatedMovieDto
                {
                    MovieId = Guid.CreateVersion7(),
                    Title = "The Matrix",
                    Year = 1999,
                    AverageRating = 4.8,
                    ReviewCount = 25
                }
            ]
        }
        ];

        _reportsServiceMock
            .Setup(service =>
                service.GetTopMoviesPerGenreAsync(cancellationToken))
            .ReturnsAsync(movies);

        ActionResult<IReadOnlyList<TopMoviesPerGenreDto>> actionResult =
            await _controller.GetTop5MoviesPerGenre(cancellationToken);

        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(actionResult.Result);

        IReadOnlyList<TopMoviesPerGenreDto> returnedMovies =
            Assert.IsAssignableFrom<IReadOnlyList<TopMoviesPerGenreDto>>(
                okResult.Value);

        Assert.Same(movies, returnedMovies);

        _serviceManagerMock.VerifyGet(
            serviceManager => serviceManager.Reports,
            Times.Once);

        _reportsServiceMock.Verify(
            service =>
                service.GetTopMoviesPerGenreAsync(cancellationToken),
            Times.Once);

        _serviceManagerMock.VerifyNoOtherCalls();
        _reportsServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMostActiveActors_WhenServiceReturnsActors_ReturnsOkWithActors()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        IReadOnlyList<MostActiveActorDto> actors =
        [
            new MostActiveActorDto
        {
            ActorId = Guid.CreateVersion7(),
            Name = "Keanu Reeves",
            MovieCount = 12
        },
        new MostActiveActorDto
        {
            ActorId = Guid.CreateVersion7(),
            Name = "Carrie-Anne Moss",
            MovieCount = 8
        }
        ];

        _reportsServiceMock
            .Setup(service =>
                service.GetMostActiveActorsAsync(cancellationToken))
            .ReturnsAsync(actors);

        ActionResult<IReadOnlyList<MostActiveActorDto>> actionResult =
            await _controller.GetMostActiveActors(cancellationToken);

        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(actionResult.Result);

        IReadOnlyList<MostActiveActorDto> returnedActors =
            Assert.IsAssignableFrom<IReadOnlyList<MostActiveActorDto>>(
                okResult.Value);

        Assert.Same(actors, returnedActors);

        _serviceManagerMock.VerifyGet(
            serviceManager => serviceManager.Reports,
            Times.Once);

        _reportsServiceMock.Verify(
            service =>
                service.GetMostActiveActorsAsync(cancellationToken),
            Times.Once);

        _serviceManagerMock.VerifyNoOtherCalls();
        _reportsServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMovieWithMostReviews_WhenMovieExists_ReturnsOkWithMovie()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        MovieWithMostReviewsDto movie = new()
        {
            MovieId = Guid.CreateVersion7(),
            Title = "The Matrix",
            ReviewCount = 25
        };

        _reportsServiceMock
            .Setup(service =>
                service.GetMovieWithMostReviewsAsync(cancellationToken))
            .ReturnsAsync(movie);

        ActionResult<MovieWithMostReviewsDto> actionResult =
            await _controller.GetMovieWithMostReviews(cancellationToken);

        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(actionResult.Result);

        MovieWithMostReviewsDto returnedMovie =
            Assert.IsType<MovieWithMostReviewsDto>(okResult.Value);

        Assert.Same(movie, returnedMovie);

        _serviceManagerMock.VerifyGet(
            serviceManager => serviceManager.Reports,
            Times.Once);

        _reportsServiceMock.Verify(
            service =>
                service.GetMovieWithMostReviewsAsync(cancellationToken),
            Times.Once);

        _serviceManagerMock.VerifyNoOtherCalls();
        _reportsServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMovieWithMostReviews_WhenMovieDoesNotExist_ReturnsNotFound()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        _reportsServiceMock
            .Setup(service =>
                service.GetMovieWithMostReviewsAsync(cancellationToken))
            .ReturnsAsync((MovieWithMostReviewsDto?)null);

        ActionResult<MovieWithMostReviewsDto> actionResult =
            await _controller.GetMovieWithMostReviews(cancellationToken);

        Assert.IsType<NotFoundResult>(actionResult.Result);

        _serviceManagerMock.VerifyGet(
            serviceManager => serviceManager.Reports,
            Times.Once);

        _reportsServiceMock.Verify(
            service =>
                service.GetMovieWithMostReviewsAsync(cancellationToken),
            Times.Once);

        _serviceManagerMock.VerifyNoOtherCalls();
        _reportsServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetPopularGenre_WhenServiceReturnsGenres_ReturnsOkWithGenres()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        IReadOnlyList<PopularGenreDto> genres =
        [
            new PopularGenreDto(),
        new PopularGenreDto()
        ];

        _reportsServiceMock
            .Setup(service =>
                service.GetPopularGenresAsync(cancellationToken))
            .ReturnsAsync(genres);

        ActionResult<IReadOnlyList<PopularGenreDto>> actionResult =
            await _controller.GetPopularGenre(cancellationToken);

        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(actionResult.Result);

        IReadOnlyList<PopularGenreDto> returnedGenres =
            Assert.IsAssignableFrom<IReadOnlyList<PopularGenreDto>>(
                okResult.Value);

        Assert.Same(genres, returnedGenres);

        _serviceManagerMock.VerifyGet(
            serviceManager => serviceManager.Reports,
            Times.Once);

        _reportsServiceMock.Verify(
            service =>
                service.GetPopularGenresAsync(cancellationToken),
            Times.Once);

        _serviceManagerMock.VerifyNoOtherCalls();
        _reportsServiceMock.VerifyNoOtherCalls();
    }
}
