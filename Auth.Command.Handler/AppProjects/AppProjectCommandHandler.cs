using Auth.Command.Commands.AppProjects;
using Auth.Command.Handler.Helpers;
using Auth.Domain.AppGroups.Aggregate;
using Auth.Domain.AppGroups.Entity;
using Auth.Domain.AppProjects.Entity;
using Auth.Infrastracture.ErrorHandler;
using Auth.Infrastracture.Messages.Commands;
using Auth.Infrastracture.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Command.Handler.AppProjects
{
    public class AppProjectCommandHandler : 
        ICommandHandler<AddUrlToGroupCommand>,
        ICommandHandler<RemoveUrlFromGroupCommand>
    {
        private readonly IRepositoryBase<AppUrl, Guid> _urlRepository;
        private readonly IRepositoryBase<AppGroup, Guid> _groupRepository;
        public AppProjectCommandHandler(IRepositoryBase<AppGroup, Guid> groupRepository, IRepositoryBase<AppUrl, Guid> urlRepository)
        {
            _groupRepository = groupRepository;
            _urlRepository = urlRepository;
        }
        public async Task Handle(AddUrlToGroupCommand request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.FindByCondition(r => r.Id == request.GroupId).FirstOrDefaultAsync();
            if (group == null)
                throw new AppException("Invalid Group");
            var SubGroupList = new List<AppGroup>();
            var subGroups = await SubGroupHelper.FindAllSubGroup(group, SubGroupList);
            foreach(var url in request.Urls)
            {
                var obj = await _urlRepository.FindByCondition(i => i.Id == url).FirstOrDefaultAsync();
                if (obj == null)
                    throw new AppException("invalid url");
                foreach(var subGroup in subGroups)
                {
                    if (obj.Groups.Any(s => s.Group == subGroup))
                        continue;
                    var newPerm = new AppGroupUrl { GroupId = subGroup.Id, UrlId= obj.Id };
                    obj.Groups.Add(newPerm);
                }
            }
            await _urlRepository.Save();
        }

        public async Task Handle(RemoveUrlFromGroupCommand request, CancellationToken cancellationToken)
        {
            // 1) Load the root group including its Url relations
            var group = await _groupRepository
                .FindByCondition(g => g.Id == request.GroupId)
                .Include(g => g.Urls)
                .FirstOrDefaultAsync();

            if (group == null)
                throw new AppException("Invalid Group");

            // 2) Load all subgroups with Urls included
            var subGroups = await SubGroupHelper
                .FindAllSubGroup(group, new List<AppGroup>());


            // 3) Remove relations
            foreach (var urlId in request.Urls)
            {
                foreach (var subGroup in subGroups)
                {
                    // Find the relation from this subgroup
                    var relation = subGroup.Urls
                        .FirstOrDefault(x => x.UrlId == urlId);

                    if (relation != null)
                    {
                        subGroup.Urls.Remove(relation);
                    }
                }
            }

            // 4) Save everything
            await _groupRepository.Save();
        }

    }
}
