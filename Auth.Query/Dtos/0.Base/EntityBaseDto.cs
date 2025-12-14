using MD.PersianDateTime.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Query.Dtos._0.Base
{
    public class EntityBaseDto<Tkey>
    {
        public Tkey Id { get; set; }
        public DateTime CreationDateTime { get; set; }
        public string PersianDateTimeString => new PersianDateTime(CreationDateTime).ToShortDateTimeString();



    }
}
