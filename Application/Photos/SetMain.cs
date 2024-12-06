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

namespace Application.Photos
{
    public class SetMain
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IPhotoAccessor _photoAccessor;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
            {
                _context = context;
                _photoAccessor = photoAccessor;
                _userAccessor = userAccessor;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Get the logged in user
                var user = await _context.Users
                    .Include(u => u.Photos)
                    .FirstOrDefaultAsync(u => u.UserName == _userAccessor.GetUsername());

                // Check if the user is null
                if (user is null) return null;

                // Get the photo from the user photos
                var photo = user.Photos.FirstOrDefault(p => p.Id == request.Id);

                // Check if the photo exists
                if (photo is null) return null;

                // Get the current main photo of the user
                var currentMain = user.Photos.FirstOrDefault(p => p.IsMain);

                // Set the main photo to false
                if (currentMain is not null) currentMain.IsMain = false;

                // Set the chosen photo to be the main photo
                photo.IsMain = true;

                // Save the updates to the database
                var success = await _context.SaveChangesAsync() > 0;

                if (success) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Problem Setting Main Photo!");
            }
        }
    }
}
