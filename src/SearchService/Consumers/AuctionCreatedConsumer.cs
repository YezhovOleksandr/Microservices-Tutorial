using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchEngine.Models;

namespace SearchService.Consumers;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    private readonly IMapper _mapper;

    public AuctionCreatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }
    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        System.Console.WriteLine($" --> Consuming auction created : {context.Message.Id}");

        var item = _mapper.Map<Item>(context.Message);

        if (item.Model == "Foo") throw new Exception("Cannot sell cars with name Foo");

        await item.SaveAsync();
    }
}
