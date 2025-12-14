using Auth.Domain.AppProjects.Aggregate;
using Auth.Domain.AppProjects.Entity;
using Auth.Infrastracture.Messages.Queries;
using Auth.Infrastracture.Repository;
using Auth.Query.Dtos.AppProjects;
using Auth.Query.Helpers;
using Auth.Query.Queries.AppProjects;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Auth.Query.Handler.QueryHandlers.AppProjects
{
    public class AppProjectsQueryHandler :
        IQueryHandler<GetAllProjectQuery, PagedResult<AppProjectSimpleDto>>,
        IQueryHandler<GetUrlsByProjectIdQuery, PagedResult<AppUrlSimpleDto>>
    {
        private readonly IRepositoryBase<AppProject, Guid> _projectRepository;
        private readonly IRepositoryBase<AppUrl, Guid> _urlRepository;
        private readonly IMapper _mapper;
        public AppProjectsQueryHandler(IRepositoryBase<AppUrl, Guid> urlRepository, IMapper mapper, IRepositoryBase<AppProject, Guid> projectRepository)
        {
            _urlRepository = urlRepository;
            _mapper = mapper;
            _projectRepository = projectRepository;
        }
        public async Task<PagedResult<AppProjectSimpleDto>> Handle(GetAllProjectQuery request, CancellationToken cancellationToken)
        {
            var projects = _projectRepository.FindAll();
            var result = PaginationHelper.ToPagedResultDynamic<AppProject, AppProjectSimpleDto>(projects.OrderBy(c=>c.CreationDateTime), request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
            return await result;
        }

        public async Task<PagedResult<AppUrlSimpleDto>> Handle(GetUrlsByProjectIdQuery request, CancellationToken cancellationToken)
        {
            var urls = _urlRepository.FindByCondition(i => i.ProjectId == request.ProjectId).ApplyFilter(request.UrlFilter);
            var mapperConfig = _mapper.ConfigurationProvider;

            return await PaginationHelper.ToPagedResultDynamic<AppUrl, AppUrlSimpleDto>(
                urls,
                request.PageNumber,
                request.PageSize,
                mapperConfig,
                cancellationToken);
        }
    }
}
