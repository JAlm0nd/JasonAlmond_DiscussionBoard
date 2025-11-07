using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JasonAlmond_DiscussionBoard.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(100)] // Limit the First Name to 100 characters
    public string FirstName { get; set; }

    [MaxLength(100)] // Limit the Last Name to 100 characters
    public string LastName { get; set; }

    public List<DiscussionThread> Threads { get; set; } // List of threads created by the user
    public List<Post> Posts { get; set; } // List of posts created by the user

    public ApplicationUser()
    {
        Threads = new List<DiscussionThread>();
        Posts = new List<Post>();
    }

    public override string ToString()
    {
        return $"{FirstName} {LastName} ({UserName})";
    }
}