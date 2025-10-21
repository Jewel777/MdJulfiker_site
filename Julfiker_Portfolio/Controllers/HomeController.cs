using System.Diagnostics;
using System.Threading.Tasks;
using Julfiker_Portfolio.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Julfiker_Portfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EmailSettings _email;

        public HomeController(ILogger<HomeController> logger, IOptions<EmailSettings> emailOptions)
        {
            _logger = logger;
            _email = emailOptions.Value;
        }

        // One-page host
        public IActionResult Index()
        {
            ViewData["Name"] = "Md Julfiker Ali Jewel";
            ViewData["Title"] = "Portfolio Website";
            return View();
        }

        // Legacy routes -> anchors on Home
        public IActionResult Resume()          => Redirect("/#resume");
        public IActionResult Publications()    => Redirect("/#publications");
        public IActionResult Education()       => Redirect("/#education");
        public IActionResult Skills()          => Redirect("/#skills");
        public IActionResult Experience()      => Redirect("/#experience");
        public IActionResult Projects()        => Redirect("/#projects");
        public IActionResult Research()        => Redirect("/#research");
        public IActionResult Accomplishments() => Redirect("/#accomplishments");
        public IActionResult Activities()      => Redirect("/#accomplishments");
        public IActionResult Privacy()         => Redirect("/#privacy");

        [HttpGet]
        public IActionResult Contact() => Redirect("/#contact");

        // We use honeypot + IgnoreAntiforgeryToken to avoid random antiforgery failures in prod.
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Contact(
            [FromForm] string Name,
            [FromForm] string Email,
            [FromForm] string Message,
            [FromForm(Name = "Website")] string Honey = "" // hidden honeypot field
        )
        {
            // Bot? Silently succeed.
            if (!string.IsNullOrWhiteSpace(Honey))
            {
                TempData["SuccessMessage"] = "Thanks!";
                return Redirect("/#contact");
            }

            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Message))
            {
                TempData["ErrorMessage"] = "Please fill in your name, email, and message.";
                return Redirect("/#contact");
            }

            try
            {
                // Prefer env var in production (Render) over JSON
                var password = System.Environment.GetEnvironmentVariable("Email__Password") ?? _email.Password;
                if (string.IsNullOrWhiteSpace(password))
                {
                    _logger.LogError("Email password not configured. Set env var Email__Password.");
                    TempData["ErrorMessage"] = "Email temporarily unavailable. Please try again later.";
                    return Redirect("/#contact");
                }

                var msg = new MimeMessage();
                msg.From.Add(new MailboxAddress(
                    string.IsNullOrWhiteSpace(_email.FromName) ? "Portfolio Contact" : _email.FromName,
                    _email.FromEmail));
                msg.To.Add(new MailboxAddress(
                    string.IsNullOrWhiteSpace(_email.ToName) ? "Md Julfiker Ali Jewel" : _email.ToName,
                    _email.ToEmail));
                msg.ReplyTo.Add(new MailboxAddress(Name, Email));
                msg.Subject = $"New Portfolio Contact from {Name}";

                var safeName = System.Net.WebUtility.HtmlEncode(Name);
                var safeEmail = System.Net.WebUtility.HtmlEncode(Email);
                var safeMsgHtml = System.Net.WebUtility.HtmlEncode(Message).Replace("\n", "<br/>");

                var body = new BodyBuilder
                {
                    TextBody = $"Name: {Name}\nEmail: {Email}\n\nMessage:\n{Message}",
                    HtmlBody = $"<p><strong>Name:</strong> {safeName}</p>" +
                               $"<p><strong>Email:</strong> {safeEmail}</p>" +
                               $"<p><strong>Message:</strong><br/>{safeMsgHtml}</p>"
                };
                msg.Body = body.ToMessageBody();

                using var client = new SmtpClient
                {
                    Timeout = 20000 // 20 seconds
                };

                // Force basic auth (no OAuth) for Microsoft 365 SMTP
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.LocalDomain = "mdjulfikeralijewel.com";

                // STARTTLS on 587 for smtp.office365.com
                await client.ConnectAsync(_email.Host, _email.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_email.User, password);
                await client.SendAsync(msg);
                await client.DisconnectAsync(true);

                TempData["SuccessMessage"] = $"Thank you, {safeName}! Your message has been sent.";
                _logger.LogInformation("Contact email sent by {Name} <{Email}>", Name, Email);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error sending contact form email.");
                TempData["ErrorMessage"] = "Sorry—couldn’t send your message right now. Please email me directly.";
            }

            return Redirect("/#contact");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
