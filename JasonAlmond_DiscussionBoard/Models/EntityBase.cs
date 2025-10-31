using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JasonAlmond_DiscussionBoard.Models;

public class EntityBase
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
    [ConcurrencyCheck]
    public long Timestamp { get; set; }

    public EntityBase()
    {
        IsDeleted = false;
        Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
    }
}