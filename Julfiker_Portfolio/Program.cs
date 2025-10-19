using Julfiker_Portfolio.Models;           // EmailSettings
using Microsoft.EntityFrameworkCore;       // EF Core
using Julfiker_Portfolio.Data;             // AppDbContext

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Bind Email settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));

// EF Core (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

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

// ✅ Log page views (must be BEFORE Authorization and endpoint mapping)
app.UseMiddleware<AnalyticsMiddleware>();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
