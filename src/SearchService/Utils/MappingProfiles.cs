using AutoMapper;
using Contracts;
using SearchEngine.Models;

namespace SearchService;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<AuctionCreated, Item>();
        CreateMap<AuctionUpdated, Item>();
        CreateMap<AuctionDeleted, Item>();
    }
}
