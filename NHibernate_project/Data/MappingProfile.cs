using AutoMapper;
using NHibernate_project.DTO;
using NHibernate_project.Models;

namespace NHibernate_project.Data;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Book, BookDto>();
        CreateMap<Chapter, ChapterDto>();
        CreateMap<Genre, GenreDto>();
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();
    }
}