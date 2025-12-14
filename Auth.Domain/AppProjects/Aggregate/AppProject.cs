using Auth.Domain._0.Base;
using Auth.Domain.AppProjects.Entity;
using Auth.Domain.AppRoles.Aggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Domain.AppProjects.Aggregate
{
    public class AppProject : EntityBase<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Host {  get; set; }
        public  string Port { get; set; }

        public virtual ICollection<AppUrl> Urls { get; set; }
   

    }
}
