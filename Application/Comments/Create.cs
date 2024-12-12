using Application.Core;
using Application.Interfaces;
using AutoMapper;
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

namespace Application.Comments
{
    public class Create
    {
        public class Command : IRequest<Result<CommentDto>>
        {
            public string Body { get; set; }
            public Guid ActivityId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(c => c.Body).NotEmpty().MaximumLength(100);
            }
        }

        public class Handler : IRequestHandler<Command, Result<CommentDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;
            }

            public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Get the cuurent activity
                var activity = await _context.Activities.FindAsync(request.ActivityId);

                // Check if the activity is null
                if (activity is null) return null;

                // Get the current user
                var user = await _context.Users
                    .Include(u => u.Photos)
                    .SingleOrDefaultAsync(u => u.UserName == _userAccessor.GetUsername()); 

                // Create the Comment object to add to the database
                var comment = new Comment
                {
                    Author = user,
                    Activity = activity,
                    Body = request.Body
                };

                // Add the Comment to the activity
                activity.Comments.Add(comment);

                // Save the changes to the database
                var success = await _context.SaveChangesAsync() > 0;

                if (success) return Result<CommentDto>.Success(_mapper.Map<CommentDto>(comment));

                return Result<CommentDto>.Failure("Failed to add comment!");
            }
        }
    }
}
