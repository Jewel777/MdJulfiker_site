using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Julfiker_Portfolio.Data;

public class AnalyticsController : Controller
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _cfg;

    private const string AdminCookieName = "mja_admin";

    public AnalyticsController(AppDbContext db, IConfiguration cfg)
    {
        _db = db; _cfg = cfg;
    }

    // GET /admin/analytics-login?k=YOUR_SECRET
    [HttpGet("/admin/analytics-login")]
    public IActionResult AnalyticsLogin([FromQuery] string? k)
    {
        var expected = _cfg["Admin:AnalyticsKey"];
        if (!string.IsNullOrEmpty(expected) && k == expected)
        {
            Response.Cookies.Append(
                AdminCookieName,
                "1",
                new CookieOptions {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(24)
                }
            );
            return RedirectToAction("Index");
        }
        return NotFound();
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
        // Gate: require admin cookie
        if (!Request.Cookies.TryGetValue(AdminCookieName, out var v) || v != "1")
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
}
