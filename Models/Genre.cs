namespace MovieApi.Models;

public class Genre
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public string Name { get; private set; } = string.Empty;

    public ICollection<Movie> Movies { get; private set; } = new List<Movie>();

    private Genre()
    {
    }

    public Genre(string name)
    {
        Name = name;
    }
}
