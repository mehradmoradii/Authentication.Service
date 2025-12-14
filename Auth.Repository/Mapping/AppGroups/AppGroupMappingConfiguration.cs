using Auth.Domain.AppGroups.Aggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Repository.Mapping.AppGroups
{
    public class AppGroupMappingConfiguration : IEntityTypeConfiguration<AppGroup>
    {
        public void Configure(EntityTypeBuilder<AppGroup> builder)
        {
            builder.HasOne(p=>p.Parent).WithMany(c=>c.Children).HasForeignKey(p=>p.ParentId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(u => u.Urls).WithOne(g => g.Group).OnDelete(DeleteBehavior.Cascade);
            builder.HasQueryFilter(i => i.Title != "Root");
        }
    }
}
