using Microsoft.EntityFrameworkCore;
using MovieApi.Interfaces.Data;
using MovieApi.Models;

public class MovieApiContext(DbContextOptions<MovieApiContext> options) : DbContext(options), IMovieApiContext
{
    public DbSet<MovieApi.Models.Movie> Movie { get; set; } = default!;
    public DbSet<MovieApi.Models.Actor> Actors { get; set; } = default!;
    public DbSet<MovieApi.Models.Review> Reviews { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MovieDetails>()
            .Property(movieDetails => movieDetails.Budget)
            .HasPrecision(18, 2);

        base.OnModelCreating(modelBuilder);
    }
}
