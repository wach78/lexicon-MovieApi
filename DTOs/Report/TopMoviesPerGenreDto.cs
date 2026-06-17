namespace MovieApi.DTOs.Report;

public class TopMoviesPerGenreDto
{
    public Guid? GenreId { get; init; }

    public string GenreName { get; init; } = string.Empty;

    public List<TopRatedMovieDto> Movies { get; init; } = new();
}
