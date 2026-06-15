namespace MovieApi.Models;

public class MovieDetails
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public string Synopsis { get; private set; } = string.Empty;
    public string Language { get; private set; } = string.Empty;
    public decimal Budget { get; private set; }
    public Guid MovieId { get; private set; }
    public Movie? Movie { get; private set; }


    public MovieDetails()
    {

    }

    public MovieDetails(string synopsis, string language, decimal budget)
    {
        Synopsis = synopsis;
        Language = language;
        Budget = budget;
    }
}
