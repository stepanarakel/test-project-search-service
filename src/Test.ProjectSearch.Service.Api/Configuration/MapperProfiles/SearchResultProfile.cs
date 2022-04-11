using AutoMapper;

public class SearchResultProfile : Profile
{
    public SearchResultProfile()
    {
        CreateMap<SearchResultRequestViewModel, SearchResult>();
    }
}