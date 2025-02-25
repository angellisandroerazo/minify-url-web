namespace UrlShorteningServiceWeb.Models
{
    public class Result
    {
        public required string message { get; set; }

        public string? url { get; set; } = "";

        public required TypeResult type { get; set; }
    }

    public enum TypeResult
    {
        Success,
        Error,
        Warning,
        Info
    }
}
