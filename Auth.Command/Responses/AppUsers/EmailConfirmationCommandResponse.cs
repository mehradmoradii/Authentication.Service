using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Command.Responses.AppUsers
{
    public class EmailConfirmationCommandResponse
    {
        public bool IsConfirmed { get; set; }
        public string? Token { get; set; }
    }
}
