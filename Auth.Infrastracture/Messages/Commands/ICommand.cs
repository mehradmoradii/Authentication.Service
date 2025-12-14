using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Infrastracture.Messages.Commands
{
    public interface ICommand : IRequest { }
    public interface ICommand<TResponse> : IRequest<TResponse> where TResponse : class { }
}
