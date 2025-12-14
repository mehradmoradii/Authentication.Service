

using Auth.Query.Dtos._0.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Query.Dtos.AppProjects
{
    public class AppUrlSimpleDto : EntityBaseDto<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
    }
}
