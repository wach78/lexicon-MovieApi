using Microsoft.EntityFrameworkCore;
using MovieApi.DTOs.Actor;
using MovieApi.DTOs.Movie;
using MovieApi.DTOs.Review;
using MovieApi.Emuns;
using MovieApi.Interfaces.Data;
using MovieApi.Interfaces.Service;
using MovieApi.Models;

namespace MovieApi.Services;

public class MovieService : IMovieService
{
    private readonly IMovieApiContext _context;

    public MovieService(IMovieApiContext context)
    {
        _context = context;
    }

    public async Task<MovieDto?> CreateMovieAsync(MovieCreateDto movieCreateDto, CancellationToken cancellationToken)
    {
        Genre? genre = null;

        if (movieCreateDto.GenreId.HasValue)
        {
            genre = await _context.Set<Genre>()
                 .FirstOrDefaultAsync(
                    genre => genre.Id == movieCreateDto.GenreId.Value,
                    cancellationToken
                );

            if (genre is null)
            {
                return null;
            }
        }

        Movie movie = new(
            movieCreateDto.Title,
            movieCreateDto.Year,
            movieCreateDto.Duration,
            genre
        );

        _context.Movie.Add(movie);

        await _context.SaveChangesAsync(cancellationToken);

        return new MovieDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Year = movie.Year,
            Duration = movie.Duration,
            GenreId = movie.GenreId,
            GenreName = genre?.Name
        };
    }

    public async Task<bool> DeleteMovieAsync(Guid id, CancellationToken cancellationToken)
    {
        Movie? movie = await _context.Movie.FindAsync(
            new object[] { id },
            cancellationToken
        );

        if (movie is null)
        {
            return false;
        }

        _context.Movie.Remove(movie);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<MovieDto?> GetMovieByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Movie
           .AsNoTracking()
           .Where(movie => movie.Id == id)
           .Select(movie => new MovieDto
           {
               Id = movie.Id,
               Title = movie.Title,
               Year = movie.Year,
               Duration = movie.Duration,
               GenreId = movie.GenreId,
               GenreName = movie.Genre != null
                   ? movie.Genre.Name
                   : null
           })
           .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<MovieDetailDto?> GetMovieDetailsAsync(Guid id, CancellationToken cancellationToken)
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
                        Id = actor.Id,
                        Name = actor.Name,
                        BirthYear = actor.BirthYear
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return movie;
    }

    public async Task<IReadOnlyList<MovieDto>> GetMoviesAsync(string? genre, int? year, string? actor, CancellationToken cancellationToken)
    {
        IQueryable<Movie> query = _context.Movie.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(genre))
        {
            string trimmedGenre = genre.Trim();

            query = query.Where(movie =>
                movie.Genre != null &&
                movie.Genre.Name == trimmedGenre);
        }

        if (year.HasValue)
        {
            query = query.Where(movie => movie.Year == year.Value);
        }

        if (!string.IsNullOrWhiteSpace(actor))
        {
            string trimmedActor = actor.Trim();

            query = query.Where(movie =>
                movie.Actors.Any(movieActor =>
                    movieActor.Name == trimmedActor));
        }

        return await query
            .OrderBy(movie => movie.Title)
            .ThenBy(movie => movie.Id)
            .Select(movie => new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Year = movie.Year,
                Duration = movie.Duration,
                GenreId = movie.GenreId,
                GenreName = movie.Genre != null
                ? movie.Genre.Name
                : null
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<UpdateMovieResult> UpdateMovieAsync(Guid id, MovieUpdateDto movieUpdateDto, CancellationToken cancellationToken)
    {
        Movie? movie = await _context.Movie
            .FirstOrDefaultAsync(
                movie => movie.Id == id,
                cancellationToken
            );

        if (movie is null)
        {
            return UpdateMovieResult.MovieNotFound;
        }

        Genre? genre = null;

        if (movieUpdateDto.GenreId.HasValue)
        {
            genre = await _context.Set<Genre>()
                .FirstOrDefaultAsync(
                    genre => genre.Id == movieUpdateDto.GenreId.Value,
                    cancellationToken
                );

            if (genre is null)
            {
                return UpdateMovieResult.GenreNotFound;
            }
        }

        movie.Update(
            movieUpdateDto.Title,
            movieUpdateDto.Year,
            movieUpdateDto.Duration,
            genre
        );

        await _context.SaveChangesAsync(cancellationToken);

        return UpdateMovieResult.Updated;
    }
}
