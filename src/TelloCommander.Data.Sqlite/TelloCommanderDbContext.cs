using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TelloCommander.Data.Entities;
using TelloCommander.Data.Interfaces;

namespace TelloCommander.Data.Sqlite
{
    [ExcludeFromCodeCoverage]
    public class TelloCommanderDbContext : DbContext, ITelloCommanderDbContext
    {
        public DbSet<Drone> Drones { get; set; }
        public DbSet<TelemetrySession> Sessions { get; set; }
        public DbSet<TelemetryProperty> Properties { get; set; }
        public DbSet<TelemetryDataPoint> DataPoints { get; set; }
        public DbSet<TelemetryError> Errors { get; set; }

        public TelloCommanderDbContext(DbContextOptions<TelloCommanderDbContext> options) : base(options)
        {
        }
    }
}
