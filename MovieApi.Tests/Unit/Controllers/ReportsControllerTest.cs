using Microsoft.AspNetCore.Mvc;
using Moq;
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Movie;
using Movie.Core.DTOs.Report;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
using Movie.Presentation.Controllers;
using Movie.Service.Contracts.Interfaces;

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
    public async Task GetAverageRatingGenre_WhenServiceReturnsRatings_ReturnsOkWithPagedRatings()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        PaginationParameters paginationParameters = new()
        {
            Page = 1,
            PageSize = 10
        };

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

        PagedResult<MovieAverageRatingDto> expectedResult = new()
        {
            Items = averageRatings,
            TotalItems = 2,
            CurrentPage = paginationParameters.Page,
            TotalPages = 1,
            PageSize = paginationParameters.PageSize
        };

        _reportsServiceMock
            .Setup(service =>
                service.GetAverageRatingsByGenreAsync(
                    paginationParameters,
                    cancellationToken
                ))
            .ReturnsAsync(expectedResult);

        ActionResult<PagedResult<MovieAverageRatingDto>> actionResult =
            await _controller.GetAverageRatingGenre(
                paginationParameters,
                cancellationToken
            );

        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(actionResult.Result);

        PagedResult<MovieAverageRatingDto> returnedResult =
            Assert.IsType<PagedResult<MovieAverageRatingDto>>(
                okResult.Value
            );

        Assert.Same(expectedResult, returnedResult);
        Assert.Same(averageRatings, returnedResult.Items);
        Assert.Equal(2, returnedResult.TotalItems);
        Assert.Equal(1, returnedResult.CurrentPage);
        Assert.Equal(1, returnedResult.TotalPages);
        Assert.Equal(10, returnedResult.PageSize);

        _serviceManagerMock.VerifyGet(
            serviceManager => serviceManager.Reports,
            Times.Once
        );

        _reportsServiceMock.Verify(
            service =>
                service.GetAverageRatingsByGenreAsync(
                    paginationParameters,
                    cancellationToken
                ),
            Times.Once
        );

        _serviceManagerMock.VerifyNoOtherCalls();
        _reportsServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetTop5MoviesPerGenre_WhenServiceReturnsMovies_ReturnsOkWithPagedMovies()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        PaginationParameters paginationParameters = new()
        {
            Page = 1,
            PageSize = 10
        };

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

        PagedResult<TopMoviesPerGenreDto> expectedResult = new()
        {
            Items = movies,
            TotalItems = 1,
            CurrentPage = paginationParameters.Page,
            TotalPages = 1,
            PageSize = paginationParameters.PageSize
        };

        _reportsServiceMock
            .Setup(service =>
                service.GetTopMoviesPerGenreAsync(
                    paginationParameters,
                    cancellationToken
                ))
            .ReturnsAsync(expectedResult);

        ActionResult<PagedResult<TopMoviesPerGenreDto>> actionResult =
            await _controller.GetTop5MoviesPerGenre(
                paginationParameters,
                cancellationToken
            );

        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(actionResult.Result);

        PagedResult<TopMoviesPerGenreDto> returnedResult =
            Assert.IsType<PagedResult<TopMoviesPerGenreDto>>(
                okResult.Value
            );

        Assert.Same(expectedResult, returnedResult);
        Assert.Same(movies, returnedResult.Items);
        Assert.Equal(1, returnedResult.TotalItems);
        Assert.Equal(1, returnedResult.CurrentPage);
        Assert.Equal(1, returnedResult.TotalPages);
        Assert.Equal(10, returnedResult.PageSize);

        _serviceManagerMock.VerifyGet(
            serviceManager => serviceManager.Reports,
            Times.Once
        );

        _reportsServiceMock.Verify(
            service =>
                service.GetTopMoviesPerGenreAsync(
                    paginationParameters,
                    cancellationToken
                ),
            Times.Once
        );

        _serviceManagerMock.VerifyNoOtherCalls();
        _reportsServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMostActiveActors_WhenServiceReturnsActors_ReturnsOkWithPagedActors()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        PaginationParameters paginationParameters = new()
        {
            Page = 1,
            PageSize = 10
        };

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

        PagedResult<MostActiveActorDto> expectedResult = new()
        {
            Items = actors,
            TotalItems = 2,
            CurrentPage = paginationParameters.Page,
            TotalPages = 1,
            PageSize = paginationParameters.PageSize
        };

        _reportsServiceMock
            .Setup(service =>
                service.GetMostActiveActorsAsync(
                    paginationParameters,
                    cancellationToken
                ))
            .ReturnsAsync(expectedResult);

        ActionResult<PagedResult<MostActiveActorDto>> actionResult =
            await _controller.GetMostActiveActors(
                paginationParameters,
                cancellationToken
            );

        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(actionResult.Result);

        PagedResult<MostActiveActorDto> returnedResult =
            Assert.IsType<PagedResult<MostActiveActorDto>>(
                okResult.Value
            );

        Assert.Same(expectedResult, returnedResult);
        Assert.Same(actors, returnedResult.Items);
        Assert.Equal(2, returnedResult.TotalItems);
        Assert.Equal(1, returnedResult.CurrentPage);
        Assert.Equal(1, returnedResult.TotalPages);
        Assert.Equal(10, returnedResult.PageSize);

        _serviceManagerMock.VerifyGet(
            serviceManager => serviceManager.Reports,
            Times.Once
        );

        _reportsServiceMock.Verify(
            service =>
                service.GetMostActiveActorsAsync(
                    paginationParameters,
                    cancellationToken
                ),
            Times.Once
        );

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
            Times.Once
        );

        _reportsServiceMock.Verify(
            service =>
                service.GetMovieWithMostReviewsAsync(cancellationToken),
            Times.Once
        );

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
            Times.Once
        );

        _reportsServiceMock.Verify(
            service =>
                service.GetMovieWithMostReviewsAsync(cancellationToken),
            Times.Once
        );

        _serviceManagerMock.VerifyNoOtherCalls();
        _reportsServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetPopularGenre_WhenServiceReturnsGenres_ReturnsOkWithPagedGenres()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        PaginationParameters paginationParameters = new()
        {
            Page = 1,
            PageSize = 10
        };

        IReadOnlyList<PopularGenreDto> genres =
        [
            new PopularGenreDto(),
            new PopularGenreDto()
        ];

        PagedResult<PopularGenreDto> expectedResult = new()
        {
            Items = genres,
            TotalItems = 2,
            CurrentPage = paginationParameters.Page,
            TotalPages = 1,
            PageSize = paginationParameters.PageSize
        };

        _reportsServiceMock
            .Setup(service =>
                service.GetPopularGenresAsync(
                    paginationParameters,
                    cancellationToken
                ))
            .ReturnsAsync(expectedResult);

        ActionResult<PagedResult<PopularGenreDto>> actionResult =
            await _controller.GetPopularGenre(
                paginationParameters,
                cancellationToken
            );

        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(actionResult.Result);

        PagedResult<PopularGenreDto> returnedResult =
            Assert.IsType<PagedResult<PopularGenreDto>>(
                okResult.Value
            );

        Assert.Same(expectedResult, returnedResult);
        Assert.Same(genres, returnedResult.Items);
        Assert.Equal(2, returnedResult.TotalItems);
        Assert.Equal(1, returnedResult.CurrentPage);
        Assert.Equal(1, returnedResult.TotalPages);
        Assert.Equal(10, returnedResult.PageSize);

        _serviceManagerMock.VerifyGet(
            serviceManager => serviceManager.Reports,
            Times.Once
        );

        _reportsServiceMock.Verify(
            service =>
                service.GetPopularGenresAsync(
                    paginationParameters,
                    cancellationToken
                ),
            Times.Once
        );

        _serviceManagerMock.VerifyNoOtherCalls();
        _reportsServiceMock.VerifyNoOtherCalls();
    }
}
