namespace Eigakan.Domain.Request.News
{
    public class UpdateNewsRequest
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Picture { get; set; }
        public string? Url { get; set; }
        public string? Status { get; set; }
    }
}