// Controllers/AnalyticsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class AnalyticsController : Controller
{
    private readonly AppDbContext _db;
    public AnalyticsController(AppDbContext db) { _db = db; }

    public async Task<IActionResult> Index()
    {
        var since = DateTime.UtcNow.AddDays(-30);

        var pageViews30 = await _db.PageHits
            .Where(h => h.CreatedUtc >= since && !h.IsBot)
            .CountAsync();

        var uniqueSessions30 = await _db.PageHits
            .Where(h => h.CreatedUtc >= since && !h.IsBot)
            .Select(h => h.SessionId)
            .Distinct()
            .CountAsync();

        var uniqueVisitors30 = await _db.PageHits
            .Where(h => h.CreatedUtc >= since && !h.IsBot)
            .Select(h => h.IpHash)
            .Distinct()
            .CountAsync();

        var byDay = await _db.PageHits
            .Where(h => h.CreatedUtc >= since && !h.IsBot)
            .GroupBy(h => h.CreatedUtc.Date)
            .Select(g => new { Day = g.Key, Views = g.Count(), UniqueSessions = g.Select(x => x.SessionId).Distinct().Count() })
            .OrderBy(g => g.Day)
            .ToListAsync();

        var topPages = await _db.PageHits
            .Where(h => h.CreatedUtc >= since && !h.IsBot)
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

