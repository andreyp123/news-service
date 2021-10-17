using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;

namespace PersistentStorage
{
    public class StateRepository<StateT> : IStateRepository<StateT>
        where StateT : class
    {
        private readonly ILogger<StateRepository<StateT>> _logger;
        private readonly StateRepositoryConfig _config;
        private StateT _cachedState = null;
        private object _lockObj = new object();

        public StateRepository(ILogger<StateRepository<StateT>> logger, StateRepositoryConfig config)
        {
            _logger = logger;
            _config = config;
            _logger.LogInformation($"Configuration: {JsonSerializer.Serialize(_config)}");
        }

        public StateT GetState()
        {
            if (_cachedState == null)
            {
                lock (_lockObj)
                {
                    _cachedState = ReadState();
                }
            }
            return _cachedState;
        }

        public void SetState(StateT state)
        {
            lock (_lockObj)
            {
                WriteState(state);
                _cachedState = state;
            }
        }

        private StateT ReadState()
        {
            if (!File.Exists(_config.StateFilePath))
            {
                return null;
            }
            string stateStr = File.ReadAllText(_config.StateFilePath);
            if (string.IsNullOrEmpty(stateStr))
            {
                return null;
            }
            return JsonSerializer.Deserialize<StateT>(stateStr);
        }

        private void WriteState(StateT state)
        {
            string stateStr = state != null
                ? JsonSerializer.Serialize(state)
                : "";
            File.WriteAllText(_config.StateFilePath, stateStr);
        }
    }
}
