using MovieApi.DTOs.Actor;

namespace MovieApi.DTOs.Movie;

public class MovieWithActorsDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public int Year { get; init; }

    public int Duration { get; init; }

    public Guid? GenreId { get; init; }

    public string? GenreName { get; init; }

    public List<ActorDto> Actors { get; init; } = new();
}
