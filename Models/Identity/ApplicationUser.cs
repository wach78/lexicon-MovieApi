using Microsoft.AspNetCore.Identity;
using MovieApi.Enums;

namespace MovieApi.Models.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public UserStatus Status { get; set; } = UserStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
