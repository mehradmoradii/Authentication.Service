using Auth.Infrastracture.Messages.Queries;
using Auth.Query.Dtos.AppProjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Query.Queries.AppUsers
{
    public class GetAllUsersUrlQuery : IQuery<List<UrlsDto>>
    {
        public Guid UserId { get; set; }
    }
}
