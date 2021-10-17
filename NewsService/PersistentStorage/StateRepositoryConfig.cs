using Common;
using Microsoft.Extensions.Configuration;

namespace PersistentStorage
{
    public class StateRepositoryConfig
    {
        private const string SECTION_NAME = nameof(StateRepository<object>);

        public string StateFilePath { get; set; }

        public StateRepositoryConfig()
        {
        }

        public StateRepositoryConfig(IConfiguration configuration)
        {
            Guard.NotNull(configuration, nameof(configuration));
            configuration.GetSection(SECTION_NAME).Bind(this);
        }
    }
}
