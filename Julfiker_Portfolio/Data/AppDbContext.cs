using Microsoft.EntityFrameworkCore;
using Julfiker_Portfolio.Models;

namespace Julfiker_Portfolio.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }


        public DbSet<PageHit> PageHits => Set<PageHit>();
        public DbSet<DailyStat> DailyStats => Set<DailyStat>();
    }
}
