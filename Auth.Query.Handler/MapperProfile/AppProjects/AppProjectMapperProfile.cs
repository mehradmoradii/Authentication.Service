using Auth.Domain.AppProjects.Aggregate;
using Auth.Domain.AppProjects.Entity;
using Auth.Query.Dtos.AppProjects;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Query.Handler.MapperProfile.AppProjects
{
    public class AppProjectMapperProfile : Profile
    {
        public AppProjectMapperProfile()
        {
            CreateMap<AppProject, AppProjectSimpleDto>();
            CreateMap<AppUrl, AppUrlSimpleDto>();
            CreateMap<AppUrl, UrlsDto>();
        }
    }
}
