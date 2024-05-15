using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchEngine.Models;

namespace SearchService;

public class BidPlacedConsumer : IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        System.Console.WriteLine("---> Consuming bid placed");

        var auction = await DB.Find<Item>().OneAsync(Guid.Parse(context.Message.AuctionId));

        if (context.Message.BidStatus.Contains("Accepted") && context.Message.Amount > auction.CurrentHighBib)
        {
            auction.CurrentHighBib = context.Message.Amount;
            await auction.SaveAsync();
        }
    }
}
