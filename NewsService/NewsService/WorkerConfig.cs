using Common;
using Microsoft.Extensions.Configuration;
using System;

namespace NewsService
{
    public class WorkerConfig
    {
        private const string SECTION_NAME = nameof(Worker);

        public TimeSpan CheckInterval { get; set; }

        public WorkerConfig(IConfiguration configuration)
        {
            Guard.NotNull(configuration, nameof(configuration));
            configuration.GetSection(SECTION_NAME).Bind(this);
        }
    }
}
