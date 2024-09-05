using API.Models.Logging;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Logging
{
    public class LogsDbContext : DbContext
    {
        public LogsDbContext(DbContextOptions<LogsDbContext> options) : base(options) { }

        public DbSet<LogEntry> LogEntries { get; set; }
    }
}