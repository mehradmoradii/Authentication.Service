using Auth.Infrastracture.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Command.Commands.AppUsers
{
    public class UpdateAppUserCommand : ICommand
    {
        //TODO : write its handler
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
    }
}
