using Auth.Infrastracture.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Command.Commands.AppRoles
{
    public class CreateAppRoleCommand : ICommand
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
