using JasonAlmond_DiscussionBoard.Models;

namespace JasonAlmond_DiscussionBoard.Repos;

public class PostRepo : RepoBase<Post>
{
    public PostRepo(IConfiguration config) : base(config)
    {
    }
}
