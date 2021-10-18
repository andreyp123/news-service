using System.Threading;
using System.Threading.Tasks;

namespace PersistentStorage
{
    public interface IStateRepository<StateT>
        where StateT : class
    {
        Task<StateT> GetStateAsync(CancellationToken ct);
        Task SetStateAsync(StateT state, CancellationToken ct);
    }
}
