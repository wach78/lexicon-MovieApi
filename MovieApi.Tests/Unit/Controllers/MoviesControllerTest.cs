using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MovieApi.Controllers;
using MovieApi.DTOs.Actor;
using MovieApi.DTOs.Movie;
using MovieApi.DTOs.Review;
using MovieApi.Emuns;
using MovieApi.Interfaces.Service;
using MovieApi.Models;
using Xunit;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace MovieApi.Tests.Unit.Controllers;

public sealed class MoviesControllerTests
{
    private readonly Mock<IMovieService> _movieServiceMock;
    private readonly MoviesController _controller;

    public MoviesControllerTests()
    {
        _movieServiceMock = new Mock<IMovieService>(MockBehavior.Strict);
        _controller = new MoviesController(_movieServiceMock.Object);
    }

    [Theory]
    [InlineData(null, null, null)]
    [InlineData("Drama", null, null)]
    [InlineData(null, 1994, null)]
    [InlineData(null, null, "Tom Hanks")]
    [InlineData("Drama", 1994, "Tom Hanks")]
    public async Task GetMovies_ReturnsOkWithMovies(string? genre, int? year, string? actor)
    {
        CancellationToken cancellationToken = CancellationToken.None;

        IReadOnlyList<MovieDto> expectedMovies =
        [
            new MovieDto
            {
                Id = Guid.NewGuid(),
                Title = "Forrest Gump",
                Year = 1994,
                Duration = 142,
                GenreId = Guid.NewGuid(),
                GenreName = "Drama"
            }
        ];

        _movieServiceMock
            .Setup(service => service.GetMoviesAsync(
                genre,
                year,
                actor,
                cancellationToken))
            .ReturnsAsync(expectedMovies);

        ActionResult<IEnumerable<MovieDto>> actionResult =
            await _controller.GetMovie(
                genre,
                year,
                actor,
                cancellationToken);

        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(actionResult.Result);

        IReadOnlyList<MovieDto> returnedMovies =
            Assert.IsAssignableFrom<IReadOnlyList<MovieDto>>(okResult.Value);

        Assert.Same(expectedMovies, returnedMovies);

        _movieServiceMock.Verify(
            service => service.GetMoviesAsync(
                genre,
                year,
                actor,
                cancellationToken),
            Times.Once);

        _movieServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMovies_ReturnsOkWithEmptyList()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        IReadOnlyList<MovieDto> expectedMovies = Array.Empty<MovieDto>();

        _movieServiceMock
            .Setup(service => service.GetMoviesAsync(
                null,
                null,
                null,
                cancellationToken))
            .ReturnsAsync(expectedMovies);

        ActionResult<IEnumerable<MovieDto>> actionResult =
            await _controller.GetMovie(
                null,
                null,
                null,
                cancellationToken);

        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(actionResult.Result);

        IReadOnlyList<MovieDto> returnedMovies =
            Assert.IsAssignableFrom<IReadOnlyList<MovieDto>>(okResult.Value);

        Assert.Empty(returnedMovies);

        _movieServiceMock.Verify(
            service => service.GetMoviesAsync(
                null,
                null,
                null,
                cancellationToken),
            Times.Once);

        _movieServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMovie_with_Guid_ReturnsOkWithMovie()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        Guid id = Guid.NewGuid();
        Guid genreId = Guid.NewGuid();

        MovieDto expectedMovie =
            new()
            {
                Id = id,
                Title = "Forrest Gump",
                Year = 1994,
                Duration = 142,
                GenreId = genreId,
                GenreName = "Drama"
            };

        _movieServiceMock
            .Setup(service => service.GetMovieByIdAsync(id, cancellationToken))
            .ReturnsAsync(expectedMovie);

        ActionResult<MovieDto> actionResult = await _controller.GetMovie(id);

        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(actionResult.Result);

        MovieDto returnedMovie =
            Assert.IsType<MovieDto>(okResult.Value);

        Assert.Equal(expectedMovie, returnedMovie);

        _movieServiceMock.Verify(
            service => service.GetMovieByIdAsync(id, cancellationToken),
            Times.Once);

        _movieServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMovie_with_Guid_ReturnsNotFound()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        Guid id = Guid.NewGuid();

        _movieServiceMock
            .Setup(service => service.GetMovieByIdAsync(id, cancellationToken))
            .ReturnsAsync((MovieDto?)null);

        ActionResult<MovieDto> actionResult =
            await _controller.GetMovie(id);

        Assert.IsType<NotFoundResult>(actionResult.Result);

        _movieServiceMock.Verify(
            service => service.GetMovieByIdAsync(id, cancellationToken),
            Times.Once);

        _movieServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMovieDetails_With_Guid_ReturnsOkWithMovieDetails()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        Guid id = Guid.NewGuid();
        Guid genreId = Guid.NewGuid();
        Guid reviewId = Guid.NewGuid();
        Guid actorId = Guid.NewGuid();

        MovieDetailDto expectedMovieDetails = new()
        {
            Id = id,
            Title = "Forrest Gump",
            Year = 1994,
            Duration = 142,
            GenreId = genreId,
            GenreName = "Drama",

            MovieDetails = new MovieDetailsDto
            {
                Id = id,
                Synopsis = "A man recounts the extraordinary events of his life.",
                Language = "English",
                Budget = 55_000_000m
            },

            Reviews =
            [
                new ReviewDto
                {
                    Id = reviewId,
                    ReviewerName = "Test Reviewer",
                    Comment = "A very good movie.",
                    Rating = 5
                }
            ],

            Actors =
            [
                new ActorDto
                {
                    Id = actorId,
                    Name = "Tom Hanks",
                    BirthYear = 1956
                }
            ]
        };

        _movieServiceMock
            .Setup(service => service.GetMovieDetailsAsync(id, cancellationToken))
            .ReturnsAsync(expectedMovieDetails);

        ActionResult<MovieDetailDto> actionResult =
            await _controller.GetMovieDetails(id, cancellationToken);

        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(actionResult.Result);

        MovieDetailDto returnedMovie =
            Assert.IsType<MovieDetailDto>(okResult.Value);

        Assert.Equal(expectedMovieDetails, returnedMovie);

        _movieServiceMock.Verify(
           service => service.GetMovieDetailsAsync(id, cancellationToken),
           Times.Once);

        _movieServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetMovieDetails_With_Guid_ReturnsNotFound()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        Guid id = Guid.NewGuid();

        _movieServiceMock
            .Setup(service => service.GetMovieDetailsAsync(id, cancellationToken))
            .ReturnsAsync((MovieDetailDto?)null);

        ActionResult<MovieDetailDto> actionResult =
           await _controller.GetMovieDetails(id, cancellationToken);

        Assert.IsType<NotFoundResult>(actionResult.Result);

        _movieServiceMock.Verify(
         service => service.GetMovieDetailsAsync(id, cancellationToken),
         Times.Once);

        _movieServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PostMovie_ok()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        MovieCreateDto movieCreateDto = new()
        {
            Title = "Inception",
            Year = 2010,
            Duration = 148,
            GenreId = null
        };

        Guid movieId = Guid.CreateVersion7();

        MovieDto expectedMovieDto = new()
        {
            Id = movieId,
            Title = movieCreateDto.Title,
            Year = movieCreateDto.Year,
            Duration = movieCreateDto.Duration,
            GenreId = movieCreateDto.GenreId,
            GenreName = null
        };

        _movieServiceMock
            .Setup(service => service.CreateMovieAsync(movieCreateDto, cancellationToken))
            .ReturnsAsync(expectedMovieDto);

        ActionResult<MovieDto> createdResult =
            await _controller.PostMovie(movieCreateDto, cancellationToken);

        CreatedAtActionResult okResult =
            Assert.IsType<CreatedAtActionResult>(createdResult.Result);

        MovieDto returnedMovie =
            Assert.IsType<MovieDto>(okResult.Value);

        Assert.Equivalent(expectedMovieDto, returnedMovie);

        _movieServiceMock.Verify(
           service => service.CreateMovieAsync(movieCreateDto, cancellationToken),
           Times.Once);

        _movieServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PostMovie_Not_found_Genre()
    {
        CancellationToken cancellationToken = CancellationToken.None;

        MovieCreateDto movieCreateDto = new()
        {
            Title = "Inception",
            Year = 2010,
            Duration = 148,
            GenreId = Guid.Parse("01912345-6789-7000-8000-000000000001")
        };

        _movieServiceMock
           .Setup(service => service.CreateMovieAsync(movieCreateDto, cancellationToken))
           .ReturnsAsync((MovieDto?)null);

        ActionResult<MovieDto> createdResult =
            await _controller.PostMovie(movieCreateDto, cancellationToken);

        BadRequestObjectResult badRequestResult =
            Assert.IsType<BadRequestObjectResult>(createdResult.Result);

        string errorMessage =
            Assert.IsType<string>(badRequestResult.Value);

        Assert.Equal("Genre does not exist.", errorMessage);

        _movieServiceMock.Verify(
             service => service.CreateMovieAsync(movieCreateDto, cancellationToken),
             Times.Once);

        _movieServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PutMovie_WhenRouteIdDoesNotMatchBodyId()
    {
        CancellationToken cancellationToken = CancellationToken.None;
        Guid routeId = Guid.CreateVersion7();

        MovieUpdateDto movieUpdateDto = new()
        {
            Id = Guid.CreateVersion7(),
            Title = "Inception",
            Year = 2010,
            Duration = 148,
            GenreId = null
        };

        IActionResult actionResult = await _controller.PutMovie(
            routeId,
            movieUpdateDto,
            cancellationToken
        );

        BadRequestObjectResult badRequestResult =
            Assert.IsType<BadRequestObjectResult>(actionResult);

        string errorMessage =
            Assert.IsType<string>(badRequestResult.Value);

        Assert.Equal("Route id does not match body id.", errorMessage);

        _movieServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PutMovie_WhenMovieDoesNotExist()
    {
        CancellationToken cancellationToken = CancellationToken.None;
        Guid movieId = Guid.CreateVersion7();

        MovieUpdateDto movieUpdateDto = new()
        {
            Id = movieId,
            Title = "Inception",
            Year = 2010,
            Duration = 148,
            GenreId = null
        };

        _movieServiceMock
            .Setup(service => service.UpdateMovieAsync(
                movieId,
                movieUpdateDto,
                cancellationToken
            ))
            .ReturnsAsync(UpdateMovieResult.MovieNotFound);

        IActionResult actionResult = await _controller.PutMovie(
            movieId,
            movieUpdateDto,
            cancellationToken
        );

        NotFoundObjectResult notFoundResult =
            Assert.IsType<NotFoundObjectResult>(actionResult);

        string errorMessage =
            Assert.IsType<string>(notFoundResult.Value);

        Assert.Equal("Movie not found.", errorMessage);

        _movieServiceMock.Verify(
            service => service.UpdateMovieAsync(
                movieId,
                movieUpdateDto,
                cancellationToken
            ),
            Times.Once
        );

        _movieServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PutMovie_WhenGenreDoesNotExist()
    {
        CancellationToken cancellationToken = CancellationToken.None;
        Guid movieId = Guid.CreateVersion7();

        MovieUpdateDto movieUpdateDto = new()
        {
            Id = movieId,
            Title = "Inception",
            Year = 2010,
            Duration = 148,
            GenreId = Guid.CreateVersion7()
        };

        _movieServiceMock
            .Setup(service => service.UpdateMovieAsync(
                movieId,
                movieUpdateDto,
                cancellationToken
            ))
            .ReturnsAsync(UpdateMovieResult.GenreNotFound);

        IActionResult actionResult = await _controller.PutMovie(
            movieId,
            movieUpdateDto,
            cancellationToken
        );

        BadRequestObjectResult badRequestResult =
            Assert.IsType<BadRequestObjectResult>(actionResult);

        string errorMessage =
            Assert.IsType<string>(badRequestResult.Value);

        Assert.Equal("Genre does not exist.", errorMessage);

        _movieServiceMock.Verify(
            service => service.UpdateMovieAsync(
                movieId,
                movieUpdateDto,
                cancellationToken
            ),
            Times.Once
        );

        _movieServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PutMovie_WhenMovieIsUpdated_ReturnsNoContent()
    {
        CancellationToken cancellationToken = CancellationToken.None;
        Guid movieId = Guid.CreateVersion7();

        MovieUpdateDto movieUpdateDto = new()
        {
            Id = movieId,
            Title = "Inception",
            Year = 2010,
            Duration = 148,
            GenreId = null
        };

        _movieServiceMock
            .Setup(service => service.UpdateMovieAsync(
                movieId,
                movieUpdateDto,
                cancellationToken
            ))
            .ReturnsAsync(UpdateMovieResult.Updated);

        IActionResult actionResult = await _controller.PutMovie(
            movieId,
            movieUpdateDto,
            cancellationToken
        );

        Assert.IsType<NoContentResult>(actionResult);

        _movieServiceMock.Verify(
            service => service.UpdateMovieAsync(
                movieId,
                movieUpdateDto,
                cancellationToken
            ),
            Times.Once
        );

        _movieServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PutMovie_WhenServiceReturnsUnknownResult_ReturnsInternalServerError()
    {
        CancellationToken cancellationToken = CancellationToken.None;
        Guid movieId = Guid.CreateVersion7();

        MovieUpdateDto movieUpdateDto = new()
        {
            Id = movieId,
            Title = "Inception",
            Year = 2010,
            Duration = 148,
            GenreId = null
        };

        _movieServiceMock
            .Setup(service => service.UpdateMovieAsync(
                movieId,
                movieUpdateDto,
                cancellationToken
            ))
            .ReturnsAsync((UpdateMovieResult)999);

        IActionResult actionResult = await _controller.PutMovie(
            movieId,
            movieUpdateDto,
            cancellationToken
        );

        StatusCodeResult objectResult =
            Assert.IsType<StatusCodeResult>(actionResult);

        Assert.Equal(
            StatusCodes.Status500InternalServerError,
            objectResult.StatusCode
        );

        _movieServiceMock.Verify(
                service => service.UpdateMovieAsync(
                    movieId,
                    movieUpdateDto,
                    cancellationToken
                ),
                Times.Once
            );

        _movieServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteMovie_WhenMovieIsDeleted_ReturnsNoContent()
    {
        CancellationToken cancellationToken = CancellationToken.None;
        Guid movieId = Guid.CreateVersion7();

        _movieServiceMock
            .Setup(service => service.DeleteMovieAsync(movieId, cancellationToken))
            .ReturnsAsync(true);

        IActionResult actionResult =
            await _controller.DeleteMovie(movieId, cancellationToken);

        Assert.IsType<NoContentResult>(actionResult);

        _movieServiceMock.Verify(
            service => service.DeleteMovieAsync(movieId, cancellationToken),
            Times.Once);

        _movieServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteMovie_WhenMovieDoesNotExist_ReturnsNotFound()
    {
        CancellationToken cancellationToken = CancellationToken.None;
        Guid movieId = Guid.CreateVersion7();

        _movieServiceMock
            .Setup(service => service.DeleteMovieAsync(movieId, cancellationToken))
            .ReturnsAsync(false);

        IActionResult actionResult =
            await _controller.DeleteMovie(movieId, cancellationToken);

        Assert.IsType<NotFoundResult>(actionResult);

        _movieServiceMock.Verify(
            service => service.DeleteMovieAsync(movieId, cancellationToken),
            Times.Once);

        _movieServiceMock.VerifyNoOtherCalls();
    }

}
