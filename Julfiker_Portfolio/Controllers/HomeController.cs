using System.Diagnostics;
using Julfiker_Portfolio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;

namespace Julfiker_Portfolio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public IActionResult Index()
        {
            ViewData["Name"] = "Md Julfiker Ali Jewel";
            ViewData["Title"] = "Portfolio Website";
            return View();
        }

        public IActionResult Resume() => View();
        public IActionResult Publications() => View();
        public IActionResult Education() => View();
        public IActionResult Skills() => View();
        public IActionResult Experience() => View();
        public IActionResult Projects() => View();
        public IActionResult Research() => View();
        public IActionResult Accomplishments() => View();
        public IActionResult Privacy() => View();

        // GET: Contact Page
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        // POST: Handle Contact Form Submission
        [HttpPost]
        public IActionResult Contact(string Name, string Email, string Message)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(Name, Email)); // Visitor info
                emailMessage.To.Add(new MailboxAddress("Md Julfiker Ali Jewel", "yourreceiveremail@example.com")); // <-- Replace with YOUR receiving email
                emailMessage.Subject = $"New Portfolio Contact Form Submission from {Name}";
                emailMessage.Body = new TextPart("plain")
                {
                    Text = $"Name: {Name}\nEmail: {Email}\n\nMessage:\n{Message}"
                };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls); // Gmail SMTP
                    client.Authenticate("yourreceiveremail@example.com", "your-app-password"); // <-- Use your Gmail app password
                    client.Send(emailMessage);
                    client.Disconnect(true);
                }

                TempData["SuccessMessage"] = "Thank you, " + Name + "! Your message has been sent successfully.";
                _logger.LogInformation("Contact form email sent by {Name} ({Email})", Name, Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending contact form email.");
                TempData["SuccessMessage"] = "Oops! Something went wrong. Please try again later.";
            }

            return RedirectToAction("Contact");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
