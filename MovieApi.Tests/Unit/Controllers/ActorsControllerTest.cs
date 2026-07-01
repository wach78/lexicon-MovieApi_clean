using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Movie.Core.DTOs.Actor;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
using Movie.Presentation.Controllers;
using Movie.Service.Contracts.Interfaces;
using Movie.Service.Contracts.Results;
using Xunit;

namespace MovieApi.Tests.Unit.Controllers;

public sealed class ActorsControllerTest
{
    private readonly Mock<IActorService> _actorServiceMock;
    private readonly Mock<IServiceManager> _serviceManagerMock;
    private readonly ActorsController _controller;
    private readonly Mock<ILogger<ActorsController>> _loggerMock;

    public ActorsControllerTest()
    {
        _actorServiceMock =
            new Mock<IActorService>(MockBehavior.Strict);

        _serviceManagerMock =
            new Mock<IServiceManager>(MockBehavior.Strict);

        _loggerMock = new Mock<ILogger<ActorsController>>();

        _serviceManagerMock
            .SetupGet(serviceManager => serviceManager.Actors)
            .Returns(_actorServiceMock.Object);

        _controller = new ActorsController(
            _serviceManagerMock.Object,
             _loggerMock.Object
        );
    }

    [Fact]
    public async Task AddActorToMovie_WhenActorIsAdded_ReturnsNoContent()
    {
        Guid movieId = Guid.CreateVersion7();
        Guid actorId = Guid.CreateVersion7();
        CancellationToken cancellationToken = CancellationToken.None;

        _actorServiceMock
            .Setup(service => service.AddActorToMovieAsync(
                movieId,
                actorId,
                cancellationToken))
            .ReturnsAsync(AddActorToMovieResult.Added);

        IActionResult actionResult =
            await _controller.AddActorToMovie(
                movieId,
                actorId,
                cancellationToken);

        Assert.IsType<NoContentResult>(actionResult);

        VerifyServiceCalls(movieId, actorId, cancellationToken);
    }

    [Fact]
    public async Task AddActorToMovie_WhenMovieDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        Guid movieId = Guid.CreateVersion7();
        Guid actorId = Guid.CreateVersion7();
        CancellationToken cancellationToken = CancellationToken.None;

        _actorServiceMock
            .Setup(service => service.AddActorToMovieAsync(
                movieId,
                actorId,
                cancellationToken))
            .ReturnsAsync(AddActorToMovieResult.MovieNotFound);

        IActionResult actionResult =
            await _controller.AddActorToMovie(
                movieId,
                actorId,
                cancellationToken);

        NotFoundObjectResult notFoundResult =
            Assert.IsType<NotFoundObjectResult>(actionResult);

        Assert.Equal(
            "Movie was not found.",
            notFoundResult.Value);

