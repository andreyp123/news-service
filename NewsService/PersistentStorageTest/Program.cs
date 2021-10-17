using Common;
using Microsoft.Extensions.Logging.Abstractions;
using PersistentStorage;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace PersistentStorageTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new StateRepositoryConfig
            {
                StateFilePath = "./state.txt"
            };
            var repository = new StateRepository<NewsState>(NullLogger<StateRepository<NewsState>>.Instance, config);

            var states = new List<NewsState>();
            states.Add(repository.GetState());
            states.Add(repository.GetState());
            repository.SetState(null);
            states.Add(repository.GetState());
            repository.SetState(NewsState.Empty);
            states.Add(repository.GetState());
            repository.SetState(NewsState.FromNewsItem((NewsItem)null));
            states.Add(repository.GetState());
            repository.SetState(NewsState.FromNewsItem(new NewsItem { Title = "title", Url = "url", Date = DateTime.Now }));
            states.Add(repository.GetState());

            foreach (var state in states)
            {
                Console.WriteLine(JsonSerializer.Serialize(state));
            }
        }
    }
}
