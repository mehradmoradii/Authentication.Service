using Auth.Infrastracture.Messages.Queries;
using Auth.Query.Dtos.AppUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Query.Queries.AppUsers
{
    public class GetUsersByGroupIdQuery : IQuery<List<AppUsersSimpleDto>>
    {
        public Guid GroupId { get; set; }
    }
}
