namespace MovieApi.DTOs.Actor;

public class ActorWithRoleDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public int BirthYear { get; init; }

    public string Role { get; init; } = string.Empty;
}
