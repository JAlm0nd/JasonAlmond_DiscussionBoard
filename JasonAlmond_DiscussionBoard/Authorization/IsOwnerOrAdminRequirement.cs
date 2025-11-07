using Microsoft.AspNetCore.Authorization;

namespace JasonAlmond_DiscussionBoard.Authorization;
public class IsOwnerOrAdminRequirement : IAuthorizationRequirement
{
    public IsOwnerOrAdminRequirement()
    {
    }
}