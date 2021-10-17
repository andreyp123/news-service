using System;

namespace Common
{
    public class NewsItem
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Url { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Title) && Date != default(DateTime);
        }

        public override string ToString()
        {
            return $"Date=\"{Date.ToString("O")}\", Url=\"{Url}\", Title=\"{Title}\"";
        }

        public override bool Equals(object obj)
        {
            NewsItem other = obj as NewsItem;
            return other != null && other.Title == Title && other.Date == Date && other.Url == Url;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = Title.GetHashCode();
                hash = 31 * hash + Date.GetHashCode();
                return 31 * hash + Url.GetHashCode();
            }
        }
    }
}
