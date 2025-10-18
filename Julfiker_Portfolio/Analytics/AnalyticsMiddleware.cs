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
            if (!HttpMethods.IsGet(ctx.Request.Method) && !HttpMethods.IsHead(ctx.Request.Method)) return;
            var path = ctx.Request.Path.Value ?? "/";
            if (IsStatic(path)) return;

            var ua = ctx.Request.Headers.UserAgent.ToString() ?? "";
            var isBot = string.IsNullOrWhiteSpace(ua) ||
                        ua.Contains("bot", StringComparison.OrdinalIgnoreCase) ||
                        ua.Contains("spider", StringComparison.OrdinalIgnoreCase) ||
                        ua.Contains("crawler", StringComparison.OrdinalIgnoreCase);

            if (isBot) return;

            const string cookieName = "aid";
            if (!ctx.Request.Cookies.TryGetValue(cookieName, out var sessionId) || string.IsNullOrWhiteSpace(sessionId))
            {
                sessionId = Guid.NewGuid().ToString("N");
                ctx.Response.Cookies.Append(cookieName, sessionId, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddMonths(6)
                });
            }

            var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
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
                StatusCode = ctx.Response.StatusCode,
                LoadTimeMs = (int)sw.ElapsedMilliseconds,
                IsBot = false
            };

            db.PageHits.Add(hit);
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Analytics log skipped");
        }
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
        return new[] { ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".svg", ".ico" }
            .Contains(ext, StringComparer.OrdinalIgnoreCase);
    }
}
