using Common;
using Microsoft.Extensions.Configuration;

namespace NewsManager
{
    public class NewsFeedReaderConfig
    {
        private const string SECTION_NAME = nameof(NewsFeedReader);

        public string NewsUrl { get; set; }
        public int NewNewsLimit { get; set; }
        public int ReadPagesLimit { get; set; }

        public NewsFeedReaderConfig()
        {
        }

        public NewsFeedReaderConfig(IConfiguration configuration)
        {
            Guard.NotNull(configuration, nameof(configuration));
            configuration.GetSection(SECTION_NAME).Bind(this);
        }
    }
}
