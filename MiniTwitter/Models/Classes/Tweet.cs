using System.ComponentModel.DataAnnotations;

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
    }
}