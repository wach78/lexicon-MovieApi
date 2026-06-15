namespace MovieApi.DTOs.Movie;

public class MovieDetailsDto
{
    public Guid Id { get; init; }

    public string Synopsis { get; init; } = string.Empty;

    public string Language { get; init; } = string.Empty;

    public decimal Budget { get; init; }
}
