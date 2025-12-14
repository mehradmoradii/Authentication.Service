using Auth.Infrastracture.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Command.Commands.AppGroups
{
    public class CreateAppGroupCommand : ICommand
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public Guid? ParentId { get; set; }
    }
}
