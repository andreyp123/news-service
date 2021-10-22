using System.Threading;
using System.Threading.Tasks;

namespace NewsService
{
    public interface INewsService
    {
        Task PrepareRepositoryAsync(CancellationToken ct);
        Task HandleLatestNewsAsync(CancellationToken ct);
    }
}
