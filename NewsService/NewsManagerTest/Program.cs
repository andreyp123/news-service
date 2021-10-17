using Microsoft.Extensions.Logging.Abstractions;
using NewsManager;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NewsManagerTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var newsConfig = new NewsFeedReaderConfig
                {
                    NewsUrl = "",
                    NewNewsLimit = 10,
                    ReadPagesLimit = 5
                };
                var newsReader = new NewsFeedReader(NullLogger<NewsFeedReader>.Instance, newsConfig);

                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                
                var newsItems = await newsReader.GetNewsAsync(new DateTime(2021, 10, 5, 16, 30, 0), cts.Token);
                foreach (var newsItem in newsItems)
                {
                    Console.WriteLine(newsItem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
