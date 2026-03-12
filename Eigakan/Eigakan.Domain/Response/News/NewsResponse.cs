using Eigakan.Domain.Models;

namespace Eigakan.Domain.Response.News
{
    public class NewsResponse
    {
        public string Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Picture { get; set; }
        public string? Url { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? Status { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
    }
}