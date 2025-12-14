using Auth.Domain._0.Base;
using Auth.Domain.AppGroups.Aggregate;
using Auth.Domain.AppProjects.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Domain.AppGroups.Entity
{
    public class AppGroupUrl : EntityBase<Guid>
    {
        public Guid GroupId { get; set; }
        public virtual AppGroup Group { get; set; }
        public Guid UrlId { get; set; }
        public virtual AppUrl Url { get; set; }
    }
}
