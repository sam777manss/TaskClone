namespace Client.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string? InnerException { get; set; }

        public string? Message { get; set; }
    }
}