using MovieApi.DTOs.Review;
namespace MovieApi.Interfaces.Service;

public interface IReviewService
{
    Task<IReadOnlyList<ReviewDto>> GetReviewsAsync(
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<ReviewDto>> GetReviewsByMovieIdAsync(
        Guid movieId,
        CancellationToken cancellationToken = default
    );

    Task<bool> MovieExistsAsync(
        Guid movieId,
        CancellationToken cancellationToken = default
    );

    Task<ReviewDto> CreateReviewAsync(
        Guid movieId,
        ReviewCreateDto reviewCreateDto,
        CancellationToken cancellationToken = default
    );

    Task<bool> DeleteReviewAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );
}
