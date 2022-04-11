using AutoMapper;
using Test.ProjectSearch.Models.Api;

public class RepositoryProfile : Profile
{
    public RepositoryProfile()
    {
        CreateMap<Repository, RepositoryViewModel>()
            .ForMember(dst => dst.Autor, opt => opt.MapFrom(src => src.Owner.Login))
            .ForMember(dst => dst.Stargazers, opt => opt.MapFrom(src => src.StargazersCount))
            .ForMember(dst => dst.Watchers, opt => opt.MapFrom(src => src.WatchersCount));

        CreateMap<RepositoryRequestViewModel, RepositoryViewModel>()
            .ForMember(dst => dst.Autor, opt => opt.MapFrom(src => src.Owner.Login))
            .ForMember(dst => dst.Stargazers, opt => opt.MapFrom(src => src.StargazersCount))
            .ForMember(dst => dst.Watchers, opt => opt.MapFrom(src => src.WatchersCount))
            .ForMember(dst => dst.Url, opt => opt.MapFrom(src => src.HtmlUrl));

        CreateMap<RepositoryRequestViewModel, Repository>()
            .ForMember(dst => dst.Url, opt => opt.MapFrom(src => src.HtmlUrl)); ;

        CreateMap<OwnerRequestViewModel, Owner>();
    }
}