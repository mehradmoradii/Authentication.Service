using Auth.Domain.AppGroups.Aggregate;
using Auth.Infrastracture.Messages.Queries;
using Auth.Infrastracture.Repository;
using Auth.Query.Dtos.AppGroups;
using Auth.Query.Queries.AppGroups;
using Auth.Repository.Base;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Query.Handler.QueryHandlers.AppGroups
{
    public class AppGroupQueryHandler :
        IQueryHandler<GetAllAppGroupQuery, List<AppGroupSimpleDto>>
       
    {
        private readonly IRepositoryBase<AppGroup, Guid> _groupRepositoryBase;
        private readonly IMapper _mapper;

        public AppGroupQueryHandler(IRepositoryBase<AppGroup, Guid> groupRepositoryBase, IMapper mapper)
        {
            _groupRepositoryBase = groupRepositoryBase;
            _mapper = mapper;
        }
        public async Task<List<AppGroupSimpleDto>> Handle(GetAllAppGroupQuery request, CancellationToken cancellationToken)
        {
            var groups = await _groupRepositoryBase
                                .FindByCondition(p => p.Parent.Title == "Root").IgnoreQueryFilters()
                                .OrderBy(c => c.CreationDateTime)
                                .Skip((request.pageNumber - 1) * request.pageSize)
                                .Take(request.pageSize)
                                .ToListAsync(cancellationToken);
            if (groups ==null)
            {
                groups = new List<AppGroup>();
            }
            var mappedGroup = _mapper.Map<List<AppGroupSimpleDto>>(groups);
            return mappedGroup;
        }
    }
}
