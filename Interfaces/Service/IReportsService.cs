using MovieApi.DTOs.Actor;
using MovieApi.DTOs.Movie;
using MovieApi.DTOs.Report;
using MovieApi.DTOs.Reports;
namespace MovieApi.Interfaces.Service;

public interface IReportsService
{
    Task<IReadOnlyList<MovieAverageRatingDto>> GetAverageRatingsByGenreAsync(
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<TopMoviesPerGenreDto>> GetTopFiveMoviesPerGenreAsync(
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<MostActiveActorDto>> GetMostActiveActorsAsync(
        CancellationToken cancellationToken = default
    );

    Task<MovieWithMostReviewsDto?> GetMovieWithMostReviewsAsync(
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<PopularGenreDto>> GetPopularGenresAsync(
        CancellationToken cancellationToken = default
    );
}
