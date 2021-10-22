using AutoMapper;
using Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersistentStorage.Model;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PersistentStorage
{
    public class NewsRepository : INewsRepository
    {
        private readonly ILogger<NewsRepository> _logger;
        private readonly NewsRepositoryConfig _config;
        private readonly IMapper _mapper;

        public NewsRepository(
            ILogger<NewsRepository> logger,
            NewsRepositoryConfig config)
        {
            _logger = logger;
            _config = config;
            _logger.LogInformation($"Configuration: {JsonSerializer.Serialize(_config)}");

            _mapper = CreateMapper();
        }

        public async Task CreateNewsItemAsync(NewsItem newsItem, CancellationToken ct)
        {
            Guard.NotNull(newsItem, nameof(newsItem));

            using (var context = CreteDbContext())
            {
                await context.NewsItems.AddAsync(_mapper.Map<NewsItemEntity>(newsItem), ct);
                await context.SaveChangesAsync(ct);
            }
        }

        public async Task<NewsItem> GetLastNewsItemAsync(CancellationToken ct)
        {
            using (var context = CreteDbContext())
            {
                var entity = await context.NewsItems
                    .OrderByDescending(nie => nie.Id)
                    .FirstOrDefaultAsync();
                return _mapper.Map<NewsItem>(entity);
            }
        }

        public async Task<int> GetNewsItemsCountAsync(CancellationToken ct)
        {
            using (var context = CreteDbContext())
            {
                return await context.NewsItems.CountAsync(ct);
            }
        }

        public async Task<(NewsItem[], int)> GetNewsItemsAsync(int start, int size, CancellationToken ct)
        {
            using (var context = CreteDbContext())
            {
                int total = await context.NewsItems.CountAsync(ct);

                NewsItem[] newsItems = await context.NewsItems
                    .OrderByDescending(nie => nie.Date)
                    .Skip(start)
                    .Take(size)
                    .Select(nie => _mapper.Map<NewsItem>(nie))
                    .ToArrayAsync(ct);

                return (newsItems, total);
            }
        }

        public async Task MigrateAsync(CancellationToken ct)
        {
            using (var context = CreteDbContext())
            {
                await context.Database.MigrateAsync(ct);
            }
        }

        private NewsDbContext CreteDbContext()
        {
            return new NewsDbContext(_config.ConnectionString);
        }

        private IMapper CreateMapper()
        {
            var mapConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<NewsItem, NewsItemEntity>();
                cfg.CreateMap<NewsItemEntity, NewsItem>();
            });
            return mapConfig.CreateMapper();
        }
    }
}
