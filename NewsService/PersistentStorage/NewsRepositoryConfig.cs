using Common;
using Microsoft.Extensions.Configuration;

namespace PersistentStorage
{
    public class NewsRepositoryConfig
    {
        private const string SECTION_NAME = nameof(NewsRepository);

        public string ConnectionString { get; set; }

        public NewsRepositoryConfig()
        {
        }

        public NewsRepositoryConfig(IConfiguration configuration)
        {
            Guard.NotNull(configuration, nameof(configuration));
            configuration.GetSection(SECTION_NAME).Bind(this);
        }
    }
}
