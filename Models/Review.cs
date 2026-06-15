namespace MovieApi.Models;

public class Review
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public string ReviewerName { get; private set; } = string.Empty;
    public string Comment { get; private set; } = string.Empty;
    public int Rating { get; private set; }

    public Guid MovieId { get; private set; }
    public Movie? Movie { get; private set; }

    public Review()
    {

    }

    public Review(string reviewerName, string comment, int rating)
    {
        ReviewerName = reviewerName;
        Comment = comment;
        Rating = rating;
    }

}
