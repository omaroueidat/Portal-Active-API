using MediatR;
using System;
using System.Collections.Generic;
using Domain;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Persistence;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Application.Core;
using AutoMapper;
using Application.Interfaces;
using AutoMapper.QueryableExtensions;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<ActivityDto>>> 
        {
            public ActivityParams Params { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<PagedList<ActivityDto>>>
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

            public IUserAccessor UserAccessor { get; }

            public async Task<Result<PagedList<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.Activities
                    .Where(d => d.Date >= request.Params.StartDate) // Get only the future activities
                    .OrderBy(d => d.Date)
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUsername() })
                    .AsQueryable();

                // Modify the query depending on the reuqest params
                
                // Get the activities that the user is attending
                if (request.Params.IsGoing && !request.Params.IsHost)
                {
                    query = query.Where(x => x.Attendees.Any(a => a.Username == _userAccessor.GetUsername()));
                }

                // Get the activities that the user is hosting
                if (request.Params.IsHost && !request.Params.IsGoing)
                {
                    query = query.Where(x => x.HostUsername == _userAccessor.GetUsername());
                }

                return Result<PagedList<ActivityDto>>.Success(
                    await PagedList<ActivityDto>
                        .CreateAsync(query, request.Params.PageNumber, request.Params.pageSize)
                   );
            }
        }
    }
}
