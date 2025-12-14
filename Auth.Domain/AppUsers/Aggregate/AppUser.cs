using Auth.Domain.AppGroups.Entity;
using Auth.Domain.AppRoles.Entity;
using Auth.Domain.AppUsers.Entity;
using Auth.Infrastracture.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Domain.AppUsers.Aggregate
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public string? NationalId { get; set; }
        public bool IsConfirm { get; set; }
        public bool IsAdmin { get; set; } = false;
        public bool IsDeleted { get; set; }
        public UserState Status { get; set; } = UserState.Pendding;
        public DateTime CreationDateTime { get; set; }

        public Guid ProfileId { get; set; }
        public virtual AppProfileImage Profile { get; set; }
        public virtual ICollection<AppLogin>? Logins { get; set; }
        public virtual ICollection<AppRefreshToken>? RefreshToken { get; set; }
        public virtual ICollection<AppClaim>? Claims { get; set; }
        public virtual ICollection<AppRoleUser>? Roles { get; set; }
        public virtual ICollection<AppGroupUser>? Groups { get; set; }
    }
}
