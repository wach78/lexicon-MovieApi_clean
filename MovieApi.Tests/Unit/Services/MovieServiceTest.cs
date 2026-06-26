using Moq;
using Movie.Core.DomainContracts;
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Movie;
using Movie.Core.DTOs.Review;
using Movie.Core.Entities;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
using Movie.Service.Contracts.Results;
using Movie.Services;
using MovieEntity = Movie.Core.Entities.Movie;

namespace MovieApi.Tests.Unit.Services;

public sealed class MovieServiceTest
{
    private readonly Mock<IMovieRepository> _movieRepositoryMock;
    private readonly Mock<IGenreRepository> _genreRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly MovieService _service;

    public MovieServiceTest()
    {
        _movieRepositoryMock =
            new Mock<IMovieRepository>(MockBehavior.Strict);

        _genreRepositoryMock =
            new Mock<IGenreRepository>(MockBehavior.Strict);

        _unitOfWorkMock =
            new Mock<IUnitOfWork>(MockBehavior.Strict);

        _unitOfWorkMock
            .SetupGet(unitOfWork => unitOfWork.Movies)
            .Returns(_movieRepositoryMock.Object);

        _unitOfWorkMock
            .SetupGet(unitOfWork => unitOfWork.Genres)
            .Returns(_genreRepositoryMock.Object);

        _service = new MovieService(_unitOfWorkMock.Object);
    }

