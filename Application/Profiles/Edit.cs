using Application.Activities;
using Application.Core;
using Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Profiles
{
    public class Edit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string DisplayName { get; set; }
            public string Bio { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.DisplayName).NotEmpty();
                RuleFor(x => x.Bio).MaximumLength(400);
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
                // We have to get the current logged in user
                var user = await _context.Users
                    .SingleOrDefaultAsync(u => u.UserName == _userAccessor.GetUsername());

                // Check if the user still exists
                if (user is null)
                {
                    return null;
                }

                // Change the Displayname and Bio
                user.DisplayName = request.DisplayName;
                user.Bio = request.Bio;

                // Save the changes
                var success = await _context.SaveChangesAsync() > 0;

                // Check if their were any changes
                if (success)
                {
                    return Result<Unit>.Success(Unit.Value);
                }

                return Result<Unit>.Failure("Problem Updating Profile! Contact Support if the problem persists!");
            }
        }


    }
}
