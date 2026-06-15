namespace MovieApi.Models;

public class Actor
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public string Name { get; private set; } = string.Empty;
    public int BirthYear { get; private set; }

    public ICollection<Movie> Movies { get; private set; } = new List<Movie>();

    public Actor()
    {

    }

    public Actor(string name, int birthYear)
    {
        Name = name;
        BirthYear = birthYear;
    }
}
