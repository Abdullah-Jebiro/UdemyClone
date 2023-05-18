using AutoMapper;
using Models.DbEntities;
using Models.Dtos;
using Models.Dtos.Account;
using Models.Identity;

namespace WebApi.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<ApplicationUser, UserwithoutRoleDto>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.UserName))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email));

            CreateMap<ApplicationUser, UserForUpdateDto>();


            CreateMap<Video, VideoDto>();
            CreateMap<Video, VideoInfoDto>();
            CreateMap<VideoForCreateDto, Video>();
            CreateMap<Video, VideoForUpdateDto>();



            CreateMap<Course, CourseDto>()
                .ForMember(d => d.Language, o => o.MapFrom(s => s.Language.Name))
                .ForMember(d => d.Level, o => o.MapFrom(s => s.Level.Name))
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User.UserName));
            CreateMap<Course, CourseForInstructorDto>()
                .ForMember(d => d.Language, o => o.MapFrom(s => s.Language.Name))
                .ForMember(d => d.Level, o => o.MapFrom(s => s.Level.Name))
                .ForMember(d => d.Category, o => o.MapFrom(s => s.Category.nameCategory))
                .ForMember(d => d.StudentsCount, o => o.MapFrom(s => s.UserCourses.Count()));
            CreateMap<CourseForCreateDto, Course>().ReverseMap();
            CreateMap<CourseForUpdateDto, Course>();


            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryForEditDto,Category>();

            CreateMap<Language, LanguageDto>();
            CreateMap<CategoryForEditDto, Category>();

            CreateMap<Level, LevelDto>();
            CreateMap<CategoryForEditDto, Category>();


            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Course.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Course.Price))
                .ForMember(dest => dest.About, opt => opt.MapFrom(src => src.Course.About))
                .ForMember(dest => dest.ThumbnailUrl , opt => opt.MapFrom(src => src.Course.ThumbnailUrl));

        }
    }
}