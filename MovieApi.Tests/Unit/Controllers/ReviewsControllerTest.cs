using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MovieApi.Controllers;
using MovieApi.DTOs.Review;
using MovieApi.Interfaces.Service;
using MovieApi.Services;

namespace MovieApi.Tests.Unit.Controllers;

public class ReviewsControllerTest
{
    private readonly Mock<IReviewService> _reviewServiceMock;
    private readonly Mock<MovieApiContext> _contextMock;
    private readonly ReviewsController _controller;

    public ReviewsControllerTest()
    {
        DbContextOptions<MovieApiContext> options =
            new DbContextOptionsBuilder<MovieApiContext>()
                .Options;

        _contextMock = new Mock<MovieApiContext>(options);
        _reviewServiceMock = new Mock<IReviewService>(MockBehavior.Strict);

        _controller = new ReviewsController(
            _contextMock.Object,
            _reviewServiceMock.Object);
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
