using Microsoft.EntityFrameworkCore;
using MovieApi.Interfaces.Data;

public class MovieApiContext(DbContextOptions<MovieApiContext> options) : DbContext(options), IMovieApiContext
{
    public DbSet<MovieApi.Models.Movie> Movie { get; set; } = default!;
    public DbSet<MovieApi.Models.Actor> Actors { get; set; } = default!;
    public DbSet<MovieApi.Models.Review> Reviews { get; set; } = default!;
}
