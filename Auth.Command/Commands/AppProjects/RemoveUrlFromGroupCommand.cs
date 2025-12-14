using Auth.Infrastracture.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Command.Commands.AppProjects
{
    public class RemoveUrlFromGroupCommand : ICommand
    {
        public Guid GroupId { get; set; }
        public List<Guid> Urls { get; set; }
    }
}
