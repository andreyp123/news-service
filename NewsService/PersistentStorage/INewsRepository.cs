using Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PersistentStorage
{
    public interface INewsRepository
    {
        Task<NewsItem> GetLastNewsItemAsync(CancellationToken ct);
        Task CreateNewsItemAsync(NewsItem newsItem, CancellationToken ct);
        Task<int> GetNewsItemsCountAsync(CancellationToken ct);
        Task<(NewsItem[], int)> GetNewsItemsAsync(int start, int size, CancellationToken ct);

        Task MigrateAsync(CancellationToken ct);
    }
}
