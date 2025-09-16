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

        // Home hosts all sections (#home, #skills, ...)
        public IActionResult Index()
        {
            ViewData["Name"] = "Md Julfiker Ali Jewel";
            ViewData["Title"] = "Portfolio Website";
            return View();
        }

        // --- Legacy routes -> anchor redirects on Home ---
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

        // GET: Contact (anchor on one-page)
        [HttpGet]
        public IActionResult Contact() => Redirect("/#contact");

        // POST: Contact form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(
            [FromForm] string Name,
            [FromForm] string Email,
            [FromForm] string Message,
            // Honeypot field named "Website" (must be empty)
            [FromForm(Name = "Website")] string Honey = ""
        )
        {
            // Honeypot: bots often fill hidden fields
            if (!string.IsNullOrWhiteSpace(Honey))
            {
                TempData["SuccessMessage"] = "Thanks!"; // silently succeed
                return Redirect("/#contact");
            }

            // Basic validation
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Message))
            {
                TempData["ErrorMessage"] = "Please fill in your name, email, and message.";
                return Redirect("/#contact");
            }

            try
            {
                var msg = new MimeMessage();

                // IMPORTANT: From must be YOUR mailbox (DMARC/SPF safe)
                msg.From.Add(new MailboxAddress(
                    string.IsNullOrWhiteSpace(_email.FromName) ? "Portfolio Contact" : _email.FromName,
                    _email.FromEmail));

                // Where you want to receive messages
                msg.To.Add(new MailboxAddress(
                    string.IsNullOrWhiteSpace(_email.ToName) ? "Md Julfiker Ali Jewel" : _email.ToName,
                    _email.ToEmail));

                // Let replies go back to the visitor
                msg.ReplyTo.Add(new MailboxAddress(Name, Email));

                msg.Subject = $"New Portfolio Contact from {Name}";

                var builder = new BodyBuilder
                {
                    TextBody = $"Name: {Name}\nEmail: {Email}\n\nMessage:\n{Message}",
                    HtmlBody =
                        $"<p><strong>Name:</strong> {System.Net.WebUtility.HtmlEncode(Name)}</p>" +
                        $"<p><strong>Email:</strong> {System.Net.WebUtility.HtmlEncode(Email)}</p>" +
                        $"<p><strong>Message:</strong><br/>{System.Net.WebUtility.HtmlEncode(Message).Replace("\n", "<br/>")}</p>"
                };
                msg.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                var useSsl = _email.UseSsl; // true = SSL on connect (465), false = StartTLS (587)
                await client.ConnectAsync(_email.Host, _email.Port, useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);

                if (!string.IsNullOrEmpty(_email.User))
                {
                    await client.AuthenticateAsync(_email.User, _email.Password);
                }

                await client.SendAsync(msg);
                await client.DisconnectAsync(true);

                TempData["SuccessMessage"] = $"Thank you, {Name}! Your message has been sent.";
                _logger.LogInformation("Contact email sent by {Name} <{Email}>", Name, Email);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error sending contact form email.");
                TempData["ErrorMessage"] = "Sorry—couldn’t send your message right now. Please try again later.";
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
