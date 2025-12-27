using Julfiker_Portfolio.Models;           // EmailSettings
using Microsoft.EntityFrameworkCore;       // EF Core
using Julfiker_Portfolio.Data;             // AppDbContext
using Microsoft.AspNetCore.HttpOverrides;  // ✅ Forwarded headers

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Bind Email settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));

// EF Core (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Forwarded headers (important when hosted behind proxy/CDN)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

    // Optional hardening (recommended if you know your proxy IPs):
    // options.KnownNetworks.Clear();
    // options.KnownProxies.Clear();
});

var app = builder.Build();

// ✅ Must be early so RemoteIpAddress is the real client IP
app.UseForwardedHeaders();

// Prod pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Ensure DB/tables exist (must run before middleware logs)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// ✅ Log page views (after routing is fine; before auth/endpoints is correct)
app.UseMiddleware<AnalyticsMiddleware>();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
