using Microsoft.EntityFrameworkCore;
using MovieApi.Models;

namespace MovieApi.Extensions;

public static class SeedDataExtensions
{
    public static void SeedData(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        MovieApiContext context = scope.ServiceProvider.GetRequiredService<MovieApiContext>();

        context.Database.Migrate();

        if (context.Set<Movie>().Any())
        {
            return;
        }

        Genre scienceFiction = new("Science Fiction");
        Genre drama = new("Drama");

        Actor keanuReeves = new("Keanu Reeves", 1964);
        Actor carrieAnneMoss = new("Carrie-Anne Moss", 1967);
        Actor tomHanks = new("Tom Hanks", 1956);
        Actor morganFreeman = new("Morgan Freeman", 1937);
        Actor timRobbins = new("Tim Robbins", 1958);

        Movie matrix = new("The Matrix", 1999, 136, scienceFiction);
        Movie forrestGump = new("Forrest Gump", 1994, 142,drama);
        Movie shawshank = new("The Shawshank Redemption", 1994, 142,drama);

        MovieDetails matrixDetails = new(
            "A hacker discovers that reality is a simulated world controlled by machines.",
            "English",
            63000000m
        );

        MovieDetails forrestGumpDetails = new(
            "A man with a kind heart experiences several major events in American history.",
            "English",
            55000000m
        );

        MovieDetails shawshankDetails = new(
            "Two imprisoned men form a friendship while trying to survive life in prison.",
            "English",
            25000000m
        );

        matrix.Actors.Add(keanuReeves);
        matrix.Actors.Add(carrieAnneMoss);

        forrestGump.Actors.Add(tomHanks);

        shawshank.Actors.Add(morganFreeman);
        shawshank.Actors.Add(timRobbins);

        matrix.Reviews.Add(new Review("Anna", "Very good science fiction movie.", 5));
        matrix.Reviews.Add(new Review("Erik", "Still holds up well.", 5));

        forrestGump.Reviews.Add(new Review("Sara", "Emotional and memorable.", 4));
        forrestGump.Reviews.Add(new Review("Johan", "Good acting and story.", 4));

        shawshank.Reviews.Add(new Review("Maria", "Excellent movie.", 5));
        shawshank.Reviews.Add(new Review("Oskar", "One of the best prison dramas.", 5));

        context.Set<Genre>().AddRange(scienceFiction, drama);

        context.Set<Movie>().AddRange(
            matrix,
            forrestGump,
            shawshank
        );

        context.Set<MovieDetails>().AddRange(
            matrixDetails,
            forrestGumpDetails,
            shawshankDetails
        );

        context.Entry(matrix).Reference(movie => movie.Genre).CurrentValue = scienceFiction;
        context.Entry(forrestGump).Reference(movie => movie.Genre).CurrentValue = drama;
        context.Entry(shawshank).Reference(movie => movie.Genre).CurrentValue = drama;

        context.Entry(matrix).Reference(movie => movie.MovieDetails).CurrentValue = matrixDetails;
        context.Entry(forrestGump).Reference(movie => movie.MovieDetails).CurrentValue = forrestGumpDetails;
        context.Entry(shawshank).Reference(movie => movie.MovieDetails).CurrentValue = shawshankDetails;

        context.SaveChanges();
    }
}
