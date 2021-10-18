using Common;
using Microsoft.Extensions.Configuration;
using System;

namespace NewsService
{
    public class NewsServiceWorkerConfig
    {
        private const string SECTION_NAME = nameof(NewsServiceWorker);

        public TimeSpan CheckInterval { get; set; }

        public NewsServiceWorkerConfig(IConfiguration configuration)
        {
            Guard.NotNull(configuration, nameof(configuration));
            configuration.GetSection(SECTION_NAME).Bind(this);
        }
    }
}
