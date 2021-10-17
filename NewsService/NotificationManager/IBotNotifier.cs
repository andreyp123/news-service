using Common;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationManager
{
    public interface IBotNotifier
    {
        Task SendAsync(string message, CancellationToken ct);
        Task SendAsync(NewsItem newsItem, CancellationToken ct);
    }
}
