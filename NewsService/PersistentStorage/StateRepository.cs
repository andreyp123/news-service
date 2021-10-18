using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PersistentStorage
{
    public class StateRepository<StateT> : IStateRepository<StateT>
        where StateT : class
    {
        private readonly ILogger<StateRepository<StateT>> _logger;
        private readonly StateRepositoryConfig _config;
        private StateT _cachedState = null;
        private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public StateRepository(
            ILogger<StateRepository<StateT>> logger,
            StateRepositoryConfig config)
        {
            _logger = logger;
            _config = config;
            _logger.LogInformation($"Configuration: {JsonSerializer.Serialize(_config)}");
        }

        public async Task<StateT> GetStateAsync(CancellationToken ct)
        {
            if (_cachedState == null)
            {
                await semaphore.WaitAsync();
                try
                {
                    _cachedState = await ReadStateAsync(ct);
                }
                finally
                {
                    semaphore.Release();
                }
            }
            return _cachedState;
        }

        public async Task SetStateAsync(StateT state, CancellationToken ct)
        {
            await semaphore.WaitAsync();
            try
            {
                await WriteStateAsync(state, ct);
                _cachedState = state;
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task<StateT> ReadStateAsync(CancellationToken ct)
        {
            StateT state = null;

            using var stream = File.Open(_config.StateFilePath, FileMode.OpenOrCreate);
            if (stream.Length > 0)
            {
                state = await JsonSerializer.DeserializeAsync<StateT>(stream, cancellationToken: ct);
            }

            return state;
        }

        private async Task WriteStateAsync(StateT state, CancellationToken ct)
        {
            using var stream = File.Create(_config.StateFilePath);
            if (state != null)
            {
                await JsonSerializer.SerializeAsync(stream, state, cancellationToken: ct);
            }
        }
    }
}
