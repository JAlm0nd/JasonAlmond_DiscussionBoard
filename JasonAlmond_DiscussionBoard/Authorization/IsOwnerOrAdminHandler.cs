using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using JasonAlmond_DiscussionBoard.Helpers;
using JasonAlmond_DiscussionBoard.Models;

namespace JasonAlmond_DiscussionBoard.Authorization
{
    public class IsOwnerOrAdminHandler : AuthorizationHandler<IsOwnerOrAdminRequirement, DiscussionBase>
    {
        public IsOwnerOrAdminHandler()
        {
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            IsOwnerOrAdminRequirement requirement,
            DiscussionBase resource)
        {
            // Get the user's Id from the claims, and trim just in case there's any whitespace
            String UserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            if (!string.IsNullOrEmpty(UserId))
            {
                if (resource.Id == 0)
                {
                    // This is a new DiscussionThread or Post, not an edit or delete
                    context.Succeed(requirement);
                }
                else if (resource.ApplicationUserId.Equals(UserId))
                {
                    // Editing our own DiscussionThread or Post
                    context.Succeed(requirement);
                }
                else if (context.User.HasClaim(PolicyTypes.IsAdmin, PolicyValues.True))
                {
                    //Admin can edit anyone's
                    context.Succeed(requirement);
                }
                else// It's not new, you don't own it, and you aren't an admin. What were you thinking?!?
                {
                    context.Fail();
                }
            }
            else// If you aren't logged in, you fail
            {
                context.Fail();
            }
            //We should never get here, but...
            return Task.CompletedTask;
        }
    }
}