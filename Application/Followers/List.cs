using Application.Core;
using Application.Interfaces;
using Application.Profiles;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Application.Followers
{
    public class List
    {
        public class Query : IRequest<Result<List<Profiles.Profile>>> 
        {
            public string Predicate { get; set; }
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<Profiles.Profile>>>
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

            public async Task<Result<List<Profiles.Profile>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // List to store the Profiles
                var profiles = new List<Profiles.Profile>();

                // Conditions to see what the user requested to
                switch (request.Predicate)
                {
                    // Get all the followers of the specific user
                    // That is done by searching the UserFollowing for all relations that have the target as the current User
                    // When the target is the current user then it means the observers that are following the user
                    case "followers":
                        profiles = await _context.UserFollowings.Where(x => x.Target.UserName == request.Username)
                            .Select(u => u.Observer)
                            .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider, 
                                new { currentUsername = _userAccessor.GetUsername()})
                            .ToListAsync();

                        break;

                    // Get all the followings of the specific user
                    // That is done by searching for the UserFollowing for all relations that have the Observer as the current User
                    // When the observer is the current user then it means that the targets are the ones that the user is following
                    case "following":
                        profiles = await _context.UserFollowings.Where(x => x.Observer.UserName == request.Username)
                            .Select(u => u.Target)
                            .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider,
                                new { currentUsername = _userAccessor.GetUsername() })
                            .ToListAsync();

                        break;
                    
                    // If the request provided invalid option, then return null
                    default: return null;
                }

                // Return the list of requested profiles
                return Result<List<Profiles.Profile>>.Success(profiles);
            }
        }


    }
}
