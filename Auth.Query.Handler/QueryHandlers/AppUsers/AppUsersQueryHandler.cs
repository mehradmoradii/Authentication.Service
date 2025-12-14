using Auth.Domain.AppGroups.Aggregate;
using Auth.Domain.AppUsers.Aggregate;
using Auth.Infrastracture.ErrorHandler;
using Auth.Infrastracture.Messages.Queries;
using Auth.Infrastracture.Repository;
using Auth.Query.Dtos.AppProjects;
using Auth.Query.Dtos.AppUsers;
using Auth.Query.Queries.AppUsers;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Query.Handler.QueryHandlers.AppUsers
{
    public class AppUsersQueryHandler :
        IQueryHandler<GetUsersByGroupIdQuery, List<AppUsersSimpleDto>>,
        IQueryHandler<GetAllUsersUrlQuery, List<UrlsDto>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IRepositoryBase<AppGroup, Guid> _geoupRepository;
        private readonly IMapper _mapper;
        public AppUsersQueryHandler(UserManager<AppUser> userManager,IMapper mapper, IRepositoryBase<AppGroup, Guid> geoupRepository)
        {
            _userManager = userManager;
            _mapper = mapper;
            _geoupRepository = geoupRepository;
        }
        public async Task<List<AppUsersSimpleDto>> Handle(GetUsersByGroupIdQuery request, CancellationToken cancellationToken)
        {
            var group = await _geoupRepository.FindByCondition(i => i.Id == request.GroupId).FirstOrDefaultAsync();
            if (group == null)
                throw new AppException("Group NotFound");
            var mappedUser = _mapper.Map<List<AppUsersSimpleDto>>(group.Users.OrderBy(c=>c.CreationDateTime).Select(s=>s.User));
            return mappedUser;
        }

        public async Task<List<UrlsDto>> Handle(GetAllUsersUrlQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                throw new AppException("User NotFound");
            var Urls = user.Groups.Select(u => u.Group).SelectMany(u => u.Urls) ;
            var mappedUrls = _mapper.Map<List<UrlsDto>>(Urls.Select(u => u.Url));
            return mappedUrls;

        }
    }
}
