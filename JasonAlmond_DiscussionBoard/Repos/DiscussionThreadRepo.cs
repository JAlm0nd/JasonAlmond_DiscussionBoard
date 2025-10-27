using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JasonAlmond_DiscussionBoard.Models;
using JasonAlmond.DiscussionBoard.Repos;

namespace JasonAlmond_DiscussionBoard.Repos
{
    public class DiscussionThreadRepo : RepoBase<DiscussionThread>
    {
        public DiscussionThreadRepo(IConfiguration config) : base(config) { }
    }
}
