using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi.DTOs.Actor;
using MovieApi.DTOs.Review;
using MovieApi.Interfaces.Service;
using MovieApi.Models;

namespace MovieApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly MovieApiContext _context;
    private readonly IReviewService _reviewService;
    public ReviewsController(MovieApiContext context, IReviewService reviewService)
    {
        _context = context;
        _reviewService = reviewService;
    }

    // GET: api/Reviws
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReview()
    {
        List<ReviewDto> reviews = await _context.Reviews
            .AsNoTracking()
            .Select(review => new ReviewDto
            {
                Id = review.Id,
                ReviewerName = review.ReviewerName,
                Comment = review.Comment,
                Rating = review.Rating
            })
            .ToListAsync();

        return Ok(reviews);
    }

    //GET /api/movies/{movieId}/reviews
    [HttpGet("/api/movies/{movieId:guid}/reviews")]
    public async Task<ActionResult<IReadOnlyList<ReviewDto>>> GetMovieReviews([FromRoute] Guid movieId, CancellationToken cancellationToken)
    {
        IReadOnlyList<ReviewDto>? reviews = await _reviewService.GetReviewsByMovieIdAsync(movieId, cancellationToken);

        if (reviews is null)
        {
            return NotFound();
        }

        return Ok(reviews);
    }

    //POST /api/movies/{movieId}/reviews
    [HttpPost("/api/movies/{movieId:guid}/reviews")]
    public async Task<ActionResult<ReviewDto>> PostReview([FromRoute] Guid movieId, [FromBody] ReviewCreateDto reviewCreateDto)
    {
        Movie? movie = await _context.Movie
            .Include(movie => movie.Reviews)
            .FirstOrDefaultAsync(movie => movie.Id == movieId);

        if (movie == null)
        {
            return NotFound();
        }

        var review = new Review(
            reviewCreateDto.ReviewerName,
            reviewCreateDto.Comment,
            reviewCreateDto.Rating
        );

        movie.Reviews.Add(review);
        _context.Set<Review>().Add(review);
        await _context.SaveChangesAsync();

        ReviewDto reviewDto = new()
        {
            Id = review.Id,
            ReviewerName = review.ReviewerName,
            Comment = review.Comment,
            Rating = review.Rating
        };

        return Created($"/api/reviews/{review.Id}", reviewDto);
    }

    // DELETE: api/Reviews/5
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteMovie([FromRoute] System.Guid id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
