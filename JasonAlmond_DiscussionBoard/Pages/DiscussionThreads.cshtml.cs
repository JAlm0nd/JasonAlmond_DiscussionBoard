using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using JasonAlmond_DiscussionBoard.Models;
using JasonAlmond_DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;

namespace JasonAlmond_DiscussionBoard.Pages
{
    [Authorize]
    public class DiscussionThreads : PageModel
    {
        private readonly DiscussionThreadService _service;
        private readonly ILogger<DiscussionThreads> _log;

        [BindProperty]
        public ViewItem Discussion { get; set; }

        [BindProperty]
        public DiscussionThread DiscussionThread { get; set; }

        public DiscussionThreads(DiscussionThreadService service, ILogger<DiscussionThreads> log)
        {
            _service = service;
            _log = log;

            Discussion = new ViewItem();
            DiscussionThread = new DiscussionThread();
        }

        public void OnGet(int? Id)
        {
            try
            {
                if (Id != null)
                {
                    DiscussionThread = _service.Get(Id.Value);

                    if (DiscussionThread != null)
                    {
                        Discussion.Id = DiscussionThread.Id;
                        Discussion.Title = DiscussionThread.Title;
                        Discussion.Content = DiscussionThread.Content;

                        _log.LogInformation($"Loaded DiscussionThread with Id: {DiscussionThread.Id}");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogWarning($"Exception in OnGet: {ex.Message}");
            }
        }

        public IActionResult OnPost()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _log.LogError("Model state is invalid.");
                    return Page();
                }

                if (Discussion.Id != 0)
                {
                    // Update existing DiscussionThread
                    var existingThread = _service.Get(Discussion.Id);

                    if (existingThread != null)
                    {
                        existingThread.Title = Discussion.Title;
                        existingThread.Content = Discussion.Content;

                        _service.Update(existingThread);

                        _log.LogInformation($"Updated DiscussionThread with Id {Discussion.Id}");
                    }
                }
                else
                {
                    // Add new DiscussionThread
                    DiscussionThread.Title = Discussion.Title;
                    DiscussionThread.Content = Discussion.Content;
                    DiscussionThread.ApplicationUser = null;
                    DiscussionThread.ApplicationUserId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier) ?? string.Empty;

                    _service.Add(DiscussionThread);

                    _log.LogInformation("Added new DiscussionThread");
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"Exception in OnPost: {ex.Message}");
            }

            return LocalRedirect("/Discussion/" + DiscussionThread.Id);
        }

    }
}