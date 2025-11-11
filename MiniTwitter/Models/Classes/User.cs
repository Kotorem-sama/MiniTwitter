using Microsoft.AspNetCore.Identity;

namespace MiniTwitter.Models.Classes;

public class User : IdentityUser
{
    public string? DisplayName { get; set; }
}