using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JasonAlmond_DiscussionBoard.Models;

public class DiscussionBase : EntityBase
{
    [MaxLength(512)]
    public string Title { get; set; }

    public string Content { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("ApplicationUser")] 
    public string ApplicationUserId { get; set; }

    public ApplicationUser ApplicationUser { get; set; }
}