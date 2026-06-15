namespace MovieApi.DTOs.Review;

public class ReviewDto
{
    public Guid Id { get; init; }
    public string ReviewerName { get; init; } = string.Empty;
    public string Comment { get; init; } = string.Empty;
    public int Rating { get; init; }
}
