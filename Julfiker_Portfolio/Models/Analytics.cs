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

    // Keep your EmailSettings here if you already had it
    public class EmailSettings
    {
        public string Host { get; set; } = "";
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public string User { get; set; } = "";
        public string Password { get; set; } = "";
        public string FromEmail { get; set; } = "";
        public string FromName { get; set; } = "";
        public string ToEmail { get; set; } = "";
        public string ToName { get; set; } = "";
    }
}
