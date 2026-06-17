namespace MovieApi.DTOs.Report;

public class MovieAverageRatingDto
{
    public Guid? GenreId { get; init; }

    public string GenreName { get; init; } = string.Empty;

    public double AverageRating { get; init; }

    public int ReviewCount { get; init; }
}
