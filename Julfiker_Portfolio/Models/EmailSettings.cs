namespace Julfiker_Portfolio.Models
{
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
