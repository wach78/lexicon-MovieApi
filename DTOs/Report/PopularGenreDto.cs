namespace MovieApi.DTOs.Report;

public class PopularGenreDto
{
    public Guid? GenreId { get; init; }

    public string GenreName { get; init; } = string.Empty;

    public int MovieCount { get; init; }
}
