using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JasonAlmond_DiscussionBoard.Models
{
    public class ViewItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(512)] // Limiting the title to 512 characters
        public string Title { get; set; }

        public string Content { get; set; }
    }
}