using Microsoft.EntityFrameworkCore;
using MovieApi.Models;

namespace MovieApi.Data.Seed;

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
        Genre action = new("Action");
        Genre crime = new("Crime");
        Genre fantasy = new("Fantasy");
        Genre animation = new("Animation");

        Actor keanuReeves = new("Keanu Reeves", 1964);
        Actor carrieAnneMoss = new("Carrie-Anne Moss", 1967);
        Actor tomHanks = new("Tom Hanks", 1956);
        Actor morganFreeman = new("Morgan Freeman", 1937);
        Actor timRobbins = new("Tim Robbins", 1958);
        Actor leonardoDiCaprio = new("Leonardo DiCaprio", 1974);
        Actor josephGordonLevitt = new("Joseph Gordon-Levitt", 1981);
        Actor matthewMcConaughey = new("Matthew McConaughey", 1969);
        Actor anneHathaway = new("Anne Hathaway", 1982);
        Actor marlonBrando = new("Marlon Brando", 1924);
        Actor alPacino = new("Al Pacino", 1940);
        Actor christianBale = new("Christian Bale", 1974);
        Actor heathLedger = new("Heath Ledger", 1979);
        Actor russellCrowe = new("Russell Crowe", 1964);
        Actor joaquinPhoenix = new("Joaquin Phoenix", 1974);
        Actor songKangHo = new("Song Kang-ho", 1967);
        Actor choYeoJeong = new("Cho Yeo-jeong", 1981);
        Actor rumiHiiragi = new("Rumi Hiiragi", 1987);
        Actor miyuIrino = new("Miyu Irino", 1988);

        Movie matrix = new("The Matrix", 1999, 136, scienceFiction);
        Movie forrestGump = new("Forrest Gump", 1994, 142, drama);
        Movie shawshank = new("The Shawshank Redemption", 1994, 142, drama);
        Movie inception = new("Inception", 2010, 148, scienceFiction);
        Movie interstellar = new("Interstellar", 2014, 169, scienceFiction);
        Movie godfather = new("The Godfather", 1972, 175, crime);
        Movie darkKnight = new("The Dark Knight", 2008, 152, action);
        Movie gladiator = new("Gladiator", 2000, 155, action);
        Movie parasite = new("Parasite", 2019, 132, drama);
        Movie spiritedAway = new("Spirited Away", 2001, 125, animation);

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

        MovieDetails inceptionDetails = new(
            "A thief who steals information through dream-sharing technology is given a difficult mission.",
            "English",
            160000000m
        );

        MovieDetails interstellarDetails = new(
            "A group of explorers travel through a wormhole in space to find a new home for humanity.",
            "English",
            165000000m
        );

        MovieDetails godfatherDetails = new(
            "The aging patriarch of a crime family transfers control of his empire to his reluctant son.",
            "English",
            6000000m
        );

        MovieDetails darkKnightDetails = new(
            "Batman faces a dangerous criminal mastermind who wants to create chaos in Gotham City.",
            "English",
            185000000m
        );

        MovieDetails gladiatorDetails = new(
            "A former Roman general seeks revenge after being betrayed and forced into slavery.",
            "English",
            103000000m
        );

        MovieDetails parasiteDetails = new(
            "A poor family becomes involved with a wealthy household, leading to unexpected consequences.",
            "Korean",
            11400000m
        );

        MovieDetails spiritedAwayDetails = new(
            "A young girl enters a mysterious spirit world and must find a way to save her parents.",
            "Japanese",
            19000000m
        );

        matrix.Actors.Add(keanuReeves);
        matrix.Actors.Add(carrieAnneMoss);

        forrestGump.Actors.Add(tomHanks);

        shawshank.Actors.Add(morganFreeman);
        shawshank.Actors.Add(timRobbins);

        inception.Actors.Add(leonardoDiCaprio);
        inception.Actors.Add(josephGordonLevitt);

        interstellar.Actors.Add(matthewMcConaughey);
        interstellar.Actors.Add(anneHathaway);

        godfather.Actors.Add(marlonBrando);
        godfather.Actors.Add(alPacino);

        darkKnight.Actors.Add(christianBale);
        darkKnight.Actors.Add(heathLedger);

        gladiator.Actors.Add(russellCrowe);
        gladiator.Actors.Add(joaquinPhoenix);

        parasite.Actors.Add(songKangHo);
        parasite.Actors.Add(choYeoJeong);

        spiritedAway.Actors.Add(rumiHiiragi);
        spiritedAway.Actors.Add(miyuIrino);

        matrix.Reviews.Add(new Review("Anna", "Very good science fiction movie.", 5));
        matrix.Reviews.Add(new Review("Erik", "Still holds up well.", 5));

        forrestGump.Reviews.Add(new Review("Sara", "Emotional and memorable.", 4));
        forrestGump.Reviews.Add(new Review("Johan", "Good acting and story.", 4));

        shawshank.Reviews.Add(new Review("Maria", "Excellent movie.", 5));
        shawshank.Reviews.Add(new Review("Oskar", "One of the best prison dramas.", 5));

        inception.Reviews.Add(new Review("Lina", "Smart and visually impressive.", 5));
        inception.Reviews.Add(new Review("Adam", "Good concept, but a bit complex.", 4));
        inception.Reviews.Add(new Review("Nora", "Great action and story.", 5));

        interstellar.Reviews.Add(new Review("Viktor", "Emotional and visually powerful.", 5));
        interstellar.Reviews.Add(new Review("Emma", "Long but very good.", 4));
        interstellar.Reviews.Add(new Review("Daniel", "Excellent soundtrack and atmosphere.", 5));

        godfather.Reviews.Add(new Review("Peter", "Classic crime drama.", 5));
        godfather.Reviews.Add(new Review("Karin", "Slow but extremely well made.", 4));

        darkKnight.Reviews.Add(new Review("Mikael", "Great superhero movie.", 5));
        darkKnight.Reviews.Add(new Review("Ida", "Very strong villain performance.", 5));
        darkKnight.Reviews.Add(new Review("Alex", "Dark, intense and entertaining.", 4));

        gladiator.Reviews.Add(new Review("Fredrik", "Strong action and drama.", 4));
        gladiator.Reviews.Add(new Review("Sofia", "Memorable story and characters.", 5));

        parasite.Reviews.Add(new Review("Hanna", "Original and unpredictable.", 5));
        parasite.Reviews.Add(new Review("Oliver", "Very well written.", 5));
        parasite.Reviews.Add(new Review("Maja", "Interesting social drama.", 4));

        spiritedAway.Reviews.Add(new Review("Elin", "Beautiful animation.", 5));
        spiritedAway.Reviews.Add(new Review("Noah", "Creative and magical.", 5));
        spiritedAway.Reviews.Add(new Review("Lisa", "A bit strange but very good.", 4));

        context.Set<Genre>().AddRange(
            scienceFiction,
            drama,
            action,
            crime,
            fantasy,
            animation
        );

        context.Set<Movie>().AddRange(
            matrix,
            forrestGump,
            shawshank,
            inception,
            interstellar,
            godfather,
            darkKnight,
            gladiator,
            parasite,
            spiritedAway
        );

        context.Set<MovieDetails>().AddRange(
            matrixDetails,
            forrestGumpDetails,
            shawshankDetails,
            inceptionDetails,
            interstellarDetails,
            godfatherDetails,
            darkKnightDetails,
            gladiatorDetails,
            parasiteDetails,
            spiritedAwayDetails
        );

        context.Entry(matrix).Reference(movie => movie.MovieDetails).CurrentValue = matrixDetails;
        context.Entry(forrestGump).Reference(movie => movie.MovieDetails).CurrentValue = forrestGumpDetails;
        context.Entry(shawshank).Reference(movie => movie.MovieDetails).CurrentValue = shawshankDetails;
        context.Entry(inception).Reference(movie => movie.MovieDetails).CurrentValue = inceptionDetails;
        context.Entry(interstellar).Reference(movie => movie.MovieDetails).CurrentValue = interstellarDetails;
        context.Entry(godfather).Reference(movie => movie.MovieDetails).CurrentValue = godfatherDetails;
        context.Entry(darkKnight).Reference(movie => movie.MovieDetails).CurrentValue = darkKnightDetails;
        context.Entry(gladiator).Reference(movie => movie.MovieDetails).CurrentValue = gladiatorDetails;
        context.Entry(parasite).Reference(movie => movie.MovieDetails).CurrentValue = parasiteDetails;
        context.Entry(spiritedAway).Reference(movie => movie.MovieDetails).CurrentValue = spiritedAwayDetails;

        context.SaveChanges();
    }
}
