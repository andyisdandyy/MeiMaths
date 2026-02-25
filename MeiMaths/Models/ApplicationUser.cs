using Microsoft.AspNetCore.Identity;

namespace MeiMaths.Models;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>UTC-tidspunkt for hvornår brugeren accepterede privatlivspolitikken.</summary>
    public DateTime? PrivacyPolicyAcceptedAt { get; set; }
}
