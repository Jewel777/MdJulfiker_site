using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Julfiker_Portfolio.Data;

public class AnalyticsController : Controller
{
    private readonly AppDbContext _db;
    public AnalyticsController(AppDbContext db) { _db = db; }

    public async Task<IActionResult> Index()
    {
        var since = DateTime.UtcNow.AddDays(-30);


        var views = await _db.PageHits.Where(h => h.CreatedUtc >= since).CountAsync();
        var uniques = await _db.PageHits.Where(h => h.CreatedUtc >= since)
                                        .Select(h => h.SessionId).Distinct().CountAsync();

        ViewBag.PageViews = views;
        ViewBag.UniqueSessions = uniques;
        return View();
    }
}
