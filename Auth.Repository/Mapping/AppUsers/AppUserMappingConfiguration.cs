using Auth.Domain.AppUsers.Aggregate;
using Auth.Domain.AppUsers.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Repository.Mapping.AppUsers
{
    public class AppUserMappingConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasMany<AppClaim>(c => c.Claims).WithOne().HasForeignKey(u => u.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(u => u.Logins).WithOne(l => l.User).HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(p => p.Profile).WithOne(u => u.User).HasForeignKey<AppProfileImage>(u => u.UserId).OnDelete(DeleteBehavior
                .Cascade);
        }
    }
}
