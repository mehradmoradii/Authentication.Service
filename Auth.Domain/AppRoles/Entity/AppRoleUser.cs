using Auth.Domain.AppRoles.Aggregate;
using Auth.Domain.AppUsers.Aggregate;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Domain.AppRoles.Entity
{
    public class AppRoleUser : IdentityUserRole<Guid>
    {
        public DateTime CreationDateTime { get; set; }
        //public virtual AppRole Role { get; set; }
        //public virtual AppUser User { get; set; }

    }
}
