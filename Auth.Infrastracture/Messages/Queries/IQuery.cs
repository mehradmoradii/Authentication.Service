using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Infrastracture.Messages.Queries
{
    public interface IQuery : IRequest { }
    public interface IQuery<TDto> : IRequest<TDto> where TDto : class { }
}
