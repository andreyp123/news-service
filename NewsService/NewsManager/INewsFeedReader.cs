using Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NewsManager
{
    public interface INewsFeedReader
    {
        List<NewsItem> GetNews(DateTime? lastNewsDate);
        Task<List<NewsItem>> GetNewsAsync(DateTime? lastNewsDate, CancellationToken cancellationToken);
    }
}