    [Fact]
    public void Constructor_WhenUnitOfWorkIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new MovieService(null!)
        );
    }

    [Fact]
    public async Task CreateMovieAsync_WhenGenreExists_AddsMovieAndReturnsMovieDto()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        Genre genre = new("Action");

        MovieCreateDto createDto = new()
        {
            Title = "The Matrix",
            Year = 1999,
            Duration = 136,
            GenreId = genre.Id
        };

        MovieEntity? addedMovie = null;

        _genreRepositoryMock
            .Setup(repository => repository.GetAsync(
                genre.Id,
                cancellationToken
            ))
            .ReturnsAsync(genre);

        _movieRepositoryMock
            .Setup(repository => repository.Add(
                It.IsAny<MovieEntity>()
            ))
            .Callback<MovieEntity>(movie => addedMovie = movie);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.CompleteAsync(
                cancellationToken
            ))
            .Returns(Task.CompletedTask);

        MovieDto result = Assert.IsType<MovieDto>(
            await _service.CreateMovieAsync(
                createDto,
                cancellationToken
            )
        );

        Assert.NotNull(addedMovie);

        Assert.Equal(addedMovie.Id, result.Id);
        Assert.Equal(createDto.Title, result.Title);
        Assert.Equal(createDto.Year, result.Year);
        Assert.Equal(createDto.Duration, result.Duration);
        Assert.Equal(genre.Name, result.GenreName);

        Assert.Equal(createDto.Title, addedMovie.Title);
        Assert.Equal(createDto.Year, addedMovie.Year);
        Assert.Equal(createDto.Duration, addedMovie.Duration);
        Assert.Same(genre, addedMovie.Genre);

        _genreRepositoryMock.Verify(
            repository => repository.GetAsync(
                genre.Id,
                cancellationToken
            ),
            Times.Once
        );

        _movieRepositoryMock.Verify(
            repository => repository.Add(
                It.Is<MovieEntity>(movie =>
                    movie.Title == createDto.Title &&
                    movie.Year == createDto.Year &&
                    movie.Duration == createDto.Duration &&
                    movie.Genre == genre
                )
            ),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.CompleteAsync(
                cancellationToken
            ),
            Times.Once
        );

        _movieRepositoryMock.VerifyNoOtherCalls();
        _genreRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateMovieAsync_WhenGenreDoesNotExist_ReturnsNull()
    {
        CancellationToken cancellationToken = CancellationToken.None;
        Guid genreId = Guid.CreateVersion7();

        MovieCreateDto createDto = new()
        {
            Title = "The Matrix",
            Year = 1999,
            Duration = 136,
            GenreId = genreId
        };

        _genreRepositoryMock
            .Setup(repository => repository.GetAsync(
                genreId,
                cancellationToken
            ))
            .ReturnsAsync((Genre?)null);

        MovieDto? result = await _service.CreateMovieAsync(
            createDto,
            cancellationToken
        );

        Assert.Null(result);

        _genreRepositoryMock.Verify(
            repository => repository.GetAsync(
                genreId,
                cancellationToken
            ),
            Times.Once
        );

        _movieRepositoryMock.Verify(
            repository => repository.Add(
                It.IsAny<MovieEntity>()
            ),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.CompleteAsync(
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        _movieRepositoryMock.VerifyNoOtherCalls();
        _genreRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteMovieAsync_WhenMovieExists_RemovesMovieAndReturnsTrue()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        MovieEntity movie = new(
            "The Matrix",
            1999,
            136,
            null
        );

        _movieRepositoryMock
            .Setup(repository => repository.GetAsync(
                movie.Id,
                cancellationToken
            ))
            .ReturnsAsync(movie);

        _movieRepositoryMock
            .Setup(repository => repository.Remove(movie));

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.CompleteAsync(
                cancellationToken
            ))
            .Returns(Task.CompletedTask);

        bool result = await _service.DeleteMovieAsync(
            movie.Id,
            cancellationToken
        );

        Assert.True(result);

        _movieRepositoryMock.Verify(
            repository => repository.GetAsync(
                movie.Id,
                cancellationToken
            ),
            Times.Once
        );

        _movieRepositoryMock.Verify(
            repository => repository.Remove(movie),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.CompleteAsync(
                cancellationToken
            ),
            Times.Once
        );

        _movieRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteMovieAsync_WhenMovieDoesNotExist_ReturnsFalse()
    {
        CancellationToken cancellationToken = CancellationToken.None;
        Guid movieId = Guid.CreateVersion7();

        _movieRepositoryMock
            .Setup(repository => repository.GetAsync(
                movieId,
                cancellationToken
            ))
            .ReturnsAsync((MovieEntity?)null);

        bool result = await _service.DeleteMovieAsync(
            movieId,
            cancellationToken
        );

        Assert.False(result);

        _movieRepositoryMock.Verify(
            repository => repository.GetAsync(
                movieId,
                cancellationToken
            ),
            Times.Once
        );

        _movieRepositoryMock.Verify(
            repository => repository.Remove(
                It.IsAny<MovieEntity>()
            ),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.CompleteAsync(
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        _movieRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMovieByIdAsync_WhenMovieExists_ReturnsMovieDto()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        Genre genre = new("Science Fiction");

        MovieEntity movie = new(
            "The Matrix",
            1999,
            136,
            genre
        );

        _movieRepositoryMock
            .Setup(repository => repository.GetWithGenreAsync(
                movie.Id,
                cancellationToken
            ))
            .ReturnsAsync(movie);

        MovieDto result = Assert.IsType<MovieDto>(
            await _service.GetMovieByIdAsync(
                movie.Id,
                cancellationToken
            )
        );

        Assert.Equal(movie.Id, result.Id);
        Assert.Equal(movie.Title, result.Title);
        Assert.Equal(movie.Year, result.Year);
        Assert.Equal(movie.Duration, result.Duration);
        Assert.Equal(genre.Name, result.GenreName);

        _movieRepositoryMock.Verify(
            repository => repository.GetWithGenreAsync(
                movie.Id,
                cancellationToken
            ),
            Times.Once
        );

        _movieRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMovieByIdAsync_WhenMovieDoesNotExist_ReturnsNull()
    {
        CancellationToken cancellationToken = CancellationToken.None;
        Guid movieId = Guid.CreateVersion7();

        _movieRepositoryMock
            .Setup(repository => repository.GetWithGenreAsync(
                movieId,
                cancellationToken
            ))
            .ReturnsAsync((MovieEntity?)null);

        MovieDto? result = await _service.GetMovieByIdAsync(
            movieId,
            cancellationToken
        );

        Assert.Null(result);

        _movieRepositoryMock.Verify(
            repository => repository.GetWithGenreAsync(
                movieId,
                cancellationToken
            ),
            Times.Once
        );

        _movieRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMovieDetailsAsync_WhenMovieExists_ReturnsMappedMovieDetails()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        Genre genre = new("Science Fiction");

        MovieEntity movie = new(
            "The Matrix",
            1999,
            136,
            genre
        );

        Review review = new(
            "Reviewer",
            "Good movie.",
            4
        );

        Actor actor = new(
            "Keanu Reeves",
            1964
        );

        movie.Reviews.Add(review);
        movie.Actors.Add(actor);

        _movieRepositoryMock
            .Setup(repository => repository.GetWithDetailsAsync(
                movie.Id,
                cancellationToken
            ))
            .ReturnsAsync(movie);

        MovieDetailDto result = Assert.IsType<MovieDetailDto>(
            await _service.GetMovieDetailsAsync(
                movie.Id,
                cancellationToken
            )
        );

        Assert.Equal(movie.Id, result.Id);
        Assert.Equal(movie.Title, result.Title);
        Assert.Equal(movie.Year, result.Year);
        Assert.Equal(movie.Duration, result.Duration);
        Assert.Equal(genre.Name, result.GenreName);

        Assert.Null(result.MovieDetails);

        ReviewDto returnedReview = Assert.Single(result.Reviews);

        Assert.Equal(review.Id, returnedReview.Id);
        Assert.Equal(review.ReviewerName, returnedReview.ReviewerName);
        Assert.Equal(review.Comment, returnedReview.Comment);
        Assert.Equal(review.Rating, returnedReview.Rating);

        ActorDto returnedActor = Assert.Single(result.Actors);

        Assert.Equal(actor.Id, returnedActor.Id);
        Assert.Equal(actor.Name, returnedActor.Name);
        Assert.Equal(actor.BirthYear, returnedActor.BirthYear);

        _movieRepositoryMock.Verify(
            repository => repository.GetWithDetailsAsync(
                movie.Id,
                cancellationToken
            ),
            Times.Once
        );

        _movieRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMovieDetailsAsync_WhenMovieDoesNotExist_ReturnsNull()
    {
        CancellationToken cancellationToken = CancellationToken.None;
        Guid movieId = Guid.CreateVersion7();

        _movieRepositoryMock
            .Setup(repository => repository.GetWithDetailsAsync(
                movieId,
                cancellationToken
            ))
            .ReturnsAsync((MovieEntity?)null);

        MovieDetailDto? result =
            await _service.GetMovieDetailsAsync(
                movieId,
                cancellationToken
            );

        Assert.Null(result);

        _movieRepositoryMock.Verify(
            repository => repository.GetWithDetailsAsync(
                movieId,
                cancellationToken
            ),
            Times.Once
        );

        _movieRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMoviesAsync_WhenRepositoryReturnsMovies_ReturnsMappedPagedResult()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        MovieQueryParameters queryParameters = new()
        {
            Page = 2,
            PageSize = 5
        };

        Genre actionGenre = new("Action");
        Genre dramaGenre = new("Drama");

        IReadOnlyList<MovieEntity> movies =
        [
            new MovieEntity(
                "The Matrix",
                1999,
                136,
                actionGenre
            ),
            new MovieEntity(
                "Forrest Gump",
                1994,
                142,
                dramaGenre
            )
        ];

        PagedResult<MovieEntity> repositoryResult = new()
        {
            Items = movies,
            TotalItems = 12,
            CurrentPage = 2,
            TotalPages = 3,
            PageSize = 5
        };

        _movieRepositoryMock
            .Setup(repository => repository.GetFilteredAsync(
                queryParameters,
                cancellationToken
            ))
            .ReturnsAsync(repositoryResult);

        PagedResult<MovieDto> result =
            await _service.GetMoviesAsync(
                queryParameters,
                cancellationToken
            );

        Assert.Equal(repositoryResult.TotalItems, result.TotalItems);
        Assert.Equal(repositoryResult.CurrentPage, result.CurrentPage);
        Assert.Equal(repositoryResult.TotalPages, result.TotalPages);
        Assert.Equal(repositoryResult.PageSize, result.PageSize);

        Assert.Equal(2, result.Items.Count);

        Assert.Equal(movies[0].Id, result.Items[0].Id);
        Assert.Equal(movies[0].Title, result.Items[0].Title);
        Assert.Equal(actionGenre.Name, result.Items[0].GenreName);

        Assert.Equal(movies[1].Id, result.Items[1].Id);
        Assert.Equal(movies[1].Title, result.Items[1].Title);
        Assert.Equal(dramaGenre.Name, result.Items[1].GenreName);

        _movieRepositoryMock.Verify(
            repository => repository.GetFilteredAsync(
                queryParameters,
                cancellationToken
            ),
            Times.Once
        );

        _movieRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMoviesAsync_WhenQueryParametersIsNull_ThrowsArgumentNullException()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.GetMoviesAsync(
                null!,
                cancellationToken
            )
        );

        _movieRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateMovieAsync_WhenMovieDoesNotExist_ReturnsMovieNotFound()
    {
        CancellationToken cancellationToken = CancellationToken.None;
        Guid movieId = Guid.CreateVersion7();

        MovieUpdateDto updateDto = new()
        {
            Id = movieId,
            Title = "Updated title",
            Year = 2000,
            Duration = 140
        };

        _movieRepositoryMock
            .Setup(repository => repository.GetAsync(
                movieId,
                cancellationToken
            ))
            .ReturnsAsync((MovieEntity?)null);

        UpdateMovieResult result =
            await _service.UpdateMovieAsync(
                movieId,
                updateDto,
                cancellationToken
            );

        Assert.Equal(
            UpdateMovieResult.MovieNotFound,
            result
        );

        _movieRepositoryMock.Verify(
            repository => repository.GetAsync(
                movieId,
                cancellationToken
            ),
            Times.Once
        );

        _genreRepositoryMock.Verify(
            repository => repository.GetAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.CompleteAsync(
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        _movieRepositoryMock.VerifyNoOtherCalls();
        _genreRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateMovieAsync_WhenGenreDoesNotExist_ReturnsGenreNotFound()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        MovieEntity movie = new(
            "The Matrix",
            1999,
            136,
            null
        );

        Guid genreId = Guid.CreateVersion7();

        MovieUpdateDto updateDto = new()
        {
            Id = movie.Id,
            Title = "Updated title",
            Year = 2000,
            Duration = 140,
            GenreId = genreId
        };

        _movieRepositoryMock
            .Setup(repository => repository.GetAsync(
                movie.Id,
                cancellationToken
            ))
            .ReturnsAsync(movie);

        _genreRepositoryMock
            .Setup(repository => repository.GetAsync(
                genreId,
                cancellationToken
            ))
            .ReturnsAsync((Genre?)null);

        UpdateMovieResult result =
            await _service.UpdateMovieAsync(
                movie.Id,
                updateDto,
                cancellationToken
            );

        Assert.Equal(
            UpdateMovieResult.GenreNotFound,
            result
        );

        Assert.Equal("The Matrix", movie.Title);
        Assert.Equal(1999, movie.Year);
        Assert.Equal(136, movie.Duration);

        _movieRepositoryMock.Verify(
            repository => repository.GetAsync(
                movie.Id,
                cancellationToken
            ),
            Times.Once
        );

        _genreRepositoryMock.Verify(
            repository => repository.GetAsync(
                genreId,
                cancellationToken
            ),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.CompleteAsync(
                It.IsAny<CancellationToken>()
            ),
            Times.Never
        );

        _movieRepositoryMock.VerifyNoOtherCalls();
        _genreRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateMovieAsync_WhenMovieAndGenreExist_UpdatesMovieAndReturnsUpdated()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        Genre oldGenre = new("Drama");
        Genre newGenre = new("Action");

        MovieEntity movie = new(
            "Old title",
            1999,
            120,
            oldGenre
        );

        MovieUpdateDto updateDto = new()
        {
            Id = movie.Id,
            Title = "Updated title",
            Year = 2005,
            Duration = 145,
            GenreId = newGenre.Id
        };

        _movieRepositoryMock
            .Setup(repository => repository.GetAsync(
                movie.Id,
                cancellationToken
            ))
            .ReturnsAsync(movie);

        _genreRepositoryMock
            .Setup(repository => repository.GetAsync(
                newGenre.Id,
                cancellationToken
            ))
            .ReturnsAsync(newGenre);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.CompleteAsync(
                cancellationToken
            ))
            .Returns(Task.CompletedTask);

        UpdateMovieResult result =
            await _service.UpdateMovieAsync(
                movie.Id,
                updateDto,
                cancellationToken
            );

        Assert.Equal(UpdateMovieResult.Updated, result);

        Assert.Equal(updateDto.Title, movie.Title);
        Assert.Equal(updateDto.Year, movie.Year);
        Assert.Equal(updateDto.Duration, movie.Duration);
        Assert.Same(newGenre, movie.Genre);

        _movieRepositoryMock.Verify(
            repository => repository.GetAsync(
                movie.Id,
                cancellationToken
            ),
            Times.Once
        );

        _genreRepositoryMock.Verify(
            repository => repository.GetAsync(
                newGenre.Id,
                cancellationToken
            ),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.CompleteAsync(
                cancellationToken
            ),
            Times.Once
        );

        _movieRepositoryMock.VerifyNoOtherCalls();
        _genreRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateMovieAsync_WhenUpdateDtoIsNull_ThrowsArgumentNullException()
    {
        CancellationToken cancellationToken = CancellationToken.None;
        Guid movieId = Guid.CreateVersion7();

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.UpdateMovieAsync(
                movieId,
                null!,
                cancellationToken
            )
        );

        _movieRepositoryMock.VerifyNoOtherCalls();
        _genreRepositoryMock.VerifyNoOtherCalls();
    }
}
