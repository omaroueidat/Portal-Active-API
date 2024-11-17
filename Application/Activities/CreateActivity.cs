using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
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
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                _context.Activities.Add(request.Activity);

                var result = await _context.SaveChangesAsync() > 0; // Check if the number of written entries to the database is > 0

                if (!result) return Result<Unit>.Failure("Failed to create the activity!");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
