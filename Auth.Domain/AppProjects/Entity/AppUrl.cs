using Auth.Domain._0.Base;
using Auth.Domain.AppGroups.Aggregate;
using Auth.Domain.AppGroups.Entity;
using Auth.Domain.AppProjects.Aggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Domain.AppProjects.Entity
{
    public class AppUrl : EntityBase<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }

        public Guid ProjectId {  get; set; }
        public virtual AppProject Project { get; set; }
        public virtual ICollection<AppGroupUrl> Groups { get; set; } 
    }
}
