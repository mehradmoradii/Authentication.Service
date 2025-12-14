using Auth.Infrastracture.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Command.Commands.AppUsers
{
    public class LogoutUserCommand : ICommand
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? IpAddress { get; set; }
    }
}
