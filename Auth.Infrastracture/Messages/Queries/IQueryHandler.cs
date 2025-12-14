using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Infrastracture.Messages.Queries
{
    public interface IQueryHandler<TQuery> : IRequestHandler<TQuery> where TQuery : IQuery { }
    public interface IQueryHandler<TQuery, TDto> : IRequestHandler<TQuery, TDto> where TQuery : IQuery<TDto> where TDto : class { }
}
