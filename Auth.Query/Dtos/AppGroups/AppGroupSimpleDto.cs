using Auth.Query.Dtos._0.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Query.Dtos.AppGroups
{
    public class AppGroupSimpleDto : EntityBaseDto<Guid>
    {
        public string Title { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<AppGroupSimpleDto> Children { get; set; }

    }
}
