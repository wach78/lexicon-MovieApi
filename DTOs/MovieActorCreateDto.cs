using System.ComponentModel.DataAnnotations;

public class MovieActorCreateDto
{
    [Required]
    [StringLength(100)]
    public string Role { get; set; } = string.Empty;
}
