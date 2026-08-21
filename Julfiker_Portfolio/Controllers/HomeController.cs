using System.Diagnostics;
using System.Threading.Tasks;
using Julfiker_Portfolio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Http.Json;

namespace Julfiker_Portfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EmailSettings _email;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(
            ILogger<HomeController> logger,
            IOptions<EmailSettings> emailOptions,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _email = emailOptions.Value;
            _httpClientFactory = httpClientFactory;
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

        // Antiforgery, rate limiting, and a honeypot provide layered abuse protection.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("contact")]
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

            if (Name.Length > 100 || Email.Length > 254 || Message.Length > 5000 ||
                !MailboxAddress.TryParse(Email, out _))
            {
                TempData["ErrorMessage"] = "Please provide a valid email and keep the message under 5,000 characters.";
                return Redirect("/#contact");
            }

            try
            {
                if (string.IsNullOrWhiteSpace(_email.ToEmail))
                {
                    _logger.LogError("Contact recipient email is not configured.");
                    TempData["ErrorMessage"] = "Email temporarily unavailable. Please try again later.";
                    return Redirect("/#contact");
                }

                // Render free services block outbound SMTP. FormSubmit accepts the
                // validated message over HTTPS and forwards it to the configured inbox.
                var client = _httpClientFactory.CreateClient("contact-delivery");
                var endpoint = $"https://formsubmit.co/ajax/{Uri.EscapeDataString(_email.ToEmail)}";
                var payload = new Dictionary<string, string>
                {
                    ["name"] = Name,
                    ["email"] = Email,
                    ["message"] = Message,
                    ["_subject"] = $"New Portfolio Contact from {Name}",
                    ["_template"] = "table",
                    ["_captcha"] = "false"
                };

                using var response = await client.PostAsJsonAsync(endpoint, payload);
                response.EnsureSuccessStatusCode();

                var safeName = System.Net.WebUtility.HtmlEncode(Name);
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

