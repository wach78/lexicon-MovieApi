using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi.DTOs.Actor;
using MovieApi.DTOs.Movie;
using MovieApi.DTOs.Report;
using MovieApi.DTOs.Reports;
using MovieApi.Models;

namespace MovieApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportsController : ControllerBase
{
    private readonly MovieApiContext _context;
    public ReportsController(MovieApiContext context)
    {
        _context = context;
    }

    //GET /api/reports/movies/average-ratings

    [HttpGet("movies/average-ratings")]
    public async Task<ActionResult<MovieAverageRatingDto>> GetAverageRatingGenre()
    {
        List<MovieAverageRatingDto> averageRatings = await _context.Movie
           .AsNoTracking()
           .Where(movie => movie.Reviews.Any())
           .GroupBy(movie => new
           {
               movie.GenreId,
               GenreName = movie.Genre != null ? movie.Genre.Name : "No genre"
           })
           .Select(group => new MovieAverageRatingDto
           {
               GenreId = group.Key.GenreId,
               GenreName = group.Key.GenreName,
               AverageRating = Math.Round(
                group
                    .SelectMany(movie => movie.Reviews)
                    .Average(review => review.Rating),
                2),

               ReviewCount = group
                   .SelectMany(movie => movie.Reviews)
                   .Count()
           })
           .ToListAsync();

        return Ok(averageRatings);
    }

    //GET /api/reports/movies/top5pergenre
    [HttpGet("movies/top5pergenre")]
    public async Task<ActionResult<IEnumerable<TopMoviesPerGenreDto>>> GetTop5MoviesPerGenre()
    {
        var movieRatings = await _context.Movie
            .AsNoTracking()
            .Where(movie => movie.Reviews.Any())
            .Select(movie => new
            {
                MovieId = movie.Id,
                movie.Title,
                movie.Year,
                movie.GenreId,
                GenreName = movie.Genre != null ? movie.Genre.Name : "No genre",
                AverageRating = movie.Reviews.Average(review => review.Rating),
                ReviewCount = movie.Reviews.Count()
            })
            .ToListAsync();

        List<TopMoviesPerGenreDto> top5PerGenre = movieRatings
            .GroupBy(movie => new
            {
                movie.GenreId,
                movie.GenreName
            })
            .Select(group => new TopMoviesPerGenreDto
            {
                GenreId = group.Key.GenreId,
                GenreName = group.Key.GenreName,
                Movies = group
                    .OrderByDescending(movie => movie.AverageRating)
                    .ThenByDescending(movie => movie.ReviewCount)
                    .Take(5)
                    .Select(movie => new TopRatedMovieDto
                    {
                        MovieId = movie.MovieId,
                        Title = movie.Title,
                        Year = movie.Year,
                        AverageRating = Math.Round(movie.AverageRating, 2),
                        ReviewCount = movie.ReviewCount
                    })
                    .ToList()
            })
            .OrderBy(result => result.GenreName)
            .ToList();

        return Ok(top5PerGenre);
    }

    //GET /api/reports/actors/most-active
    [HttpGet("actors/most-active")]
    public async Task<ActionResult<IReadOnlyList<MostActiveActorDto>>> GetMostActiveActors()
    {
        List<MostActiveActorDto> actors = await _context.Actors
         .AsNoTracking()
         .Where(actor => actor.Movies.Any())
         .Select(actor => new MostActiveActorDto
         {
             ActorId = actor.Id,
             Name = actor.Name,
             MovieCount = actor.Movies.Count
         })
         .OrderByDescending(actor => actor.MovieCount)
         .ThenBy(actor => actor.Name)
         .ToListAsync();

        return Ok(actors);
    }

    //GET /api/reports/movies/with-most-reviews
    [HttpGet("movies/with-most-reviews")]
    public async Task<ActionResult<MovieWithMostReviewsDto>> GetMovieWithMostReviews()
    {
        MovieWithMostReviewsDto? movie = await _context.Movie
            .AsNoTracking()
            .Where(movie => movie.Reviews.Any())
            .Select(movie => new MovieWithMostReviewsDto
            {
                MovieId = movie.Id,
                Title = movie.Title,
                ReviewCount = movie.Reviews.Count
            })
            .OrderByDescending(movie => movie.ReviewCount)
            .ThenBy(movie => movie.Title)
            .FirstOrDefaultAsync();

        if (movie is null)
        {
            return NotFound();
        }

        return Ok(movie);
    }

    //GET /api/reports/genres/popular
    [HttpGet("/reports/genres/popular")]
    public async Task<ActionResult<IReadOnlyList<PopularGenreDto>>> GetPopularGenre()
    {
        List<PopularGenreDto> popularGenres = await _context.Movie
         .AsNoTracking()
         .Where(movie => movie.GenreId != null)
         .GroupBy(movie => new
         {
             movie.GenreId,
             GenreName = movie.Genre != null
                 ? movie.Genre.Name
                 : "Unknown"
         })
         .Select(group => new PopularGenreDto
         {
             GenreId = group.Key.GenreId,
             GenreName = group.Key.GenreName,
             MovieCount = group.Count()
         })
         .OrderByDescending(genre => genre.MovieCount)
         .ThenBy(genre => genre.GenreName)
         .ToListAsync();

        return Ok(popularGenres);
    }
}
