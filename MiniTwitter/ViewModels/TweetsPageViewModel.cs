using MiniTwitter.Models;

namespace MiniTwitter.ViewModels;

public class TweetsPageViewModel
    {
        public Tweet NewTweet { get; set; } = new Tweet()
        {
            Content = string.Empty,
            UserId = string.Empty
        };
        public IEnumerable<Tweet> AllTweets { get; set; } = new List<Tweet>();
    }
