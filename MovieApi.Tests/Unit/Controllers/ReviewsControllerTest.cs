using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Movie.Core.DTOs.Review;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
using Movie.Presentation.Controllers;
using Movie.Service.Contracts.Interfaces;
namespace MovieApi.Tests.Unit.Controllers;

public class ReviewsControllerTest
{
    private readonly Mock<IReviewService> _reviewServiceMock;
    private readonly Mock<IServiceManager> _serviceManagerMock;
    private readonly ReviewsController _controller;

    public ReviewsControllerTest()
    {
        _reviewServiceMock =
            new Mock<IReviewService>(MockBehavior.Strict);

        _serviceManagerMock =
            new Mock<IServiceManager>(MockBehavior.Strict);

        _serviceManagerMock
            .SetupGet(serviceManager => serviceManager.Reviews)
            .Returns(_reviewServiceMock.Object);

        _controller = new ReviewsController(
            _serviceManagerMock.Object
        );
    }

    [Fact]
    public async Task GetMovieReviews_WhenServiceReturnsReviews_ReturnsOkWithReviews()
    {
        Guid movieId = Guid.CreateVersion7();
        CancellationToken cancellationToken = CancellationToken.None;

        PaginationParameters paginationParameters = new()
        {
            Page = 1,
            PageSize = 10
        };

        IReadOnlyList<ReviewDto> reviews =
        [
            new ReviewDto
        {
            Id = Guid.CreateVersion7(),
            ReviewerName = "Reviewer",
            Comment = "Good movie.",
            Rating = 4
        }
        ];

        PagedResult<ReviewDto> expectedResult = new()
        {
            Items = reviews,
            TotalItems = 1,
            CurrentPage = paginationParameters.Page,
            TotalPages = 1,
            PageSize = paginationParameters.PageSize
        };

        _reviewServiceMock
            .Setup(service => service.GetReviewsByMovieIdAsync(
                movieId,
                paginationParameters,
                cancellationToken))
            .ReturnsAsync(expectedResult);

        ActionResult<PagedResult<ReviewDto>> actionResult =
            await _controller.GetMovieReviews(
                movieId,
                paginationParameters,
                cancellationToken);

        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(actionResult.Result);

        PagedResult<ReviewDto> returnedResult =
            Assert.IsType<PagedResult<ReviewDto>>(okResult.Value);

        Assert.Same(expectedResult, returnedResult);
        Assert.Same(reviews, returnedResult.Items);
        Assert.Equal(1, returnedResult.TotalItems);
        Assert.Equal(1, returnedResult.CurrentPage);
        Assert.Equal(1, returnedResult.TotalPages);
        Assert.Equal(10, returnedResult.PageSize);

        _reviewServiceMock.Verify(
            service => service.GetReviewsByMovieIdAsync(
                movieId,
                paginationParameters,
                cancellationToken),
            Times.Once);

        _reviewServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMovieReviews_WhenServiceReturnsNull_ReturnsNotFound()
    {
        Guid movieId = Guid.CreateVersion7();
        CancellationToken cancellationToken = CancellationToken.None;

        PaginationParameters paginationParameters = new()
        {
            Page = 1,
            PageSize = 10
        };

        _reviewServiceMock
            .Setup(service => service.GetReviewsByMovieIdAsync(
                movieId,
                paginationParameters,
                cancellationToken))
            .ReturnsAsync((PagedResult<ReviewDto>?)null);

        ActionResult<PagedResult<ReviewDto>> actionResult =
            await _controller.GetMovieReviews(
                movieId,
                paginationParameters,
                cancellationToken);

        Assert.IsType<NotFoundResult>(actionResult.Result);

        _reviewServiceMock.Verify(
            service => service.GetReviewsByMovieIdAsync(
                movieId,
                paginationParameters,
                cancellationToken),
            Times.Once);

        _reviewServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetReview_WhenServiceReturnsReviews_ReturnsOkWithPagedReviews()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        PaginationParameters paginationParameters = new()
        {
            Page = 1,
            PageSize = 10
        };

        IReadOnlyList<ReviewDto> reviews =
        [
            new ReviewDto
        {
            Id = Guid.CreateVersion7(),
            ReviewerName = "Reviewer",
            Comment = "Good movie.",
            Rating = 4
        }
        ];

        PagedResult<ReviewDto> expectedResult = new()
        {
            Items = reviews,
            TotalItems = 1,
            CurrentPage = paginationParameters.Page,
            TotalPages = 1,
            PageSize = paginationParameters.PageSize
        };

        _reviewServiceMock
            .Setup(service => service.GetReviewsAsync(
                paginationParameters,
                cancellationToken))
            .ReturnsAsync(expectedResult);

        ActionResult<PagedResult<ReviewDto>> actionResult =
            await _controller.GetReview(
                paginationParameters,
                cancellationToken);

        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(actionResult.Result);

        PagedResult<ReviewDto> returnedResult =
            Assert.IsType<PagedResult<ReviewDto>>(okResult.Value);

        Assert.Same(expectedResult, returnedResult);
        Assert.Same(reviews, returnedResult.Items);
        Assert.Equal(1, returnedResult.TotalItems);
        Assert.Equal(paginationParameters.Page, returnedResult.CurrentPage);
        Assert.Equal(1, returnedResult.TotalPages);
        Assert.Equal(paginationParameters.PageSize, returnedResult.PageSize);

        _reviewServiceMock.Verify(
            service => service.GetReviewsAsync(
                paginationParameters,
                cancellationToken),
            Times.Once);

        _reviewServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PostReview_WhenServiceCreatesReview_ReturnsCreatedWithReview()
    {
        // Arrange
        Guid movieId = Guid.CreateVersion7();
        CancellationToken cancellationToken = CancellationToken.None;

        ReviewCreateDto reviewCreateDto = new()
        {
            ReviewerName = "Reviewer",
            Comment = "Good movie.",
            Rating = 4
        };

        ReviewDto createdReview = new()
        {
            Id = Guid.CreateVersion7(),
            ReviewerName = reviewCreateDto.ReviewerName,
            Comment = reviewCreateDto.Comment,
            Rating = reviewCreateDto.Rating
        };

        _reviewServiceMock
            .Setup(service => service.CreateReviewAsync(
                movieId,
                reviewCreateDto,
                cancellationToken))
            .ReturnsAsync(createdReview);

        ActionResult<ReviewDto> actionResult =
            await _controller.PostReview(
                movieId,
                reviewCreateDto,
                cancellationToken);

        CreatedResult createdResult =
            Assert.IsType<CreatedResult>(actionResult.Result);

        Assert.Equal($"/api/reviews/{createdReview.Id}", createdResult.Location);
        Assert.Same(createdReview, createdResult.Value);

        _reviewServiceMock.Verify(
            service => service.CreateReviewAsync(
                movieId,
                reviewCreateDto,
                cancellationToken),
            Times.Once);

        _reviewServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PostReview_WhenServiceReturnsNull_ReturnsNotFound()
    {
        Guid movieId = Guid.CreateVersion7();
        CancellationToken cancellationToken = CancellationToken.None;

        ReviewCreateDto reviewCreateDto = new()
        {
            ReviewerName = "Reviewer",
            Comment = "Good movie.",
            Rating = 4
        };

        _reviewServiceMock
            .Setup(service => service.CreateReviewAsync(
                movieId,
                reviewCreateDto,
                cancellationToken))
            .ReturnsAsync((ReviewDto?)null);

        ActionResult<ReviewDto> actionResult =
            await _controller.PostReview(
                movieId,
                reviewCreateDto,
                cancellationToken);

        Assert.IsType<NotFoundResult>(actionResult.Result);

        _reviewServiceMock.Verify(
            service => service.CreateReviewAsync(
                movieId,
                reviewCreateDto,
                cancellationToken),
            Times.Once);

        _reviewServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteReview_WhenServiceDeletesReview_ReturnsNoContent()
    {
        Guid reviewId = Guid.CreateVersion7();
        CancellationToken cancellationToken = CancellationToken.None;

        _reviewServiceMock
            .Setup(service => service.DeleteReviewAsync(
                reviewId,
                cancellationToken))
            .ReturnsAsync(true);

        IActionResult actionResult =
            await _controller.DeleteReview(reviewId, cancellationToken);

        Assert.IsType<NoContentResult>(actionResult);

        _reviewServiceMock.Verify(
            service => service.DeleteReviewAsync(
                reviewId,
                cancellationToken),
            Times.Once);

        _reviewServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteReview_WhenReviewDoesNotExist_ReturnsNotFound()
    {
        Guid reviewId = Guid.CreateVersion7();
        CancellationToken cancellationToken = CancellationToken.None;

        _reviewServiceMock
            .Setup(service => service.DeleteReviewAsync(
                reviewId,
                cancellationToken))
            .ReturnsAsync(false);

        IActionResult actionResult =
            await _controller.DeleteReview(reviewId, cancellationToken);

        Assert.IsType<NotFoundResult>(actionResult);

        _reviewServiceMock.Verify(
            service => service.DeleteReviewAsync(
                reviewId,
                cancellationToken),
            Times.Once);

        _reviewServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void Constructor_WhenServiceManagerIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new ReviewsController(null!));
    }
}
