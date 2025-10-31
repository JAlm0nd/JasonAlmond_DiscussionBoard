using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using JasonAlmond_DiscussionBoard.Models;
using JasonAlmond_DiscussionBoard.Services;
using System;
using System.Collections.Generic;

namespace JasonAlmond_DiscussionBoard.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DiscussionThreadService _discussionThreadService;

       // public List<DiscussionThread> DiscussionThreads { get; private set; }
       public List<DiscussionThread> DiscussionThreads { get; set; }
        public IndexModel(ILogger<IndexModel> logger, DiscussionThreadService discussionThreadService)
        {
            _logger = logger;
            _discussionThreadService = discussionThreadService;
            DiscussionThreads = new List<DiscussionThread>();
        }

        public void OnGet()
        {
            try
            {
                // Fetch fresh list from service; assigns directly to avoid duplicates
                DiscussionThreads = _discussionThreadService.GetAll();
                _logger.LogInformation($"Loaded {DiscussionThreads.Count} discussion threads from database.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error loading discussion threads on Index page: " + ex.Message);
            }
        }
    }
}