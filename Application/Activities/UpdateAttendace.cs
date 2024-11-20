using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Activities
{
    public class UpdateAttendace
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Get the activity we are working with
                var activity = await _context.Activities
                    .Include(a => a.Attendees)
                    .ThenInclude(aa => aa.AppUser)
                    .SingleOrDefaultAsync(a => a.Id == request.Id);

                if (activity is null) return null;

                // Get the current user from the database
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == _userAccessor.GetUsername());

                // Check is the current user exists
                if (user is null) return null;

                // Get the host Username of the current activity where we will use it later to check
                // if the user is the host or not
                var hostUsername = activity.Attendees.FirstOrDefault(a => a.IsHost)?.AppUser?.UserName;

                // Get the attendance status for the current user in the requested activity
                // where we will check is he is joined in the current activity or not
                var attendace = activity.Attendees.FirstOrDefault(a => a.AppUser.UserName == user.UserName);

                // If the User is in the activity and is the host then cancel the current Activity
                if (attendace is not null && hostUsername == user.UserName)
                {
                    activity.IsCancelled = !activity.IsCancelled;
                }

                // If the User is in the activity and is a normal user then let the user exit the activity
                if (attendace is not null && hostUsername != user.UserName)
                {
                    activity.Attendees.Remove(attendace);
                }

                // If there is no attendace for the current user then add the user to the activity
                if (attendace is null)
                {
                    attendace = new Domain.ActivityAttendee
                    {
                        IsHost = false,
                        AppUserId = user.Id,
                        ActivityId = activity.Id
                    };

                    activity.Attendees.Add(attendace);
                }

                // Save the changes to the database
                var result = await _context.SaveChangesAsync() > 0;

                return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Problem adding attendace!");
            }
        }
    }
}
