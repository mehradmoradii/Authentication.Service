using Auth.Domain._0.Base;
using Auth.Domain.AppUsers.Aggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Domain.AppUsers.Entity
{
    public class AppRefreshToken : EntityBase<Guid>
    {
        public string Token { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime Expiration { get; set; }

        public Guid UserId { get; set; }
        public virtual AppUser User { get; set; }
    }
}
