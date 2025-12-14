using Auth.Domain._0.Base;
using Auth.Domain.AppGroups.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Domain.AppGroups.Aggregate
{
    public class AppGroup  : EntityBase<Guid>
    {
        public string Title { get; set; }
        public string? Description { get; set; }

        public Guid? ParentId { get; set; }
        public virtual AppGroup? Parent { get; set; }
        public virtual ICollection<AppGroup> Children { get; set; }

        public virtual ICollection<AppGroupUser> Users { get; set; }
        public virtual ICollection<AppGroupUrl> Urls { get; set; } 
        
    }
}
