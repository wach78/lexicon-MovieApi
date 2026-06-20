using System.ComponentModel.DataAnnotations;
using MovieApi.Constants;

namespace MovieApi.DTOs.Movie;

public class MovieDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public int Year { get; init; }

    public int Duration { get; init; }

    public Guid? GenreId { get; init; }

    public string? GenreName { get; init; }
}
