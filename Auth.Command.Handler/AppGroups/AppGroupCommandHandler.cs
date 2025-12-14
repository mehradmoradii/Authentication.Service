using Auth.Command.Commands.AppGroups;
using Auth.Domain.AppGroups.Aggregate;
using Auth.Domain.AppGroups.Entity;
using Auth.Domain.AppUsers.Aggregate;
using Auth.Infrastracture.ErrorHandler;
using Auth.Infrastracture.Messages.Commands;
using Auth.Infrastracture.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Command.Handler.AppGroups
{
    public class AppGroupCommandHandler : 
        ICommandHandler<CreateAppGroupCommand>,
        ICommandHandler<AssignGroupToUserCommand>
    {
        private readonly IRepositoryBase<AppGroup, Guid> _groupRepository;
        private readonly UserManager<AppUser> _userManager;
        public AppGroupCommandHandler(UserManager<AppUser> userManager, IRepositoryBase<AppGroup, Guid> groupRepository)
        {
            _userManager = userManager;
            _groupRepository = groupRepository;
        }
        public async Task Handle(CreateAppGroupCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Title))
                throw new AppException("Please Enter Group's Title ...");
            AppGroup? parent;
            var prId = Guid.TryParse(request.ParentId.ToString(), out var parentId);
            if(parentId == null)
            {
                parent = await _groupRepository.FindByCondition(i=>i.Id == request.ParentId).FirstOrDefaultAsync();
                if (parent == null)
                    throw new AppException("Group parent not found...");
            }
            else
            {
                parent = await _groupRepository.FindByCondition(t => t.Title == "Root").IgnoreQueryFilters().FirstOrDefaultAsync();
            }

            var newGroup = new AppGroup
            {
                Title = request.Title,
                Description = request.Description,
                Parent = parent,
                Urls = new List<AppGroupUrl>()
            };

            parent.Children.Add(newGroup);
            await _groupRepository.Save();
        }

        public async Task Handle(AssignGroupToUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                throw new AppException("Invalid User");
            var group = await _groupRepository.FindByCondition(i => i.Id == request.GroupId).FirstOrDefaultAsync();
            if (group == null)
                throw new AppException("Invalid Group");
            var newRelation = new AppGroupUser()
            {
                UserId = user.Id,
                GroupId = group.Id,
            };
            group.Users.Add(newRelation);
            await _groupRepository.Save();
        }
    }
}
