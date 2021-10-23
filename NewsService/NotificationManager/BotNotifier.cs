using Common;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NotificationManager
{
    public class BotNotifier : IBotNotifier
    {
        private readonly ILogger<BotNotifier> _logger;
        private readonly BotNotifierConfig _config;
        private readonly ChatId _chatId;
        private readonly TelegramBotClient _botClient;
        private readonly SemaphoreSlim _semaphore;
        private readonly int _waitIntervalMsec;

        public BotNotifier(ILogger<BotNotifier> logger, BotNotifierConfig config)
        {
            _logger = logger;
            _config = config;
            _logger.LogInformation($"Configuration: {JsonSerializer.Serialize(_config)}");

            _chatId = new ChatId(_config.ChatId);
            _botClient = new TelegramBotClient(_config.BotToken);
            _semaphore = new SemaphoreSlim(1, 1);
            _waitIntervalMsec = 1000 / (_config.LimitPerSec > 0 ? _config.LimitPerSec : 1);
        }

        public async Task SendAsync(string message, CancellationToken ct)
        {
            await _semaphore.WaitAsync(ct);
            try
            {
                await _botClient.SendTextMessageAsync(
                    chatId: _chatId,
                    text: message,
                    parseMode: ParseMode.Markdown,
                    cancellationToken: ct);
                _logger.LogInformation($"Sent message:{Environment.NewLine}{message}");
            }
            finally
            {
                await Task.Delay(_waitIntervalMsec);
                _semaphore.Release();
            }
        }

        public async Task SendAsync(NewsItem newsItem, CancellationToken ct)
        {
            await SendAsync(FormatNewsMessage(newsItem), ct);
        }

        private string FormatNewsMessage(NewsItem newsItem)
        {
            return new StringBuilder()
                .AppendLine($"*{newsItem.Title}*")
                .AppendLine(newsItem.Date.ToString("g", CultureInfo.GetCultureInfo("ru-RU")))
                .AppendLine(newsItem.Url)
                .ToString();
        }
    }
}
