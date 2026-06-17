using MovieApi.DTOs.Actor;
namespace MovieApi.Interfaces.Service;

public interface IActorService
{
    Task<IReadOnlyList<ActorDto>> GetActorsAsync(
        CancellationToken cancellationToken = default
    );

    Task<ActorDto?> GetActorByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<ActorDto> CreateActorAsync(
        ActorCreateDto actorCreateDto,
        CancellationToken cancellationToken = default
    );

    Task<bool> UpdateActorAsync(
        Guid id,
        ActorUpdateDto actorUpdateDto,
        CancellationToken cancellationToken = default
    );

    Task<bool> MovieExistsAsync(
        Guid movieId,
        CancellationToken cancellationToken = default
    );

    Task<bool> ActorExistsAsync(
        Guid actorId,
        CancellationToken cancellationToken = default
    );

    Task<bool> IsActorAddedToMovieAsync(
        Guid movieId,
        Guid actorId,
        CancellationToken cancellationToken = default
    );

    Task AddActorToMovieAsync(
        Guid movieId,
        Guid actorId,
        CancellationToken cancellationToken = default
    );
}
