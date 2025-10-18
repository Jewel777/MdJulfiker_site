
public class AppDbContext : DbContext
{
    public DbSet<PageHit> PageHits => Set<PageHit>();
    public DbSet<DailyStat> DailyStats => Set<DailyStat>();
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }
}
