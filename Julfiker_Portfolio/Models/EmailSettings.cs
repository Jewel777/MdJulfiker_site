namespace Julfiker_Portfolio.Models
{
    public class EmailSettings
    {
        public string Host { get; set; } = "smtp.gmail.com";
        public int    Port { get; set; } = 587;     // 587 StartTLS, 465 SSL
        public bool   UseSsl { get; set; } = false; // false->StartTLS, true->SSL on connect
        public string User { get; set; } = "";      // SMTP username (email)
        public string Password { get; set; } = "";  // App password / SMTP password

        public string FromEmail { get; set; } = ""; // your mailbox (sender)
        public string FromName  { get; set; } = "Portfolio Contact";

        public string ToEmail   { get; set; } = ""; // where you receive
        public string ToName    { get; set; } = "Md Julfiker Ali Jewel";
    }
}

