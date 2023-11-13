using AutoMapper;
using HandiPapi.DataAccess;
using HandiPapi.Models;

namespace HandiPapi.Configurations
{
    public class MapperInitializer: Profile
    {
        public MapperInitializer()
        {
            CreateMap<ApiUser, UserDto>().ReverseMap();
            CreateMap<ApiUser, LoginDto>().ReverseMap();
        }
    }
}
