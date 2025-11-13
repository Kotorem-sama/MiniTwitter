using MiniTwitter.Models;

namespace MiniTwitter.ViewModels;

public class ProfileViewModel
{
    public string DisplayName { get; set; } = string.Empty;
    public List<Tweet> Tweets { get; set; } = new();
}
