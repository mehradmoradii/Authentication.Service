using Auth.Domain._0.Base;
using Auth.Domain.AppUsers.Aggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Domain.AppUsers.Entity
{
    public class AppProfileImage : EntityBase<Guid>
    {
        public Guid UserId { get; set; } // Foreign key to AppUser
        public string? DisplayName { get; set; } // Optional display name
        public string? ProfileImageUrl { get; set; } // URL to the profile image (could be a path or cloud URL)

        // Navigation property
        public virtual AppUser User { get; set; } = null!;
    }
}

