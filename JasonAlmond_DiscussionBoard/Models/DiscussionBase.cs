using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;//?

namespace JasonAlmond_DiscussionBoard.Models;

public class DiscussionBase : EntityBase
{
    [MaxLength(512)] // Limiting the title to 512 characters
    public string Title { get; set; }

    public string Content { get; set; } // Content of the discussion

    public DateTime CreatedAt { get; set; } // When the discussion was created

    [ForeignKey("ApplicationUser")] 
    public string ApplicationUserId { get; set; } // Foreign key to ApplicationUser

    public ApplicationUser ApplicationUser { get; set; } // Navigation property to ApplicationUser
}