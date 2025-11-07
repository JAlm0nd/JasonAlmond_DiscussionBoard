using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using JasonAlmond_DiscussionBoard.Models;
using JasonAlmond_DiscussionBoard.Helpers;
using System.Linq;

namespace JasonAlmond_DiscussionBoard.Services
{
    public class ApplicationUserService
    {
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly ILogger<ApplicationUserService> Logger;

        private ApplicationUser? ApplicationUser;
        private List<Claim> Claims;

        public ApplicationUserService(UserManager<ApplicationUser> userManager, ILogger<ApplicationUserService> logger)
        {
            UserManager = userManager;
            Logger = logger;
            ApplicationUser = null;
            Claims = new List<Claim>();
        }

        public async Task<ApplicationUser> GetUserAsync(string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            if(user == null)
            {
                throw new ArgumentException($"No user with the UserId {userId} was found");
            }
            ApplicationUser = user;
            return ApplicationUser;
        }
        
        public async Task<List<ApplicationUser>> GetAllUsers()
        {
            List<ApplicationUser> users = await UserManager.Users.ToListAsync();
            return users;
        }

        public async Task<bool> IsAdminAsync(String UserId)
        {
            if (!ApplicationUser.Id.Equals(UserId))
            {
                ApplicationUser = await GetUserAsync(UserId);
            }
            foreach (Claim claim in await GetApplicationClaimsAsync(UserId))
            {
                if (claim.Type.Equals(PolicyTypes.IsAdmin))
                {
                    return Boolean.Parse(claim.Value);
                }
            }
            return false;
        }
        
        public async Task<bool> IsModeratorAsync(string UserId)
        {
            if (!ApplicationUser.Id.Equals(UserId))
            {
                ApplicationUser = await GetUserAsync(UserId);
            }

            foreach(var claim in Claims)
            {
                if (claim.Type.Equals(PolicyTypes.IsModerator))
                {
                    return Boolean.Parse(claim.Value);
                }
            }
            return false;
        }

        public async Task<List<Claim>> GetApplicationClaimsAsync(string UserId)
        {
            if (!ApplicationUser.Id.Equals(UserId))
            {
                ApplicationUser = await GetUserAsync(UserId);
            }
            return (await UserManager.GetClaimsAsync(ApplicationUser)).ToList();
        }

        public async Task UpsertUserClaimsAsync(string UserId, string type, string value)
        {
            if (!ApplicationUser.Id.Equals(UserId))
            {
                ApplicationUser = await GetUserAsync(UserId);
            }

            Claim claim = null;
            try
            {
                if (type == PolicyTypes.IsAdmin || type == PolicyTypes.IsModerator)
                {
                    claim = new Claim(type, value);
                }
                else
                {
                    throw new InvalidOperationException($"Invalid claim '{type}' for this application");
                }

                if (!string.IsNullOrEmpty(UserId) && claim != null)
                {
                    if(ApplicationUser == null || !ApplicationUser.Id.Equals(UserId))
                    {
                        ApplicationUser = await GetUserAsync(UserId);
                    }

                    foreach (var existingClaim in await GetApplicationClaimsAsync(UserId))
                    {
                        if (existingClaim.Type.Equals(claim.Type))
                        {
                            await UserManager.RemoveClaimAsync(ApplicationUser, existingClaim);
                            break;
                        }
                    }

                    var result = await UserManager.AddClaimAsync(ApplicationUser, claim);

                    if (result.Succeeded)
                    {
                        Logger.LogInformation($"Added Claim {claim.Type} with value {claim.Value} to user {UserId}");
                    }
                    else
                    {
                        var msg = $"Failed to add Claim {claim.Type} with value {claim.Value} to user {UserId}";
                        Logger.LogError(msg);
                        throw new InvalidOperationException(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception in UpsertUserClaimsAsync: {ex.Message}");
                throw;
            }
        }
    }
}
