using Auth.Domain._0.Base;
using Auth.Domain.AppUsers.Aggregate;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Domain.AppUsers.Entity
{
    public class AppLogin : EntityBase<Guid>
    {
        public Guid UserId { get; set; }
        public string IP { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool IsActive { get; set; } 
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string? DisplayName { get; set; }

        public virtual AppUser User { get; set; }
    }

}
