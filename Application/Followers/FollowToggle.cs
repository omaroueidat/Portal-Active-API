using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Domain;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Followers
{
    public class FollowToggle
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string TargetUsername { get; set; }
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
                // Get the current User (which is the observer)
                var observer = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

                // Get the user that the current User is trying to follow
                var target = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserName == request.TargetUsername);

                // Check if we have a target
                if (target is null)
                {
                    return null;
                }

                // Check to see if the relation already exists in the database or not
                var following = await _context.UserFollowings
                    .FindAsync(observer.Id, target.Id);

                // If relation doesnt exist then create a new one
                if (following is null)
                {
                    following = new UserFollowing
                    {
                        Observer = observer,
                        Target = target
                    };

                    // Add the relation to the database
                    _context.UserFollowings.Add(following);
                }
                // If relation exist then delete it from the table
                else
                {
                    _context.UserFollowings.Remove(following);
                }

                // Save the changes to the database
                var success = await _context.SaveChangesAsync() > 0;

                if (success) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Failed to update the following");
            }
        }
    }
}
