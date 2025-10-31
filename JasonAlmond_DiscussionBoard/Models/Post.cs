using System.ComponentModel.DataAnnotations.Schema;

namespace JasonAlmond_DiscussionBoard.Models;

public class Post : DiscussionBase
{
    [ForeignKey("DiscussionThread")]
    public int DiscussionThreadId { get; set; }

    public DiscussionThread DiscussionThread { get; set; }

    [ForeignKey("ParentPost")]
    public int? ParentPostId { get; set; } // Nullable foreign key for parent post (for replies)

    public Post? ParentPost { get; set; } // Navigation property to parent post (if it's a reply)
    
    public List<Post> SubPosts { get; set; } // List of reply posts (sub-posts)

    public Post()
    {
        DiscussionThread = new DiscussionThread(); // Initialize referenced DiscussionThread
        CreatedAt = DateTime.Now;                   // Initialize CreatedAt to current time
        SubPosts = new List<Post>();                // Initialize the list of sub-posts
    }
}