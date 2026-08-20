using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieApi.Interfaces.Data;
using MovieApi.Models;
using MovieApi.Models.Identity;

public class MovieApiContext(DbContextOptions<MovieApiContext> options) : IdentityDbContext<
        ApplicationUser,
        IdentityRole<Guid>,
        Guid
    >(options),
    IMovieApiContext
{
    public DbSet<Movie> Movie { get; set; } = default!;
    public DbSet<Actor> Actors { get; set; } = default!;
    public DbSet<Review> Reviews { get; set; } = default!;

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MovieDetails>()
            .Property(movieDetails => movieDetails.Budget)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ApplicationUser>()
            .ToTable("Users");

        modelBuilder.Entity<IdentityRole<Guid>>()
            .ToTable("Roles");

        modelBuilder.Entity<IdentityUserRole<Guid>>()
            .ToTable("UserRoles");

        modelBuilder.Entity<IdentityUserClaim<Guid>>()
            .ToTable("UserClaims");

        modelBuilder.Entity<IdentityUserLogin<Guid>>()
            .ToTable("UserLogins");

        modelBuilder.Entity<IdentityRoleClaim<Guid>>()
            .ToTable("RoleClaims");

        modelBuilder.Entity<IdentityUserToken<Guid>>()
            .ToTable("UserTokens");

        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(user => user.NormalizedEmail)
            .IsUnique();
    }

}
