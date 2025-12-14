using Auth.Domain.AppGroups.Aggregate;
using Auth.Domain.AppGroups.Entity;
using Auth.Query.Dtos.AppGroups;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Query.Handler.MapperProfile.AppGroups
{
    public class AppGroupMapperProfile : Profile
    {
        public AppGroupMapperProfile()
        {
            CreateMap<AppGroup, AppGroupSimpleDto>();

     
        }
    }
}
