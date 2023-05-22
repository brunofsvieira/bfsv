using AutoMapper;
using bfsv.Dtos;
using bfsv.Models;
using bfsv.ServiceModels;

namespace bfsv.Mappers
{
    public class ModelsProfile : Profile
    {
        public ModelsProfile() 
        {
            this.CreateMap<UserDto, User>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.InsertDate, opt => opt.Ignore())
                .ForMember(d => d.UpdateDate, opt => opt.Ignore());

            this.CreateMap<User, UserResponse>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id));

            this.CreateMap<EntityDto, Entity>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.InsertDate, opt => opt.Ignore())
                .ForMember(d => d.UpdateDate, opt => opt.Ignore());

            this.CreateMap<Entity, EntityResponse>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id));

            this.CreateMap<CommentDto, Comment>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(d => d.EntityId, opt => opt.MapFrom(src => src.EntityId))
                .ForMember(d => d.InsertDate, opt => opt.Ignore())
                .ForMember(d => d.UpdateDate, opt => opt.Ignore());

            this.CreateMap<Comment, CommentResponse>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(d => d.EntityId, opt => opt.MapFrom(src => src.EntityId));
        }
    }
}
