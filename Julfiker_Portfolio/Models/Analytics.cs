using System;

namespace Julfiker_Portfolio.Models
{
    public class PageHit
    {
        public long Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string Path { get; set; } = "";
        public string? Query { get; set; }
        public string? Referrer { get; set; }
        public string? UserAgent { get; set; }
        public string IpHash { get; set; } = "";
        public string SessionId { get; set; } = "";
        public int? StatusCode { get; set; }
        public int? LoadTimeMs { get; set; }
        public bool IsBot { get; set; }
    }

    
    public class DailyStat
    {
        public int Id { get; set; }
        public DateOnly Day { get; set; }
        public string Path { get; set; } = "";
        public int PageViews { get; set; }
        public int UniqueSessions { get; set; }
    }
}
