using System.ComponentModel.DataAnnotations.Schema;

namespace JasonAlmond_DiscussionBoard.Models;

public class DiscussionThread : DiscussionBase
{
    public List<Post> Posts { get; set; } // List of posts related to this thread

    [NotMapped]
    public int PostCount
    {
        get
        {
            return Posts?.Count ?? 0;
        }
    }

    public DiscussionThread()
    {
        Posts = new List<Post>();
        CreatedAt = DateTime.Now;
    }
}