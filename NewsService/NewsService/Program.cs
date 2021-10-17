using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewsManager;
using NotificationManager;
using PersistentStorage;

namespace NewsService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<WorkerConfig>();
                    services.AddSingleton<NewsFeedReaderConfig>();
                    services.AddSingleton<INewsFeedReader, NewsFeedReader>();
                    services.AddSingleton<StateRepositoryConfig>();
                    services.AddSingleton<IStateRepository<NewsState>, StateRepository<NewsState>>();
                    services.AddSingleton<BotNotifierConfig>();
                    services.AddSingleton<IBotNotifier, BotNotifier>();
                });
    }
}
