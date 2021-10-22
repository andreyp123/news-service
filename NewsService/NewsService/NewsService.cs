using Common;
using Microsoft.Extensions.Logging;
using NewsManager;
using NotificationManager;
using PersistentStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NewsService
{
    public class NewsService : INewsService
    {
        private readonly ILogger<NewsService> _logger;
        private readonly INewsFeedReader _newsFeedReader;
        private readonly INewsRepository _newsRepository;
        private readonly IBotNotifier _botNotifier;

        public NewsService(
            ILogger<NewsService> logger,
            INewsFeedReader newsFeedReader,
            INewsRepository newsRepository,
            IBotNotifier botNotifier)
        {
            _logger = logger;
            _newsFeedReader = newsFeedReader;
            _newsRepository = newsRepository;
            _botNotifier = botNotifier;
        }

        public async Task PrepareRepositoryAsync(CancellationToken ct)
        {
            try
            {
                await _newsRepository.MigrateAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while preparing news repository.");
                throw;
            }
        }

        public async Task HandleLatestNewsAsync(CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Handling news...");

                var lastNewsItem = await _newsRepository.GetLastNewsItemAsync(ct);
                _logger.LogInformation($"Last news: {lastNewsItem}");

                var newsItems = await _newsFeedReader.GetNewsAsync(lastNewsItem?.Date, ct);
                _logger.LogInformation($"Loaded {newsItems.Count} news items");

                foreach (var newsItem in ((IEnumerable<NewsItem>)newsItems).Reverse())
                {
                    await _newsRepository.CreateNewsItemAsync(newsItem, ct);
                    await _botNotifier.SendAsync(newsItem, ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling news.");
            }
        }
    }
}
