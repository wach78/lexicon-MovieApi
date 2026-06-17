namespace MovieApi.DTOs.Report;

public class TopRatedMovieDto
{
    public Guid MovieId { get; init; }

    public string Title { get; init; } = string.Empty;

    public int Year { get; init; }

    public double AverageRating { get; init; }

    public int ReviewCount { get; init; }
}
