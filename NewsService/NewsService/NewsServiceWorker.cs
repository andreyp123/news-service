using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NewsService
{
    public class NewsServiceWorker : BackgroundService
    {
        private readonly ILogger<NewsServiceWorker> _logger;
        private readonly NewsServiceWorkerConfig _config;
        private readonly INewsService _newsService;

        public NewsServiceWorker(
            ILogger<NewsServiceWorker> logger,
            NewsServiceWorkerConfig config,
            INewsService newsService)
        {
            _logger = logger;
            _config = config;
            _logger.LogInformation($"Configuration: {JsonSerializer.Serialize(_config)}");

            _newsService = newsService;
        }

        public override async Task StartAsync(CancellationToken ct)
        {
            _logger.LogInformation($"Staring worker...");

            await _newsService.PrepareRepositoryAsync(ct);
            await base.StartAsync(ct);
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await _newsService.HandleLatestNewsAsync(ct);

                _logger.LogInformation($"Sleeping {_config.CheckInterval.ToString("c")}...");
                await Task.Delay((int)_config.CheckInterval.TotalMilliseconds, ct);
            }
        }
    }
}
