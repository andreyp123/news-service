using System;

namespace PersistentStorage.Model
{
    public class NewsItemEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Url { get; set; }
    }
}
