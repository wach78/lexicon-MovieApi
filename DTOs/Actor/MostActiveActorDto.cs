namespace MovieApi.DTOs.Reports;

public sealed class MostActiveActorDto
{
    public Guid ActorId { get; init; }

    public string Name { get; init; } = string.Empty;

    public int MovieCount { get; init; }
}
