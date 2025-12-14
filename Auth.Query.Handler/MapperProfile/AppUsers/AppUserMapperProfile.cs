using Auth.Domain.AppUsers.Aggregate;
using Auth.Query.Dtos.AppUsers;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Query.Handler.MapperProfile.AppUsers
{
    public class AppUserMapperProfile : Profile
    {
        public AppUserMapperProfile()
        {
            CreateMap<AppUser, AppUsersSimpleDto>();
        }
    }
}
