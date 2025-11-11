using System.ComponentModel.DataAnnotations;

namespace MiniTwitter.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Username is required!")]
    public required string UserName { get; set; }

    [Required(ErrorMessage = "Email is required!")]
    [EmailAddress]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password is required!")]
    [StringLength(40, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 40 characters!")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Confirm Password is required!")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "Passwords do not match!")]
    public required string ConfirmPassword { get; set; }
}