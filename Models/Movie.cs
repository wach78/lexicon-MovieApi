namespace MovieApi.Models;

public class Movie
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public string Title { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public int Duration { get; private set; }

    public Guid? GenreId { get; private set; }
    public Genre? Genre { get; private set; }
    public MovieDetails? MovieDetails { get; private set; }
    public ICollection<Review> Reviews { get; private set; } = new List<Review>();
    public ICollection<Actor> Actors { get; private set; } = new List<Actor>();

    public Movie()
    {

    }

    public Movie(string title, int year, int duration, Genre? genre)
    {
        Title = title;
        Year = year;
        Duration = duration;
        Genre = genre;
    }

    public void Update(string title, int year, int duration, Genre? genre)
    {
        Title = title;
        Year = year;
        Duration = duration;
        Genre = genre;
    }
}
