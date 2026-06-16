using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApi.DTOs.Actor;
using MovieApi.DTOs.Movie;
using MovieApi.Models;

namespace MovieApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActorsController : ControllerBase
{
    private readonly MovieApiContext _context;
    public ActorsController(MovieApiContext context)
    {
        _context = context;
    }

    // POST: api/movies/{movieId}/actors/{actorId}
    [HttpPost("/api/movies/{movieId:guid}/actors/{actorId:guid}")]
    public async Task<IActionResult> AddActorToMovie(
        [FromRoute] Guid movieId,
        [FromRoute] Guid actorId)
    {
        Movie? movie = await _context.Movie
            .Include(movie => movie.Actors)
            .FirstOrDefaultAsync(movie => movie.Id == movieId);

        if (movie == null)
        {
            return NotFound("Movie was not found.");
        }

        Actor? actor = await _context.Actors
            .FirstOrDefaultAsync(actor => actor.Id == actorId);

        if (actor == null)
        {
            return NotFound("Actor was not found.");
        }

        bool actorAlreadyAdded = movie.Actors.Any(existingActor => existingActor.Id == actorId);

        if (actorAlreadyAdded)
        {
            return BadRequest("Actor is already added to this movie.");
        }

        movie.Actors.Add(actor);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/Actor
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActorDto>>> GetActor()
    {
        List<ActorDto> actors = await _context.Actors
            .AsNoTracking()
            .Select(actor => new ActorDto
            {
                Id = actor.Id,
                Name = actor.Name,
                BirthYear = actor.BirthYear,
            })
            .ToListAsync();

        return Ok(actors);
    }

    // GET: api/Actor/5
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ActorDto>> GetActor([FromRoute] Guid id)
    {
        ActorDto? actor = await _context.Actors
            .AsNoTracking()
            .Where(actor => actor.Id == id)
            .Select(actor => new ActorDto
            {
                Id = actor.Id,
                Name = actor.Name,
                BirthYear = actor.BirthYear,
            })
            .FirstOrDefaultAsync();

        if (actor == null)
        {
            return NotFound();
        }

        return Ok(actor);
    }

    // POST: api/Actor
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<ActorDto>> PostActor([FromBody] ActorCreateDto actorCreateDto)
    {
        Actor actor = new(
            actorCreateDto.Name,
            actorCreateDto.BirthYear

        );

        _context.Actors.Add(actor);
        await _context.SaveChangesAsync();

        ActorDto actorDto = new()
        {
            Name = actorCreateDto.Name,
            BirthYear = actorCreateDto.BirthYear,
        };

        return CreatedAtAction(nameof(GetActor), new { id = actor.Id }, actorDto);
    }

    // PUT: api/Actor/5
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutMovie([FromRoute] Guid id, [FromBody] ActorUpdateDto actorUpdateDto)
    {
        if (id != actorUpdateDto.Id)
        {
            return BadRequest();
        }

        Actor? actor = await _context.Actors.FirstOrDefaultAsync(actor => actor.Id == id);

        if (actor == null)
        {
            return NotFound();
        }

        actor.Update(
            actorUpdateDto.Name,
            actorUpdateDto.BirthYear
        );

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
