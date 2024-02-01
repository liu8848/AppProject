using Microsoft.AspNetCore.Identity;

namespace AppProject.Model.Entities.Identities;

public class ApplicationUser:IdentityUser<long>
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}