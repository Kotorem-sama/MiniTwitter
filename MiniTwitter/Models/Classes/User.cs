using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.Models.Classes;

public class User
{
    [Key]
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
}
