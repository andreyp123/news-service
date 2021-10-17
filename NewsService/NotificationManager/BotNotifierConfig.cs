using Common;
using Microsoft.Extensions.Configuration;

namespace NotificationManager
{
    public class BotNotifierConfig
    {
        private const string SECTION_NAME = nameof(BotNotifier);

        public string BotToken { get; set; }
        public long ChatId { get; set; }

        public BotNotifierConfig()
        {
        }

        public BotNotifierConfig(IConfiguration configuration)
        {
            Guard.NotNull(configuration, nameof(configuration));
            configuration.GetSection(SECTION_NAME).Bind(this);
        }
    }
}
