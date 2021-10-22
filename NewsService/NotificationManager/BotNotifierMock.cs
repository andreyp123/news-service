using Common;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationManager
{
    public class BotNotifierMock : IBotNotifier
    {
        private ILogger<BotNotifierMock> _logger;

        public BotNotifierMock(ILogger<BotNotifierMock> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string message, CancellationToken ct)
        {
            _logger.LogInformation($"Sent message: {message}");
            return Task.CompletedTask;
        }

        public Task SendAsync(NewsItem newsItem, CancellationToken ct)
        {
            return SendAsync(JsonSerializer.Serialize(newsItem), ct);
        }
    }
}
