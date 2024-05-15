using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchEngine.Models;

namespace SearchService;

public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
{
    public async Task Consume(ConsumeContext<AuctionFinished> context)
    {
        var auction = await DB.Find<Item>().OneAsync(Guid.Parse(context.Message.AuctionId));

        if (context.Message.ItemSold) 
        {
            auction.Winner = context.Message.Winner;
            auction.SoldAmount = context.Message.Amount.Value;
        }

        auction.Status = "Finished";
        await auction.SaveAsync();
    }
}
