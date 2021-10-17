using Common;
using Microsoft.Extensions.Logging.Abstractions;
using NotificationManager;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationManagerTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var botConfig = new BotNotifierConfig
                {
                    BotToken = "",
                    ChatId = 0
                };
                var botNotifier = new BotNotifier(NullLogger<BotNotifier>.Instance, botConfig);

                var newsItem = new NewsItem
                {
                    Title = "News title",
                    Date = new DateTime(2021, 10, 5, 16, 30, 0),
                    Url = "https://google.com"
                };
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

                await botNotifier.SendAsync(newsItem, cts.Token);

                Console.WriteLine("Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
