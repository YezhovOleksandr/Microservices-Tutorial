﻿using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchEngine.Models;

namespace SearchService;

public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
{
    private readonly IMapper _mapper;

    public AuctionDeletedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }
    public async Task Consume(ConsumeContext<AuctionDeleted> context)
    {
        var item = _mapper.Map<Item>(context.Message);

        await item.SaveAsync();
    }
}
