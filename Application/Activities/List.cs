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

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<Result<List<Activity>>> { }
        public class Handler : IRequestHandler<Query, Result<List<Activity>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context) 
            {
                _context = context;
            }

            public async Task<Result<List<Activity>>> Handle(Query query, CancellationToken cancellationToken)
            {
                var activities =  await _context.Activities.ToListAsync();

                return Result<List<Activity>>.Success(activities);
            }
        }
    }
}
