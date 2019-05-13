using AutoMapper;
using DatingAppBack.Dtos;
using DatingAppBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppBack.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {   //CreateMap<source, destination>();
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl,
                           opt =>
                           {
                               opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                           })
                .ForMember(dest => dest.Age,
                           opt => {
                               opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
                           });

            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl,
                           opt =>
                           {
                               opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                           })
               .ForMember(dest => dest.Age,
                           opt => {
                               opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
                           });

            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotosForDetailedDto>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
        }
    }
}
