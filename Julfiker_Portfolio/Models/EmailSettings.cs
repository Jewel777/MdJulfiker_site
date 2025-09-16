namespace Julfiker_Portfolio.Models
{
    public class EmailSettings
    {
        public string Host { get; set; } = "smtp.gmail.com";
        public int    Port { get; set; } = 587;    // 587=StartTLS, 465=SSL
        public bool   UseSsl { get; set; } = false; // false -> StartTLS, true -> SSL on connect
        public string User { get; set; } = "";     // SMTP username (email address)
        public string Password { get; set; } = ""; // App password / SMTP password

        // Sender (must be YOUR mailbox for DMARC)
        public string FromEmail { get; set; } = "";
        public string FromName  { get; set; } = "Portfolio Contact";

        // Receiver (where you read messages)
        public string ToEmail   { get; set; } = "";
        public string ToName    { get; set; } = "Md Julfiker Ali Jewel";
    }
}

