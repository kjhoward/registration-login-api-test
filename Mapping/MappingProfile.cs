using AutoMapper;
using DevConsulting.RegistrationLoginApi.Models;
using RegistrationLoginApi.Data.DataModels;

namespace RegistrationLoginApi.Test.Mapping{
    public class MappingProfile{
        public static MapperConfiguration GetMapperConfiguration(){
            return new MapperConfiguration( opts => {
                opts.CreateMap<User, UserResource>();
                opts.CreateMap<UserResource, User>();
            });
        }
    }
}