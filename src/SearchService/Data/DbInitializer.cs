using MongoDB.Driver;
using MongoDB.Entities;
using SearchEngine.Models;
using SearchEngine.Services;
using System.Text.Json;

namespace SearchEngine.Data
{
    public class DbInitializer
    {
        public static async Task InitDb(WebApplication application)
        {
            await DB.InitAsync("SearchDb", MongoClientSettings
                .FromConnectionString(application.Configuration.GetConnectionString("MongoDbConnection")));
            await DB.Index<Item>()
                .Key(x => x.Make, KeyType.Text)
                .Key(x => x.Model, KeyType.Text)
                .Key(x => x.Color, KeyType.Text)
                .CreateAsync();

            var count = await DB.CountAsync<Item>();

            using var scope = application.Services.CreateScope();

            var httpClient = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();

            var items = await httpClient.GetItemsForSearchDbAsync();

            await Console.Out.WriteLineAsync(items.Count + " returned from auction service");

            if (items.Count > 0) await DB.SaveAsync(items);
        }
    }
}
