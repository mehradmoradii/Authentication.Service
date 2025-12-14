using Auth.Domain._0.Base;
using Auth.Query.Dtos._0.Base;
using AutoMapper;
using MD.PersianDateTime.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Query.Handler.MapperProfile._0.Base
{
    public class EntityBaseMapperProfile : Profile
    {
        public EntityBaseMapperProfile()
        {
            CreateMap(typeof(EntityBase<>), typeof(EntityBaseDto<>));
        }
    }
}
