using Microsoft.AspNetCore.Identity;

namespace MeiMaths.Models;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
}
