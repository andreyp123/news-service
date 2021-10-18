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
