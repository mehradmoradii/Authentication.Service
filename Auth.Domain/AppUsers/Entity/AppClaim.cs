using Auth.Domain.AppUsers.Aggregate;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Domain.AppUsers.Entity
{
    public class AppClaim : IdentityUserClaim<Guid>
    {
        public DateTime CreationDateTime { get; set; }
        //public virtual AppUser User { get; set; }
    }
}
