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

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<Result<List<ActivityDto>>> { }
        public class Handler : IRequestHandler<Query, Result<List<ActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper) 
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<List<ActivityDto>>> Handle(Query query, CancellationToken cancellationToken)
            {
                var activities =  await _context.Activities
                    .Include(a => a.Attendees)           
                    .ThenInclude(aa => aa.AppUser)
                    .ThenInclude(au => au.Photos)
                    .ToListAsync();

                var activitiesToReturn = _mapper.Map<List<ActivityDto>>(activities);

                return Result<List<ActivityDto>>.Success(activitiesToReturn);
            }
        }
    }
}
