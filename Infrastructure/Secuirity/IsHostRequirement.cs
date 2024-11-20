using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Secuirity
{
    public class IsHostRequirement : IAuthorizationRequirement
    {
    }

    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        private readonly DataContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IsHostRequirementHandler(DataContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            // Get the user Id
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
            {
                return Task.CompletedTask;
            }

            // Get the activity Id
            // The Activity Id is in the route parameter, so we will parse it from a string to a Guid
            var activityId = Guid.Parse(_httpContextAccessor.HttpContext?
                .Request.RouteValues.SingleOrDefault(x => x.Key == "id").Value.ToString());

            // Get the attendees of the activity
            var attendee = _dbContext.ActivityAttendees
                .AsNoTracking() // This will not track the atendee in memeory any more which will solve the problem of deleted hostusername
                .SingleOrDefaultAsync(a => a.AppUserId == userId && a.ActivityId == activityId)
                .Result;

            // Failed if the attendee is null (User is not in the activity (he didint join))
            if (attendee is null) return Task.CompletedTask;

            // Succeeded if the attendee is in the activity and is the host
            if (attendee.IsHost) context.Succeed(requirement);

            return Task.CompletedTask; 
        }
    }
}
