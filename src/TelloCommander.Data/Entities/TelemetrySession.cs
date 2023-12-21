using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TelloCommander.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class TelemetrySession
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Start { get; set; }
        public int DroneId { get; set; }

        public Drone Drone { get; set; }
        public IList<TelemetryDataPoint> DataPoints { get; set; }
        public IList<TelemetryError> Errors { get; set; }
    }
}
