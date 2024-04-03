using Contracts;
using MassTransit;

namespace AuctionService;

public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
{
    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        System.Console.WriteLine("---> Consuming fault creation");

        var exception = context.Message.Exceptions.First();

        if (exception.ExceptionType == "System.Exception")
        {
            context.Message.Message.Model = "FooBar";
            await context.Publish(context.Message.Message);
        }
        else
        {
            System.Console.WriteLine("Not an argument exception");
        }
    }
}
