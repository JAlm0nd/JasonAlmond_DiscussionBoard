using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JasonAlmond_DiscussionBoard.Models;

public class Post : DiscussionBase
{
    [ForeignKey("DiscussionThread")]
    public int DiscussionThreadId { get; set; } // Foreign key to the DiscussionThread

    public DiscussionThread DiscussionThread { get; set; } // Navigation property to DiscussionThread

    [ForeignKey("ParentPost")]
    public int? ParentPostId { get; set; } // Nullable foreign key for parent post (for replies)

    public Post ParentPost { get; set; } // Navigation property to parent post (if it's a reply)
}