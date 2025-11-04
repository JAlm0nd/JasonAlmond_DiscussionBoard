using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using JasonAlmond_DiscussionBoard.Helpers;
using JasonAlmond_DiscussionBoard.Models;
using JasonAlmond_DiscussionBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace JasonAlmond_DiscussionBoard.Pages
{
    [Authorize(Policy = PolicyTypes.IsAdmin)]
    public class UserAdmin : PageModel
    {
        private readonly ApplicationUserService _service;
        private readonly ILogger<UserAdmin> _logger;

        [BindProperty(SupportsGet = true)]
        public string? Id { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsModerator { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public List<Claim> Claims { get; set; }

        public UserAdmin(ApplicationUserService service, ILogger<UserAdmin> logger)
        {
            _service = service;
            _logger = logger;
            Users = new List<ApplicationUser>();
            Claims = new List<Claim>();
            ApplicationUser = new ApplicationUser();
        }

        /*public async Task OnGet()
        {
            try
            {
                Users = await _service.GetAllUsers();

                if (!string.IsNullOrEmpty(Id))
                {
                    ApplicationUser = await _service.GetUserAsync(Id);
                    Claims = await _service.GetApplicationClaimsAsync(Id);
                    IsAdmin = await _service.IsAdminAsync(Id);
                    IsModerator = await _service.IsModeratorAsync(Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in UserAdmin OnGet: {ex.Message}");
            }
        }*/
        public async Task OnGet()
        {
            try
            {
                Users = await _service.GetAllUsers();
                _logger.LogInformation("UserAdmin OnGetAsync loaded {Count} users", Users?.Count ?? 0);

                if (!string.IsNullOrEmpty(Id))
                {
                    ApplicationUser = await _service.GetUserAsync(Id);
                    Claims = await _service.GetApplicationClaimsAsync(Id);
                    IsAdmin = await _service.IsAdminAsync(Id);
                    IsModerator = await _service.IsModeratorAsync(Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in UserAdmin OnGetAsync: {ex.Message}");
            }
        }
        
        /*public async Task<IActionResult> OnPostAsync(string Type, string Value)
        {
            try
            {
                await _service.UpsertUserdClaimsAsync(Id, Type, Value);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in UserAdmin OnGetAsync: {ex.Message}");
            }
            return LocalRedirect($"/UserAdmin?id={Id}");
        }*/
    }
}
