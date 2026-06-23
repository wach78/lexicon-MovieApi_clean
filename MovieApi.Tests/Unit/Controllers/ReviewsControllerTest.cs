using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Movie.Presentation.Controllers;
using Movie.Core.DTOs.Review;
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
        // Arrange
        Guid movieId = Guid.CreateVersion7();
        CancellationToken cancellationToken = CancellationToken.None;

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

        _reviewServiceMock
            .Setup(service => service.GetReviewsByMovieIdAsync(movieId, cancellationToken))
            .ReturnsAsync(reviews);

        ActionResult<IReadOnlyList<ReviewDto>> actionResult =
            await _controller.GetMovieReviews(movieId, cancellationToken);

        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(actionResult.Result);

        IReadOnlyList<ReviewDto> returnedReviews =
            Assert.IsAssignableFrom<IReadOnlyList<ReviewDto>>(okResult.Value);

        Assert.Same(reviews, returnedReviews);

        _reviewServiceMock.Verify(
            service => service.GetReviewsByMovieIdAsync(movieId, cancellationToken),
            Times.Once);

        _reviewServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMovieReviews_WhenServiceReturnsNull_ReturnsNotFound()
    {
        Guid movieId = Guid.CreateVersion7();
        CancellationToken cancellationToken = CancellationToken.None;

        _reviewServiceMock
            .Setup(service => service.GetReviewsByMovieIdAsync(movieId, cancellationToken))
            .ReturnsAsync((IReadOnlyList<ReviewDto>?)null);

        ActionResult<IReadOnlyList<ReviewDto>> actionResult =
            await _controller.GetMovieReviews(movieId, cancellationToken);
        Assert.IsType<NotFoundResult>(actionResult.Result);

        _reviewServiceMock.Verify(
            service => service.GetReviewsByMovieIdAsync(movieId, cancellationToken),
            Times.Once);

        _reviewServiceMock.VerifyNoOtherCalls();
    }
}
