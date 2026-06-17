using Microsoft.EntityFrameworkCore;
using MovieApi.Models;

namespace MovieApi.Interfaces.Data;

public interface IMovieApiContext
{
    DbSet<Models.Movie> Movie { get; set; }
    DbSet<Models.Actor> Actors { get; set; }
    DbSet<Models.Review> Reviews { get; set; }
    //DbSet<Genre> Genres { get; set; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
