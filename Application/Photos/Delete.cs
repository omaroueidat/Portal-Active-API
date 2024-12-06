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
    public class Delete
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

                // Check if the user exists
                if (user is null) return null;

                // Check if the photo requested to delete exists
                var photo = user.Photos.FirstOrDefault(p => p.Id == request.Id);

                if (photo is null) return null;

                // User cant delete his main photo, he should change it then delete
                if (photo.IsMain) return Result<Unit>.Failure("You cannot delete your main photo!");

                // Delete the photo
                var result = await _photoAccessor.DeletePhoto(photo.Id);

                // Check the result of deletion from Cloudinary
                if (result is null) return Result<Unit>.Failure("Problem Deleting photo from Cloudinary");

                // Remove the photo from the database
                user.Photos.Remove(photo);
                
                var success = await _context.SaveChangesAsync() > 0;

                if (success) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Problem Deleting photo from API");
            }
        }
    }
}
