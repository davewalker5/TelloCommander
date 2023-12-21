using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TelloCommander.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class Drone
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public IList<TelemetrySession> Sessions { get; set; }
    }
}
