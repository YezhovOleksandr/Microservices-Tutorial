﻿using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    private readonly AuctionDbContext _dbcontext;

    public BidPlacedConsumer(AuctionDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }
    
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        System.Console.WriteLine("----> Consuming Bid placed");
        var auction = await _dbcontext.Auctions.FindAsync(context.Message.AuctionId);

        if (auction.CurrentHighBib == null || context.Message.BidStatus.Contains("Accepted")
        && context.Message.Amount > auction.CurrentHighBib)
        {
            auction.CurrentHighBib = context.Message.Amount;
            await _dbcontext.SaveChangesAsync();
        }

    }
}