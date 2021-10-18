using Common;
using Microsoft.Extensions.Logging;
using NewsManager;
using NotificationManager;
using PersistentStorage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NewsService
{
    public class NewsService : INewsService
    {
        private readonly ILogger<NewsService> _logger;
        private readonly INewsFeedReader _newsFeedReader;
        private readonly IStateRepository<NewsState> _stateRepository;
        private readonly IBotNotifier _botNotifier;

        public NewsService(
            ILogger<NewsService> logger,
            INewsFeedReader newsFeedReader,
            IStateRepository<NewsState> stateRepository,
            IBotNotifier botNotifier)
        {
            _logger = logger;
            _newsFeedReader = newsFeedReader;
            _stateRepository = stateRepository;
            _botNotifier = botNotifier;
        }

        public async Task HandleLatestNewsAsync(CancellationToken ct)
        {
            NewsItem lastSuccessfulItem = null;
            try
            {
                _logger.LogInformation("Handling latest news...");
                var state = await _stateRepository.GetStateAsync(ct);
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
                    await _botNotifier.SendAsync(newsItems[i], ct);
                    lastSuccessfulItem = newsItems[i];
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling news.");
            }
            finally
            {
                if (lastSuccessfulItem != null)
                {
                    await SafeUpdateStateAsync(lastSuccessfulItem, ct);
                }
            }
        }

        private async Task SafeUpdateStateAsync(NewsItem newsItem, CancellationToken ct)
        {
            try
            {
                var state = NewsState.FromNewsItem(newsItem);
                await _stateRepository.SetStateAsync(state, ct);
                _logger.LogInformation($"New state: {state}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while setting state.");
            }
        }
    }
}
