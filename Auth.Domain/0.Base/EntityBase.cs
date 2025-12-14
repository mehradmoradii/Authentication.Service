using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Domain._0.Base
{
    public abstract class EntityBase<Tkey> 
    {
        public Tkey Id { get; set; }
        public DateTime CreationDateTime { get; set; }
    }
}
