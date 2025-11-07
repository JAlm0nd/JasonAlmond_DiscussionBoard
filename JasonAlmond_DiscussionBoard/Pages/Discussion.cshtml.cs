using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using JasonAlmond_DiscussionBoard.Models;
using JasonAlmond_DiscussionBoard.Services;

namespace JasonAlmond_DiscussionBoard.Pages
{
    [Authorize]
    public class Discussion : PageModel
    {
        private readonly ILogger<Discussion> _log;
        private readonly DiscussionThreadService _discussionThreadService;
        
        [BindProperty(SupportsGet = true)]
        public int ThreadId { get; set; }
        
        public DiscussionThread DiscussionThread { get; set; }

        public Discussion(ILogger<Discussion> log, DiscussionThreadService discussionThreadService)
        {
            _log = log;
            _discussionThreadService = discussionThreadService;
        }

        /*public void OnGet(int ThreadId)
        {
            try
            {
                // Use ThreadId if you want to bind from a route/query parameter, otherwise use DiscussionThread.Id directly
                DiscussionThread = _discussionThreadService.Get(ThreadId);
            }
            catch (Exception ex)
            {
                _log.LogError($"Error loading posts for ThreadId {ThreadId}: {ex.Message}");
            }
        }*/
        public void OnGet()
        {
            try
            {
                DiscussionThread = _discussionThreadService.Get(ThreadId);

                if (DiscussionThread == null)
                {
                    DiscussionThread = new DiscussionThread
                    {
                        Title = "Thread Not Found",
                        ApplicationUser = new ApplicationUser { UserName = "Unknown" },
                        CreatedAt = DateTime.Now,
                        Content = "This thread could not be found."
                    };
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"Error loading thread with Id {ThreadId}: {ex.Message}");
                // Initialize with default to prevent null refs
                DiscussionThread = new DiscussionThread();
            }
        }
    }
}