        VerifyServiceCalls(movieId, actorId, cancellationToken);
    }

    [Fact]
    public async Task AddActorToMovie_WhenActorDoesNotExist_ReturnsNotFound()
    {
        Guid movieId = Guid.CreateVersion7();
        Guid actorId = Guid.CreateVersion7();
        CancellationToken cancellationToken = CancellationToken.None;

        _actorServiceMock
            .Setup(service => service.AddActorToMovieAsync(
                movieId,
                actorId,
                cancellationToken))
            .ReturnsAsync(AddActorToMovieResult.ActorNotFound);

        IActionResult actionResult =
            await _controller.AddActorToMovie(
                movieId,
                actorId,
                cancellationToken);

        NotFoundObjectResult notFoundResult =
            Assert.IsType<NotFoundObjectResult>(actionResult);

        Assert.Equal(
            "Actor was not found.",
            notFoundResult.Value);

        VerifyServiceCalls(movieId, actorId, cancellationToken);
    }

    [Fact]
    public async Task AddActorToMovie_WhenActorIsAlreadyAdded_ReturnsBadRequest()
    {
        Guid movieId = Guid.CreateVersion7();
        Guid actorId = Guid.CreateVersion7();
        CancellationToken cancellationToken = CancellationToken.None;

        _actorServiceMock
            .Setup(service => service.AddActorToMovieAsync(
                movieId,
                actorId,
                cancellationToken))
            .ReturnsAsync(AddActorToMovieResult.ActorAlreadyAdded);

        IActionResult actionResult =
            await _controller.AddActorToMovie(
                movieId,
                actorId,
                cancellationToken);

        BadRequestObjectResult badRequestResult =
            Assert.IsType<BadRequestObjectResult>(actionResult);

        Assert.Equal(
            "Actor is already added to this movie.",
            badRequestResult.Value);

        VerifyServiceCalls(movieId, actorId, cancellationToken);
    }

    [Fact]
    public async Task AddActorToMovie_WhenServiceReturnsUnknownResult_ReturnsInternalServerError()
    {
        Guid movieId = Guid.CreateVersion7();
        Guid actorId = Guid.CreateVersion7();
        CancellationToken cancellationToken = CancellationToken.None;

        AddActorToMovieResult unknownResult =
            (AddActorToMovieResult)999;

        _actorServiceMock
            .Setup(service => service.AddActorToMovieAsync(
                movieId,
                actorId,
                cancellationToken))
            .ReturnsAsync(unknownResult);

        IActionResult actionResult =
            await _controller.AddActorToMovie(
                movieId,
                actorId,
                cancellationToken);

        StatusCodeResult statusCodeResult =
            Assert.IsType<StatusCodeResult>(actionResult);

        Assert.Equal(
            StatusCodes.Status500InternalServerError,
            statusCodeResult.StatusCode);

        VerifyServiceCalls(movieId, actorId, cancellationToken);
    }

    private void VerifyServiceCalls(
        Guid movieId,
        Guid actorId,
        CancellationToken cancellationToken)
    {
        _serviceManagerMock.VerifyGet(
            serviceManager => serviceManager.Actors,
            Times.Once);

        _actorServiceMock.Verify(
            service => service.AddActorToMovieAsync(
                movieId,
                actorId,
                cancellationToken),
            Times.Once);

        _serviceManagerMock.VerifyNoOtherCalls();
        _actorServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetActors_WhenServiceReturnsActors_ReturnsOkWithPagedActors()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        PaginationParameters paginationParameters = new()
        {
            Page = 1,
            PageSize = 100
        };

        IReadOnlyList<ActorDto> actors =
        [
            new ActorDto
        {
            Id = Guid.CreateVersion7(),
            Name = "Keanu Reeves",
            BirthYear = 1964
        },
        new ActorDto
        {
            Id = Guid.CreateVersion7(),
            Name = "Carrie-Anne Moss",
            BirthYear = 1967
        }
        ];

        PagedResult<ActorDto> expectedResult = new()
        {
            Items = actors,
            TotalItems = 2,
            CurrentPage = paginationParameters.Page,
            TotalPages = 1,
            PageSize = paginationParameters.PageSize
        };

        _actorServiceMock
            .Setup(service => service.GetActorsAsync(
                paginationParameters,
                cancellationToken))
            .ReturnsAsync(expectedResult);

        ActionResult<PagedResult<ActorDto>> actionResult =
            await _controller.GetActors(
                paginationParameters,
                cancellationToken);

        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(actionResult.Result);

        PagedResult<ActorDto> returnedResult =
            Assert.IsType<PagedResult<ActorDto>>(okResult.Value);

        Assert.Same(expectedResult, returnedResult);
        Assert.Same(actors, returnedResult.Items);
        Assert.Equal(2, returnedResult.TotalItems);
        Assert.Equal(paginationParameters.Page, returnedResult.CurrentPage);
        Assert.Equal(1, returnedResult.TotalPages);
        Assert.Equal(paginationParameters.PageSize, returnedResult.PageSize);

        _serviceManagerMock.VerifyGet(
            serviceManager => serviceManager.Actors,
            Times.Once);

        _actorServiceMock.Verify(
            service => service.GetActorsAsync(
                paginationParameters,
                cancellationToken),
            Times.Once);

        _serviceManagerMock.VerifyNoOtherCalls();
        _actorServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetActor_WhenActorExists_ReturnsOkWithActor()
    {
        Guid actorId = Guid.CreateVersion7();
        CancellationToken cancellationToken = CancellationToken.None;

        ActorDto actor = new()
        {
            Id = actorId,
            Name = "Keanu Reeves",
            BirthYear = 1964
        };

        _actorServiceMock
            .Setup(service => service.GetActorByIdAsync(
                actorId,
                cancellationToken))
            .ReturnsAsync(actor);

        ActionResult<ActorDto> actionResult =
            await _controller.GetActor(
                actorId,
                cancellationToken);

        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(actionResult.Result);

        ActorDto returnedActor =
            Assert.IsType<ActorDto>(okResult.Value);

        Assert.Same(actor, returnedActor);

        _serviceManagerMock.VerifyGet(
            serviceManager => serviceManager.Actors,
            Times.Once);

        _actorServiceMock.Verify(
            service => service.GetActorByIdAsync(
                actorId,
                cancellationToken),
            Times.Once);

        _serviceManagerMock.VerifyNoOtherCalls();
        _actorServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetActor_WhenActorDoesNotExist_ReturnsNotFound()
    {
        Guid actorId = Guid.CreateVersion7();
        CancellationToken cancellationToken = CancellationToken.None;

        _actorServiceMock
            .Setup(service => service.GetActorByIdAsync(
                actorId,
                cancellationToken))
            .ReturnsAsync((ActorDto?)null);

        ActionResult<ActorDto> actionResult =
            await _controller.GetActor(
                actorId,
                cancellationToken);

        Assert.IsType<NotFoundResult>(actionResult.Result);

        _serviceManagerMock.VerifyGet(
            serviceManager => serviceManager.Actors,
            Times.Once);

        _actorServiceMock.Verify(
            service => service.GetActorByIdAsync(
                actorId,
                cancellationToken),
            Times.Once);

        _serviceManagerMock.VerifyNoOtherCalls();
        _actorServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PostActor_WhenServiceCreatesActor_ReturnsCreatedAtActionWithActor()
    {
        // Arrange
        CancellationToken cancellationToken = CancellationToken.None;

        ActorCreateDto actorCreateDto = new()
        {
            Name = "Keanu Reeves",
            BirthYear = 1964
        };

        ActorDto createdActor = new()
        {
            Id = Guid.CreateVersion7(),
            Name = actorCreateDto.Name,
            BirthYear = actorCreateDto.BirthYear
        };

        _actorServiceMock
            .Setup(service => service.CreateActorAsync(
                actorCreateDto,
                cancellationToken))
            .ReturnsAsync(createdActor);

        ActionResult<ActorDto> actionResult =
            await _controller.PostActor(
                actorCreateDto,
                cancellationToken);

        CreatedAtActionResult createdResult =
            Assert.IsType<CreatedAtActionResult>(actionResult.Result);

        Assert.Equal(
            StatusCodes.Status201Created,
            createdResult.StatusCode);

        Assert.Equal(
            nameof(ActorsController.GetActor),
            createdResult.ActionName);

        Assert.NotNull(createdResult.RouteValues);
        Assert.Equal(
            createdActor.Id,
            createdResult.RouteValues["id"]);

        Assert.Same(createdActor, createdResult.Value);

        _serviceManagerMock.VerifyGet(
            serviceManager => serviceManager.Actors,
            Times.Once);

        _actorServiceMock.Verify(
            service => service.CreateActorAsync(
                actorCreateDto,
                cancellationToken),
            Times.Once);

        _serviceManagerMock.VerifyNoOtherCalls();
        _actorServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PutActor_WhenRouteIdDoesNotMatchBodyId_ReturnsBadRequest()
    {
        Guid routeId = Guid.CreateVersion7();

        ActorUpdateDto actorUpdateDto = new()
        {
            Id = Guid.CreateVersion7(),
            Name = "Keanu Reeves",
            BirthYear = 1964
        };

        CancellationToken cancellationToken = CancellationToken.None;

        IActionResult actionResult =
            await _controller.PutActor(
                routeId,
                actorUpdateDto,
                cancellationToken);

        Assert.IsType<BadRequestResult>(actionResult);

        _serviceManagerMock.VerifyNoOtherCalls();
        _actorServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PutActor_WhenActorDoesNotExist_ReturnsNotFound()
    {
        Guid actorId = Guid.CreateVersion7();
        CancellationToken cancellationToken = CancellationToken.None;

        ActorUpdateDto actorUpdateDto = new()
        {
            Id = actorId,
            Name = "Keanu Reeves",
            BirthYear = 1964
        };

        _actorServiceMock
            .Setup(service => service.UpdateActorAsync(
                actorId,
                actorUpdateDto,
                cancellationToken))
            .ReturnsAsync(false);

        IActionResult actionResult =
            await _controller.PutActor(
                actorId,
                actorUpdateDto,
                cancellationToken);

        Assert.IsType<NotFoundResult>(actionResult);

        _serviceManagerMock.VerifyGet(
            serviceManager => serviceManager.Actors,
            Times.Once);

        _actorServiceMock.Verify(
            service => service.UpdateActorAsync(
                actorId,
                actorUpdateDto,
                cancellationToken),
            Times.Once);

        _serviceManagerMock.VerifyNoOtherCalls();
        _actorServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PutActor_WhenActorIsUpdated_ReturnsNoContent()
    {
        Guid actorId = Guid.CreateVersion7();
        CancellationToken cancellationToken = CancellationToken.None;

        ActorUpdateDto actorUpdateDto = new()
        {
            Id = actorId,
            Name = "Keanu Reeves",
            BirthYear = 1964
        };

        _actorServiceMock
            .Setup(service => service.UpdateActorAsync(
                actorId,
                actorUpdateDto,
                cancellationToken))
            .ReturnsAsync(true);

        IActionResult actionResult =
            await _controller.PutActor(
                actorId,
                actorUpdateDto,
                cancellationToken);

        Assert.IsType<NoContentResult>(actionResult);

        _serviceManagerMock.VerifyGet(
            serviceManager => serviceManager.Actors,
            Times.Once);

        _actorServiceMock.Verify(
            service => service.UpdateActorAsync(
                actorId,
                actorUpdateDto,
                cancellationToken),
            Times.Once);

        _serviceManagerMock.VerifyNoOtherCalls();
        _actorServiceMock.VerifyNoOtherCalls();
    }
}
