namespace MovieApi.DTOs.Actor;

public class ActorDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int BirthYear { get; init; }
}
