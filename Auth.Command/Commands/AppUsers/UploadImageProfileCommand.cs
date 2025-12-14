using Auth.Infrastracture.Messages.Commands;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Command.Commands.AppUsers
{
    public class UploadImageProfileCommand : ICommand
    {
        public Guid? UserId { get; set; } // The user whose profile is updated
        public IFormFile? ProfileImage { get; set; } = null!;
    }
}
