using System.Collections.Generic;

namespace JasonAlmond_DiscussionBoard.Models;

public class DiscussionThread : DiscussionBase
{
    public List<Post> Posts { get; set; } // List of posts related to this thread

    public DiscussionThread()
    {
        Posts = new List<Post>();
    }
}