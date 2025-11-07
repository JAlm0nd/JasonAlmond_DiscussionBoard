using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using JasonAlmond_DiscussionBoard.Models;
using JasonAlmond_DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;

namespace JasonAlmond_DiscussionBoard.Pages
{
    [Authorize]
    public class DiscussionThreads : PageModel
    {
        private readonly DiscussionThreadService _service;
        private readonly ILogger<DiscussionThreads> _log;
        private readonly IAuthorizationService _authorizationService;


        [BindProperty]
        public ViewItem Discussion { get; set; }

        /*[BindProperty]*/
        public DiscussionThread DiscussionThread { get; set; }

        public DiscussionThreads(DiscussionThreadService service, ILogger<DiscussionThreads> log, IAuthorizationService authorizationService)
        {
            _service = service;
            _log = log;
            _authorizationService = authorizationService;
            Discussion = new ViewItem();
            DiscussionThread = new DiscussionThread();
        }

        public async Task<IActionResult> OnGetAsync(int? Id)
        {
            try
            {
                if (Id != null)
                {
                    DiscussionThread = _service.Get(Id.Value);
                    AuthorizationResult isAuthorized = await _authorizationService.AuthorizeAsync(User, DiscussionThread, Helpers.PolicyTypes.IsOwnerOrAdmin);
                    if (isAuthorized.Succeeded)
                    {
                        Discussion.Id = DiscussionThread.Id;
                        Discussion.Title = DiscussionThread.Title;
                        Discussion.Content = DiscussionThread.Content;

                        _log.LogInformation($"Loaded DiscussionThread with Id: {DiscussionThread.Id}");
                    }
                    else
                    {
                        //Use the built-in "Access Denied" page from Identity UI
                        return LocalRedirect("/Identity/Account/AccessDenied");
                    }
                }
                else if (Id == null || Id == 0)
                {
                    Discussion = new ViewItem();
                    DiscussionThread = new DiscussionThread();
                    DiscussionThread.Title = new ViewItem().Title;
                    DiscussionThread.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                    return Page();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _log.LogWarning($"Exception in OnGet: {ex.Message}");
            }
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {//This was useful error checking so I'm leaving it in
                foreach (var kv in ModelState)
                {
                    var key = kv.Key;
                    var errors = kv.Value.Errors;
                    foreach (var err in errors)
                    {
                        _log.LogError("ModelState error for {Key}: {ErrorMessage}", key, err.ErrorMessage);
                    }
                }
                _log.LogError("Model state is invalid.");
                return Page();
            }

            if (Discussion.Id != 0)
            {
                var existingThread = _service.Get(Discussion.Id);

                if (existingThread != null)
                {
                    AuthorizationResult isAuthorized = await _authorizationService.AuthorizeAsync(User, existingThread, Helpers.PolicyTypes.IsOwnerOrAdmin);
                    if (!isAuthorized.Succeeded)
                    {
                        _log.LogWarning($"Unauthorized attempt to edit DiscussionThread with Id {Discussion.Id}");
                        return Forbid();
                    }

                    existingThread.Title = Discussion.Title;
                    existingThread.Content = Discussion.Content;
                    _service.Update(existingThread);

                    _log.LogInformation($"Updated DiscussionThread with Id {Discussion.Id}");
                }
            }
            else
            {
                DiscussionThread.Title = Discussion.Title;
                DiscussionThread.Content = Discussion.Content;
                DiscussionThread.ApplicationUser = null;
                DiscussionThread.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
                _service.Add(DiscussionThread);
                _log.LogInformation("Added new DiscussionThread");
            }

            /*return LocalRedirect("/Discussion/" + (Discussion.Id != 0 ? Discussion.Id : DiscussionThread.Id));
            */
            return LocalRedirect("/Discussion/" + DiscussionThread.Id);
        }

    }
}