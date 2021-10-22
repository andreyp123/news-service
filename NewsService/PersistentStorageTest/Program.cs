using Common;
using Microsoft.Extensions.Logging.Abstractions;
using PersistentStorage;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PersistentStorageTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                await TestNewsRepository();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static async Task TestNewsRepository()
        {
            var config = new NewsRepositoryConfig
            {
                ConnectionString = "Data Source=./news.db"
            };
            var repository = new NewsRepository(NullLogger<NewsRepository>.Instance, config);
            var ct = CancellationToken.None;

            await repository.MigrateAsync(ct);

            var items = new List<NewsItem>();
            items.Add(await repository.GetLastNewsItemAsync(ct));
            await repository.CreateNewsItemAsync(new NewsItem { Title = "a", Date = DateTime.UtcNow }, ct);
            items.Add(await repository.GetLastNewsItemAsync(ct));
            await repository.CreateNewsItemAsync(new NewsItem { Title = "b", Date = DateTime.UtcNow }, ct);
            items.Add(await repository.GetLastNewsItemAsync(ct));
            await repository.CreateNewsItemAsync(new NewsItem { Title = "c", Date = DateTime.UtcNow }, ct);
            await repository.CreateNewsItemAsync(new NewsItem { Title = "d", Date = DateTime.UtcNow }, ct);
            items.Add(await repository.GetLastNewsItemAsync(ct));

            foreach (var item in items)
            {
                Console.WriteLine(JsonSerializer.Serialize(item));
            }

            Console.WriteLine(await repository.GetNewsItemsCountAsync(ct));
            (NewsItem[] newsItems, int total) = await repository.GetNewsItemsAsync(0, int.MaxValue, ct);
            foreach (var item in newsItems)
            {
                Console.WriteLine(JsonSerializer.Serialize(item));
            }
        }

        static async Task TestStateRepository()
        {
            var config = new StateRepositoryConfig
            {
                StateFilePath = "./state.txt"
            };
            var repository = new StateRepository<NewsState>(NullLogger<StateRepository<NewsState>>.Instance, config);

            var ct = CancellationToken.None;

            var states = new List<NewsState>();
            states.Add(await repository.GetStateAsync(ct));
            states.Add(await repository.GetStateAsync(ct));
            await repository.SetStateAsync(null, ct);
            states.Add(await repository.GetStateAsync(ct));
            await repository.SetStateAsync(NewsState.Empty, ct);
            states.Add(await repository.GetStateAsync(ct));
            await repository.SetStateAsync(NewsState.FromNewsItem(null), ct);
            states.Add(await repository.GetStateAsync(ct));
            await repository.SetStateAsync(NewsState.FromNewsItem(new NewsItem { Title = "title", Url = "url", Date = DateTime.Now }), ct);
            states.Add(await repository.GetStateAsync(ct));

            foreach (var state in states)
            {
                Console.WriteLine(JsonSerializer.Serialize(state));
            }
        }
    }
}
