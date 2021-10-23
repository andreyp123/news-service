using Common;
using Microsoft.Extensions.Logging.Abstractions;
using NotificationManager;
using System;
using System.Linq;
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
                await BotNotifierTest();
                //await RateLimitTest();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static async Task BotNotifierTest()
        {
            var botConfig = new BotNotifierConfig
            {
                BotToken = "",
                ChatId = 0,
                LimitPerSec = 2
            };
            var botNotifier = new BotNotifier(NullLogger<BotNotifier>.Instance, botConfig);

            var newsItem = new NewsItem
            {
                Title = "News title",
                Date = new DateTime(2021, 10, 5, 16, 30, 0),
                Url = "https://google.com"
            };
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            //await botNotifier.SendAsync(newsItem, cts.Token);
            await Task.WhenAll(Enumerable.Range(0, 5).Select(_ => botNotifier.SendAsync(newsItem, cts.Token)));

            Console.WriteLine("Success");
        }

        static async Task RateLimitTest()
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            var sender = new MessageSenderWithLimit(5);

            Console.WriteLine("Parallel");
            await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => sender.SendMessage(cts.Token)));

            Console.WriteLine();
            Console.WriteLine("Sequence");
            foreach (var i in Enumerable.Range(0, 20))
            {
                await sender.SendMessage(cts.Token);
            }
        }
    }

    class MessageSenderWithLimit
    {
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private int _waitIntervalMsec;

        public MessageSenderWithLimit(int limitPerSec)
        {
            _waitIntervalMsec = 1000 / limitPerSec;
        }

        public async Task SendMessage(CancellationToken ct)
        {
            await _semaphore.WaitAsync(ct);
            try
            {
                Console.WriteLine($"{DateTime.Now.ToString("o")} - {Thread.CurrentThread.ManagedThreadId} - Sent message - wait {_waitIntervalMsec}");
            }
            finally
            {
                await Task.Delay(_waitIntervalMsec, ct);
                _semaphore.Release();
            }
        }
    }
}
