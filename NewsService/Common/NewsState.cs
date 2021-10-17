using System;

namespace Common
{
    public class NewsState
    {
        public NewsItem LastNewsItem { get; set; }

        public static NewsState FromNewsItem(NewsItem item)
        {
            return new NewsState { LastNewsItem = item };
        }

        public static NewsState Empty
        {
            get { return new NewsState { LastNewsItem = null }; }
        }

        public override string ToString()
        {
            return LastNewsItem != null ? LastNewsItem.ToString() : "null";
        }

        public override bool Equals(object obj)
        {
            NewsState other = obj as NewsState;
            return other != null && LastNewsItem.Equals(other);
        }

        public override int GetHashCode()
        {
            return LastNewsItem.GetHashCode();
        }
    }
}
