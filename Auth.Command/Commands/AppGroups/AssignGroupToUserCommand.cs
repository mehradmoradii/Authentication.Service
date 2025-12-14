using Auth.Infrastracture.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Command.Commands.AppGroups
{
    public class AssignGroupToUserCommand : ICommand
    {
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
    }
}
