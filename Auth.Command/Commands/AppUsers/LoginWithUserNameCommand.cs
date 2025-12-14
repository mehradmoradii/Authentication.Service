using Auth.Command.Responses.AppUsers;
using Auth.Infrastracture.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Command.Commands.AppUsers
{
    public class LoginWithUserNameCommand : ICommand<LoginUserCommandResponse>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? UserAgent { get; set; }
    }
}
