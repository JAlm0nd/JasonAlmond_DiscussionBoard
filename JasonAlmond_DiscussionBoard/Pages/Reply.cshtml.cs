using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using JasonAlmond_DiscussionBoard.Models;
using JasonAlmond_DiscussionBoard.Services;

namespace JasonAlmond_DiscussionBoard.Pages
{
    [Authorize]
    public class Reply : PageModel
    {
        private readonly DiscussionThreadService _discussionThreadService;
        private readonly PostService _postService;
        private readonly ILogger<Reply> _log;

        public DiscussionThread DiscussionThread { get; set; }
        [BindProperty(SupportsGet = true)]
        public int ThreadId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? PostId { get; set; }

        [BindProperty(SupportsGet = true)]
        public ViewItem Parent { get; set; }

        [BindProperty(SupportsGet = true)]
        private Post Post { get; set; }

        public Reply(DiscussionThreadService discussionThreadService, 
            PostService postService, ILogger<Reply> log)
        {
            _discussionThreadService = discussionThreadService;
            _postService = postService;
            _log = log;

            DiscussionThread = new DiscussionThread();
            Parent = new ViewItem();
            Post = new Post();
        }

        public void OnGet()
        {
            try
            {
                if (ThreadId > 0)
                {
                    DiscussionThread = _discussionThreadService.Get(ThreadId);
                    if (DiscussionThread != null)
                    {
                        Parent.Id = DiscussionThread.Id;
                        Parent.Title = DiscussionThread.Title;
                        Parent.Content = DiscussionThread.Content;
                    }
                    else
                    {
                        // Optional: Initialize or handle missing discussion thread scenario
                        Parent = new ViewItem();
                    }

                    if (PostId != null && PostId > 0)
                    {
                        Post = _postService.Get(PostId.Value);

                        if (Post != null)
                        {
                            Parent.Id = Post.Id;
                            Parent.Title = Post.Title;
                            Parent.Content = Post.Content;
                            ThreadId = Post.DiscussionThreadId;
                        }
                        else if (ThreadId > 0)
                        {
                            DiscussionThread = _discussionThreadService.Get(ThreadId);
                            Parent.Id = DiscussionThread.Id;
                            Parent.Title = DiscussionThread.Title;
                            Parent.Content = DiscussionThread.Content;
                        }

                        _log.LogInformation($"Loaded reply page for ThreadId {ThreadId} and PostId {PostId}");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"Error loading thread with Id {ThreadId}. Exception: {ex.Message}");
            }
        }

        [Authorize]
        public IActionResult OnPost()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                if (PostId != null && PostId > 0)
                {
                    Post.ParentPostId = PostId.Value;
                }

                    Post.DiscussionThreadId = ThreadId;
                    Post.Title = Parent.Title;
                    Post.Content = Parent.Content;
                    Post.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? String.Empty;
//Don't confuse the FK mapping
                    Post.DiscussionThread = null;
                    Post.ApplicationUser = null;
                    Post.ParentPost = null;
                    _postService.Add(Post);
                    _log.LogInformation("Added reply with Id {PostId}", Post.Id);
            }
            catch (Exception ex)
            {
                _log.LogError($"Error saving reply for ThreadId {ThreadId}. Exception: {ex.Message}");
            }

            return LocalRedirect("/Discussion/" + ThreadId);
        }
    }
}
