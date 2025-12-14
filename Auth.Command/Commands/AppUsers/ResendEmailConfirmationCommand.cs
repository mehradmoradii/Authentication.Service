using Auth.Infrastracture.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Command.Commands.AppUsers
{
    public class ResendEmailConfirmationCommand : ICommand
    {
        public string Email { get; set; }
        public string? UserAgent { get; set; }
    }
}
