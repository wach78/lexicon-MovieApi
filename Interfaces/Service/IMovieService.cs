using MovieApi.DTOs.Movie;
using MovieApi.Emuns;
namespace MovieApi.Interfaces.Service;

public interface IMovieService
{
    Task<IReadOnlyList<MovieDto>> GetMoviesAsync(
        string? genre,
        int? year,
        string? actor,
        CancellationToken cancellationToken = default
    );

    Task<MovieDto?> GetMovieByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<MovieDetailDto?> GetMovieDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<MovieDto?> CreateMovieAsync(
        MovieCreateDto movieCreateDto,
        CancellationToken cancellationToken = default
    );

    Task<UpdateMovieResult> UpdateMovieAsync(
        Guid id,
        MovieUpdateDto movieUpdateDto,
        CancellationToken cancellationToken = default
    );

    Task<bool> DeleteMovieAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );
}

