using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniTwitter.Models
{
    public class Tweet
    {
        [Key]
        public int TweetId { get; set; }

        [Required]
        [MaxLength(280)]
        [StringLength(280)]
        public required string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public required string UserId { get; set; }
        public int? ParentTweetId { get; set; }
        public int LikesCount { get; set; } = 0;

        [NotMapped]
        public string TimeSinceCreation
        {
            get
            {
                var timeSpan = DateTime.Now - CreatedAt;

                if (timeSpan.TotalSeconds < 60)
                    return $"{Math.Floor(timeSpan.TotalSeconds)}s ago";
                if (timeSpan.TotalMinutes < 60)
                    return $"{Math.Floor(timeSpan.TotalMinutes)}m ago";
                if (timeSpan.TotalHours < 24)
                    return $"{Math.Floor(timeSpan.TotalHours)}h ago";
                if (timeSpan.TotalDays < 7)
                    return $"{Math.Floor(timeSpan.TotalDays)}d ago";

                return CreatedAt.ToString("dd MMM yyyy");
            }
        }
    }
}