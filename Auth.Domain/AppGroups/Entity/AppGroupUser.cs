using Auth.Domain._0.Base;
using Auth.Domain.AppGroups.Aggregate;
using Auth.Domain.AppUsers.Aggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Domain.AppGroups.Entity
{
    public class AppGroupUser : EntityBase<Guid>
    {
        public virtual AppGroup Group { get; set; }
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
        public virtual AppUser User { get; set; }

    }
}
