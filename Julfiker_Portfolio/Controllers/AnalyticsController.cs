using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Julfiker_Portfolio.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Cryptography;
using System.Text;

public class AnalyticsController : Controller
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _cfg;
    private readonly ITimeLimitedDataProtector _cookieProtector;

    private const string AdminCookieName = "mja_admin";

    public AnalyticsController(AppDbContext db, IConfiguration cfg, IDataProtectionProvider dataProtection)
    {
        _db = db;
        _cfg = cfg;
        _cookieProtector = dataProtection
            .CreateProtector("JulfikerPortfolio.AnalyticsAdmin.v1")
            .ToTimeLimitedDataProtector();
    }

    [HttpGet("/admin/analytics-login")]
    public IActionResult AnalyticsLogin() => View("Login");

    [HttpPost("/admin/analytics-login")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("admin-login")]
    public IActionResult AnalyticsLogin([FromForm] string key)
    {
        var expected = _cfg["Admin:AnalyticsKey"];
        if (!string.IsNullOrWhiteSpace(expected) && SecretsMatch(key, expected))
        {
            Response.Cookies.Append(
                AdminCookieName,
                _cookieProtector.Protect("analytics-admin", TimeSpan.FromHours(24)),
                new CookieOptions {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(24)
                }
            );
            return RedirectToAction("Index");
        }
        ModelState.AddModelError(string.Empty, "Invalid analytics key.");
        return View("Login");
    }

    // GET /admin/analytics-logout
    [HttpGet("/admin/analytics-logout")]
    public IActionResult AnalyticsLogout()
    {
        Response.Cookies.Delete(AdminCookieName, new CookieOptions { Secure = true, SameSite = SameSiteMode.Lax });
        return Redirect("~/");
    }

    public async Task<IActionResult> Index()
    {
        if (!HasValidAdminCookie())
            return NotFound();

        var since = DateTime.UtcNow.AddDays(-30);

        var pageViews30 = await _db.PageHits.Where(h => h.CreatedUtc >= since).CountAsync();
        var uniqueSessions30 = await _db.PageHits.Where(h => h.CreatedUtc >= since).Select(h => h.SessionId).Distinct().CountAsync();
        var uniqueVisitors30 = await _db.PageHits.Where(h => h.CreatedUtc >= since).Select(h => h.IpHash).Distinct().CountAsync();

        var byDay = await _db.PageHits.Where(h => h.CreatedUtc >= since)
            .GroupBy(h => h.CreatedUtc.Date)
            .Select(g => new { Day = g.Key, Views = g.Count(), UniqueSessions = g.Select(x => x.SessionId).Distinct().Count() })
            .OrderBy(g => g.Day)
            .ToListAsync();

        var topPages = await _db.PageHits.Where(h => h.CreatedUtc >= since)
            .GroupBy(h => h.Path)
            .Select(g => new { Path = g.Key, Views = g.Count(), UniqueSessions = g.Select(x => x.SessionId).Distinct().Count() })
            .OrderByDescending(g => g.Views).Take(10).ToListAsync();

        ViewBag.PageViews30 = pageViews30;
        ViewBag.UniqueSessions30 = uniqueSessions30;
        ViewBag.UniqueVisitors30 = uniqueVisitors30;
        ViewBag.ByDay = byDay;
        ViewBag.TopPages = topPages;

        return View();
    }

    private bool HasValidAdminCookie()
    {
        if (!Request.Cookies.TryGetValue(AdminCookieName, out var value))
            return false;

        try
        {
            return _cookieProtector.Unprotect(value) == "analytics-admin";
        }
        catch (CryptographicException)
        {
            return false;
        }
    }

    private static bool SecretsMatch(string supplied, string expected)
    {
        var suppliedHash = SHA256.HashData(Encoding.UTF8.GetBytes(supplied ?? string.Empty));
        var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(expected));
        return CryptographicOperations.FixedTimeEquals(suppliedHash, expectedHash);
    }
}
