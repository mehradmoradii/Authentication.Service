using Auth.Infrastracture.Messages.Queries;
using Auth.Query.Dtos.AppProjects;
using Auth.Query.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Query.Queries.AppProjects
{
    public class GetUrlsByProjectIdQuery : IQuery<PagedResult<AppUrlSimpleDto>>
    {
        public Guid ProjectId { get; set; }
        public AppUrlFilter UrlFilter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
