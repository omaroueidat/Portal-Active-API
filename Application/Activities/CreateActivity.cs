using Application.Core;
using Application.Interfaces;
using Domain;
using FluentValidation;
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
    public class CreateActivity
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Activity Activity { get; set; }
        }

        // Adding the validation to our creation model
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Activity).SetValidator(new ActivityValidator());
            }
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
                // Get the User from the database based on the logged in user
                var user = await _context.Users.FirstOrDefaultAsync(user =>
                    user.UserName == _userAccessor.GetUsername()
                );

                // Create an Attendee object to add the the table of ActivityAttendee
                var attendee = new ActivityAttendee
                {
                    AppUser = user,
                    Activity = request.Activity,
                    IsHost = true,
                };

                _context.Activities.Add(request.Activity);

                _context.ActivityAttendees.Add(attendee);

                var result = await _context.SaveChangesAsync() > 0; // Check if the number of written entries to the database is > 0

                if (!result) return Result<Unit>.Failure("Failed to create the activity!");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
