using Auth.Infrastracture.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Command.Commands.AppRoles
{
    public class AssignRoleToUserCommand : ICommand
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
