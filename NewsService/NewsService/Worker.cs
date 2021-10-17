using Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsManager;
using NotificationManager;
using PersistentStorage;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NewsService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly WorkerConfig _config;
        private readonly INewsFeedReader _newsFeedReader;
        private readonly IStateRepository<NewsState> _stateRepository;
        private readonly IBotNotifier _botNotifier;

        public Worker(
            ILogger<Worker> logger,
            WorkerConfig config,
            INewsFeedReader newsFeedReader,
            IStateRepository<NewsState> stateRepository,
            IBotNotifier botNotifier)
        {
            _logger = logger;
            _config = config;
            _logger.LogInformation($"Configuration: {JsonSerializer.Serialize(_config)}");

            _newsFeedReader = newsFeedReader;
            _stateRepository = stateRepository;
            _botNotifier = botNotifier;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("Started worker.");

            while (!ct.IsCancellationRequested)
            {
                await HandleLatestNews(ct);

                _logger.LogInformation($"Sleeping {_config.CheckInterval.ToString("c")}...");
                await Task.Delay((int)_config.CheckInterval.TotalMilliseconds, ct);
            }
        }

        private async Task HandleLatestNews(CancellationToken ct)
        {
            NewsItem lastItem = null;
            try
            {
                _logger.LogInformation("Handling latest news...");
                var state = _stateRepository.GetState();
                _logger.LogInformation($"State: {state}");

                var newsItems = await _newsFeedReader.GetNewsAsync(state?.LastNewsItem?.Date, ct);
                _logger.LogInformation($"Loaded {newsItems.Count} news items");
                if (newsItems.Count == 0)
                {
                    _logger.LogInformation("Nothing to handle");
                    return;
                }

                for (int i = newsItems.Count - 1; i >= 0; i--)
                {
                    lastItem = newsItems[i];
                    await _botNotifier.SendAsync(lastItem, ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling news.");
            }
            finally
            {
                if (lastItem != null)
                {
                    var newState = NewsState.FromNewsItem(lastItem);
                    SetStateSafe(newState);
                }
            }
        }

        private void SetStateSafe(NewsState state)
        {
            try
            {
                _stateRepository.SetState(state);
                _logger.LogInformation($"New state: {state}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while setting state.");
            }
        }
    }
}
