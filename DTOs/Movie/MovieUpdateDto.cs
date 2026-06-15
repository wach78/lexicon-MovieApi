using System.ComponentModel.DataAnnotations;
using MovieApi.Constants;

namespace MovieApi.DTOs.Movie;

public class MovieUpdateDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [StringLength(MovieValidationConstants.TitleMaxLength)]
    public string Title { get; set; } = string.Empty;

    [Range(MovieValidationConstants.MinimumYear, MovieValidationConstants.MaximumYear)]
    public int Year { get; set; }

    [Range(MovieValidationConstants.MinimumDuration, MovieValidationConstants.MaximumDuration)]
    public int Duration { get; set; }

    public Guid? GenreId { get; set; }
}
