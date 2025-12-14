using Auth.Command.Commands.AppRoles;
using Auth.Domain.AppProjects.Aggregate;
using Auth.Domain.AppRoles.Aggregate;
using Auth.Domain.AppRoles.Entity;
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

namespace Auth.Command.Handler.AppRoles
{
    public class AppRoleCommandHandler :
        ICommandHandler<CreateAppRoleCommand>,
        ICommandHandler<AssignRoleToUserCommand>
    {
        private readonly IRepositoryBase<AppRole, Guid> _roleRepository;
        private readonly UserManager<AppUser> _userManager;
        
        public AppRoleCommandHandler(UserManager<AppUser> userManager, IRepositoryBase<AppRole, Guid> roleRepository)
        {
            _userManager = userManager;
            _roleRepository = roleRepository;
        }
        public async Task Handle(CreateAppRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.FindByCondition(n=>n.Name == request.Name).FirstOrDefaultAsync();
            if (role != null)
                throw new AppException("Name is duplicated");
            var newRole = new AppRole()
            {
                Name = request.Name,
                Description = request.Description,
                CreationDateTime = DateTime.Now,
            };
            await _roleRepository.Create(newRole);
            await _roleRepository.Save();
        }

        public async Task Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                throw new AppException("invalid User");
            var role = await _roleRepository.FindByCondition(i=>i.Id == request.RoleId).FirstOrDefaultAsync();
            if (role == null)
                throw new AppException("invalid role");
            var newRelation = new AppRoleUser()
            {
                RoleId = role.Id,
                UserId = user.Id,
                CreationDateTime = DateTime.Now
            };
            role.Users.Add(newRelation);
            await _roleRepository.Save();
        }
    }
}
