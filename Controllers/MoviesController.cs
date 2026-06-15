using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi.DTOs.Actor;
using MovieApi.DTOs.Movie;
using MovieApi.DTOs.Review;
using MovieApi.Models;

[Route("api/[controller]")]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly MovieApiContext _context;
    public MoviesController(MovieApiContext context)
    {
        _context = context;
    }

    // GET: api/Movie
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovie()
    {
        List<MovieDto> movies = await _context.Movie
            .AsNoTracking()
            .Select(movie => new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Year = movie.Year,
                Duration = movie.Duration,
                GenreId = movie.GenreId,
                GenreName = movie.Genre != null ? movie.Genre.Name : null
            })
            .ToListAsync();

        return Ok(movies);
    }

    // GET: api/Movie/5
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MovieDto>> GetMovie([FromRoute] Guid id)
    {
        MovieDto? movie = await _context.Movie
            .AsNoTracking()
            .Where(movie => movie.Id == id)
            .Select(movie => new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Year = movie.Year,
                Duration = movie.Duration,
                GenreId = movie.GenreId,
                GenreName = movie.Genre != null ? movie.Genre.Name : null
            })
            .FirstOrDefaultAsync();

        if (movie == null)
        {
            return NotFound();
        }

        return Ok(movie);
    }

    [HttpGet("{id:guid}/details")]
    public async Task<ActionResult<MovieDetailDto>> GetMovieDetails([FromRoute] Guid id)
    {
        MovieDetailDto? movie = await _context.Movie
            .AsNoTracking()
            .Where(movie => movie.Id == id)
            .Select(movie => new MovieDetailDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Year = movie.Year,
                Duration = movie.Duration,
                GenreId = movie.GenreId,
                GenreName = movie.Genre != null ? movie.Genre.Name : null,

                MovieDetails = movie.MovieDetails == null
                    ? null
                    : new MovieDetailsDto
                    {
                        Id = movie.MovieDetails.Id,
                        Synopsis = movie.MovieDetails.Synopsis,
                        Language = movie.MovieDetails.Language,
                        Budget = movie.MovieDetails.Budget
                    },

                Reviews = movie.Reviews
                    .Select(review => new ReviewDto
                    {
                        Id = review.Id,
                        ReviewerName = review.ReviewerName,
                        Comment = review.Comment,
                        Rating = review.Rating
                    })
                    .ToList(),

                Actors = movie.Actors
                    .Select(actor => new ActorDto
                    {
                        Name = actor.Name,
                        BirthYear = actor.BirthYear
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (movie == null)
        {
            return NotFound();
        }

        return Ok(movie);
    }

    // PUT: api/Movie/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutMovie([FromRoute] Guid id, [FromBody] MovieUpdateDto movieUpdateDto)
    {
        if (id != movieUpdateDto.Id)
        {
            return BadRequest();
        }

        Movie? movie = await _context.Movie.FirstOrDefaultAsync(movie => movie.Id == id);

        if (movie == null)
        {
            return NotFound();
        }

        Genre? genre = null;

        if (movieUpdateDto.GenreId.HasValue)
        {
            genre = await _context.Set<Genre>()
                .FindAsync(movieUpdateDto.GenreId.Value);

            if (genre == null)
            {
                return BadRequest("Genre does not exist.");
            }
        }

        movie.Update(
            movieUpdateDto.Title,
            movieUpdateDto.Year,
            movieUpdateDto.Duration,
            genre
        );

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Movie
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<MovieDto>> PostMovie([FromBody] MovieCreateDto movieCreateDto)
    {
        Genre? genre = null;

        if (movieCreateDto.GenreId.HasValue)
        {
            genre = await _context.Set<Genre>().FindAsync(movieCreateDto.GenreId.Value);

            if (genre == null)
            {
                return BadRequest("Genre does not exist.");
            }
        }

        Movie movie = new(
            movieCreateDto.Title,
            movieCreateDto.Year,
            movieCreateDto.Duration,
            genre
        );

        _context.Movie.Add(movie);
        await _context.SaveChangesAsync();

        MovieDto movieDto = new()
        {
            Id = movie.Id,
            Title = movie.Title,
            Year = movie.Year,
            Duration = movie.Duration,
            GenreId = movie.GenreId,
            GenreName = movie.Genre?.Name
        };

        return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movieDto);
    }

    // DELETE: api/Movie/5
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteMovie([FromRoute] System.Guid id)
    {
        var movie = await _context.Movie.FindAsync(id);
        if (movie == null)
        {
            return NotFound();
        }

        _context.Movie.Remove(movie);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool MovieExists(System.Guid? id)
    {
        return _context.Movie.Any(e => e.Id == id);
    }
}

