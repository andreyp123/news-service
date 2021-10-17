using Common;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace NewsManager
{
    public class NewsFeedReader : INewsFeedReader
    {
        private const string XPATH_NEWS_ITEM = "//div[contains(concat(' ', normalize-space(@class), ' '), ' news-item ')]";
        private const string XPATH_NEWS_ITEM_HEADER = "./a[contains(concat(' ', normalize-space(@class), ' '), ' news-item-header ')]";
        private const string XPATH_NEWS_ITEM_FOOTER_DATE = "./div[contains(concat(' ', normalize-space(@class), ' '), ' news-item-footer ')]/div/span";
        private static readonly IFormatProvider NEWS_DATE_CULTURE = CultureInfo.GetCultureInfo("ru-RU");

        private readonly ILogger<NewsFeedReader> _logger;
        private readonly NewsFeedReaderConfig _config;
        private readonly HtmlWeb _web;

        public NewsFeedReader(ILogger<NewsFeedReader> logger, NewsFeedReaderConfig config)
        {
            _logger = logger;
            _config = config;
            _logger.LogInformation($"Configuration: {JsonSerializer.Serialize(_config)}");

            _web = new HtmlWeb();
        }

        public List<NewsItem> GetNews(DateTime? lastNewsDate)
        {
            return Task.Run(async () => await GetNewsAsync(lastNewsDate, CancellationToken.None)).GetAwaiter().GetResult();
        }

        public async Task<List<NewsItem>> GetNewsAsync(DateTime? lastNewsDate, CancellationToken ct)
        {
            List<NewsItem> newNews = new List<NewsItem>();

            int readPagesLimit = _config.ReadPagesLimit > 0 ? _config.ReadPagesLimit : int.MaxValue;
            int newNewsLimit = _config.NewNewsLimit > 0 ? _config.NewNewsLimit : int.MaxValue;

            DateTime lastNewsDateVal = lastNewsDate.HasValue ? lastNewsDate.Value : DateTime.MinValue;
            DateTime curNewsDateVal = DateTime.MaxValue;
            int curPpageNumber = 0;
            

            while (curPpageNumber < readPagesLimit &&
                newNews.Count < newNewsLimit &&
                curNewsDateVal > lastNewsDateVal)
            {
                string pageUrl = $"{_config.NewsUrl}?page={++curPpageNumber}";
                HtmlDocument doc = await _web.LoadFromWebAsync(pageUrl, ct);

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(XPATH_NEWS_ITEM);
                if (nodes == null || nodes.Count == 0)
                {
                    break;
                }
                
                for (int i = 0; i < nodes.Count; i++)
                {
                    NewsItem newsItem = new NewsItem();
                    
                    HtmlNode headerNode = nodes[i].SelectSingleNode(XPATH_NEWS_ITEM_HEADER);
                    if (headerNode != null)
                    {
                        string urlSuffix = headerNode.GetAttributeValue("href", "");
                        newsItem.Title = HttpUtility.HtmlDecode(headerNode.InnerText.Trim());
                        newsItem.Url = new Uri(new Uri(_config.NewsUrl), urlSuffix).AbsoluteUri;
                        
                    }
                    
                    HtmlNode footerDateNode = nodes[i].SelectSingleNode(XPATH_NEWS_ITEM_FOOTER_DATE);
                    if (footerDateNode != null)
                    {
                        string dateStr = HttpUtility.HtmlDecode(footerDateNode.InnerText.Trim());
                        if (DateTime.TryParse(dateStr, NEWS_DATE_CULTURE, DateTimeStyles.None, out DateTime date))
                        {
                            curNewsDateVal = date;
                            newsItem.Date = date;
                        }
                    }

                    if (newNews.Count >= newNewsLimit ||
                        (newsItem.IsValid() && newsItem.Date <= lastNewsDateVal))
                    {
                        break;
                    }
                    if (newsItem.IsValid())
                    {
                        newNews.Add(newsItem);
                    }
                }
            }

            return newNews;
        }
    }
}
