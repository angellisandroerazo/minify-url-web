namespace UrlShorteningServiceWeb.Models
{
    public class UrlStatics
    {
        public Guid id { get; set; }
        public required string url { get; set; }

        public required string shortCode { get; set; }

        public DateTime? createdAt { get; set; }

        public DateTime? updatedAt { get; set; }

        public int accessCount { get; set; } = 0;
    }

    public class UrlString
    {
        public required string url { get; set; }
    }

    public class UrlGet
    {
        public Guid id { get; set; }
        public required string url { get; set; }

        public required string shortCode { get; set; }

        public DateTime? createdAt { get; set; }

        public DateTime? updatedAt { get; set; }
    }
}
