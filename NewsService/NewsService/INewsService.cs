using System.Threading;
using System.Threading.Tasks;

namespace NewsService
{
    public interface INewsService
    {
        Task HandleLatestNewsAsync(CancellationToken ct);
    }
}
