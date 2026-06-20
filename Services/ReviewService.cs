using Microsoft.EntityFrameworkCore;
using MovieApi.DTOs.Review;
using MovieApi.Interfaces.Data;
using MovieApi.Interfaces.Service;
using MovieApi.Models;

namespace MovieApi.Services;

public class ReviewService : IReviewService
{
    private readonly IMovieApiContext _context;

    public ReviewService(IMovieApiContext context)
    {
        _context = context;
    }
    public Task<ReviewDto> CreateReviewAsync(Guid movieId, ReviewCreateDto reviewCreateDto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteReviewAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<ReviewDto>> GetReviewsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<ReviewDto>?> GetReviewsByMovieIdAsync(Guid movieId, CancellationToken cancellationToken = default)
    {
        Movie? movie = await _context.Movie
            .AsNoTracking()
            .FirstOrDefaultAsync(movie => movie.Id == movieId, cancellationToken);

        if (movie is null)
        {
            return null;
        }

        return await _context.Reviews
            .AsNoTracking()
            .Where(review => review.MovieId == movieId)
            .Select(review => new ReviewDto
            {
                Id = review.Id,
                ReviewerName = review.ReviewerName,
                Comment = review.Comment,
                Rating = review.Rating
            })
            .ToListAsync(cancellationToken);

    }

    public Task<bool> MovieExistsAsync(Guid movieId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

/*
 *  Movie? movie = await _context.Movie
        .AsNoTracking()
        .FirstOrDefaultAsync(movie => movie.Id == movieId);

        if (movie == null)
        {
            return NotFound();
        }

        List<ReviewDto> reviews = await _context.Reviews
            .AsNoTracking()
            .Where(review => review.MovieId == movieId)
            .Select(review => new ReviewDto
            {
                Id = review.Id,
                ReviewerName = review.ReviewerName,
                Comment = review.Comment,
                Rating = review.Rating
            })
            .ToListAsync();

        return Ok(reviews);
 */
