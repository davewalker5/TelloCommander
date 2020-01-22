using Microsoft.EntityFrameworkCore;
using TelloCommander.Data.Entities;

namespace TelloCommander.Data.Interfaces
{
    public interface ITelloCommanderDbContext
	{
        DbSet<Drone> Drones { get; set; }
        DbSet<TelemetrySession> Sessions { get; set; }
        DbSet<TelemetryProperty> Properties { get; set; }
        DbSet<TelemetryDataPoint> DataPoints { get; set; }
        DbSet<TelemetryError> Errors { get; set; }

        int SaveChanges();
	}
}
