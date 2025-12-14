using Auth.Domain.AppProjects.Aggregate;
using Auth.Domain.AppRoles.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Domain.AppRoles.Aggregate
{
    public class AppRole : IdentityRole<Guid>
    {
        public DateTime CreationDateTime { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<AppRoleUser> Users { get; set; }
     
    }
}
