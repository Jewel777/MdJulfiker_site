// Analytics/AnalyticsMiddleware.cs
using System.Security.Cryptography;
using System.Text;

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

        // Let the rest of the pipeline run first to capture status code
        await _next(ctx);
        sw.Stop();

        try
        {
            // Only track GET/HEAD to your site pages (adjust as needed)
            if (!HttpMethods.IsGet(ctx.Request.Method) && !HttpMethods.IsHead(ctx.Request.Method)) return;
            if (ctx.Request.Path.Value?.StartsWith("/admin", StringComparison.OrdinalIgnoreCase) == true) return;
            if (ctx.Request.Path.Value?.StartsWith("/analytics", StringComparison.OrdinalIgnoreCase) == true) return;
            if (ctx.Response.StatusCode >= 500) return; // skip server errors if you want

            // Basic bot filter
            var ua = ctx.Request.Headers.UserAgent.ToString();
            var isBot = string.IsNullOrWhiteSpace(ua) || ua.Contains("bot", StringComparison.OrdinalIgnoreCase) ||
                        ua.Contains("spider", StringComparison.OrdinalIgnoreCase) ||
                        ua.Contains("crawler", StringComparison.OrdinalIgnoreCase);

            // First-party session cookie
            const string cookieName = "aid";
            if (!ctx.Request.Cookies.TryGetValue(cookieName, out var sessionId) || string.IsNullOrWhiteSpace(sessionId))
            {
                sessionId = Guid.NewGuid().ToString("N");
                ctx.Response.Cookies.Append(cookieName, sessionId, new CookieOptions {
                    HttpOnly = true, Secure = true, SameSite = SameSiteMode.Lax, Expires = DateTimeOffset.UtcNow.AddMonths(6)
                });
            }

            // Privacy-safe IP hash
            var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var salt = cfg["Analytics:IpHashSalt"] ?? "default_salt_change_me";
            var ipHash = Hash($"{ip}|{salt}");

            var hit = new PageHit
            {
                CreatedUtc = DateTime.UtcNow,
                Path = ctx.Request.Path.Value ?? "/",
                Query = ctx.Request.QueryString.HasValue ? ctx.Request.QueryString.Value : null,
                Referrer = ctx.Request.Headers.Referer.ToString(),
                UserAgent = ua,
                IpHash = ipHash,
                SessionId = sessionId!,
                StatusCode = ctx.Response.StatusCode,
                LoadTimeMs = (int)sw.ElapsedMilliseconds,
                IsBot = isBot
            };

            if (!isBot) // keep if you want bot stats too
            {
                db.PageHits.Add(hit);
                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Analytics log skipped");
        }
    }

    private static string Hash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes); // uppercase hex
    }
}

