using System.Security.Cryptography;
using System.Text;
using Julfiker_Portfolio.Data;

public class AnalyticsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AnalyticsMiddleware> _logger;

    public AnalyticsMiddleware(RequestDelegate next, ILogger<AnalyticsMiddleware> logger)
    {
        _next = next; _logger = logger;
    }

    public async Task Invoke(HttpContext ctx, AppDbContext db, IConfiguration cfg)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await _next(ctx);
        sw.Stop();

        try
        {
            // Only track page views
            if (!HttpMethods.IsGet(ctx.Request.Method) && !HttpMethods.IsHead(ctx.Request.Method)) return;

            var path = ctx.Request.Path.Value ?? "/";
            if (IsStatic(path)) return;

            // ✅ Don’t track obvious scanner targets (your app is not PHP)
            if (path.EndsWith(".php", StringComparison.OrdinalIgnoreCase)) return;

            // Optional: ignore error noise (bots generate tons of 404s on random paths)
            var status = ctx.Response.StatusCode;
            var trackNon2xx = cfg.GetValue("Analytics:TrackNon2xx", false);
            if (!trackNon2xx && (status < 200 || status >= 400)) return;

            // Config excludes
            var exclude = (cfg.GetSection("Analytics:ExcludePaths").Get<string[]>() ?? Array.Empty<string>());
            if (exclude.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase))) return;

            var ua = ctx.Request.Headers.UserAgent.ToString() ?? "";
            var isBot = LooksLikeBot(ua);

            var trackBots = cfg.GetValue("Analytics:TrackBots", false);
            if (isBot && !trackBots) return;

            // Session cookie
            const string cookieName = "aid";
            if (!ctx.Request.Cookies.TryGetValue(cookieName, out var sessionId) || string.IsNullOrWhiteSpace(sessionId))
            {
                sessionId = Guid.NewGuid().ToString("N");
                ctx.Response.Cookies.Append(cookieName, sessionId, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = ctx.Request.IsHttps,  // safer locally
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddMonths(6)
                });
            }

            // ✅ Use real client IP (after app.UseForwardedHeaders)
            var ip =
                ctx.Connection.RemoteIpAddress?.ToString()
                ?? ctx.Request.Headers["X-Forwarded-For"].ToString().Split(',').FirstOrDefault()?.Trim()
                ?? "0.0.0.0";

            var salt = cfg["Analytics:IpHashSalt"] ?? "default_salt_change_me";
            var ipHash = Hash($"{ip}|{salt}");

            var hit = new Julfiker_Portfolio.Models.PageHit
            {
                CreatedUtc = DateTime.UtcNow,
                Path = path,
                Query = ctx.Request.QueryString.HasValue ? ctx.Request.QueryString.Value : null,
                Referrer = ctx.Request.Headers.Referer.ToString(),
                UserAgent = ua,
                IpHash = ipHash,
                SessionId = sessionId!,
                StatusCode = status,
                LoadTimeMs = (int)sw.ElapsedMilliseconds,
                IsBot = isBot
            };

            db.PageHits.Add(hit);
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Analytics log skipped");
        }
    }

    private static bool LooksLikeBot(string ua)
    {
        if (string.IsNullOrWhiteSpace(ua)) return true;

        // classic keywords
        if (ua.Contains("bot", StringComparison.OrdinalIgnoreCase) ||
            ua.Contains("spider", StringComparison.OrdinalIgnoreCase) ||
            ua.Contains("crawler", StringComparison.OrdinalIgnoreCase))
            return true;

        // common automation / scanners
        string[] bad =
        {
            "curl", "wget", "python-requests", "httpclient", "okhttp",
            "postman", "insomnia", "go-http-client", "java/", "axios",
            "headless", "puppeteer", "playwright", "selenium"
        };

        return bad.Any(x => ua.Contains(x, StringComparison.OrdinalIgnoreCase));
    }

    private static string Hash(string input)
    {
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
    }

    private static bool IsStatic(string path)
    {
        var ext = Path.GetExtension(path);
        if (string.IsNullOrEmpty(ext)) return false;

        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".css",".js",".png",".jpg",".jpeg",".gif",".svg",".webp",".ico",
            ".woff",".woff2",".ttf",".eot",".map",".pdf",".txt",".xml",".json",".csv"
        };

        return set.Contains(ext);
    }
}
