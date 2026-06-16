using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi.DTOs.Actor;
using MovieApi.DTOs.Review;
using MovieApi.Models;

namespace MovieApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly MovieApiContext _context;
    public ReviewsController(MovieApiContext context)
    {
        _context = context;
    }

    // GET: api/Reviws
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReview()
    {
        List<ReviewDto> reviews = await _context.reviews
            .AsNoTracking()
            .Select(review => new ReviewDto
            {
                ReviewerName = review.ReviewerName,
                Comment = review.Comment,
                Rating = review.Rating
            })
            .ToListAsync();

        return Ok(reviews);
    }

    //GET /api/movies/{movieId}/reviews
    [HttpGet("/api/movies/{movieId:guid}/reviews")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetMovieReviws([FromRoute] Guid movieId)
    {
        List<ReviewDto> reviews = await _context.reviews
            .AsNoTracking()
            .Where(review => review.MovieId == movieId)
            .Select(review => new ReviewDto
            {
                ReviewerName = review.ReviewerName,
                Comment = review.Comment,
                Rating = review.Rating
            })
            .ToListAsync();

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
        var review = await _context.reviews.FindAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        _context.reviews.Remove(review);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
