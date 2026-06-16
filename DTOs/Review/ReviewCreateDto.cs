using System.ComponentModel.DataAnnotations;
using MovieApi.Constants;

namespace MovieApi.DTOs.Review;

public class ReviewCreateDto
{
    [Required]
    [StringLength(ReviewValidationConstants.ReviewerNameMaxLength)]
    public string ReviewerName { get; set; } = string.Empty;

    [Required]
    [StringLength(ReviewValidationConstants.CommentMaxLength)]
    public string Comment { get; set; } = string.Empty;

    [Range(ReviewValidationConstants.MinimumRating, ReviewValidationConstants.MaximumRating)]
    public int Rating { get; set; }
}
