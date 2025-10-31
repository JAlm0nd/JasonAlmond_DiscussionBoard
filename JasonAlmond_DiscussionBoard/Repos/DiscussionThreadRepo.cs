using JasonAlmond_DiscussionBoard.Models;

namespace JasonAlmond_DiscussionBoard.Repos
{
    public class DiscussionThreadRepo : RepoBase<DiscussionThread>
    {
        public DiscussionThreadRepo(IConfiguration config) : base(config) { }
    }
}
