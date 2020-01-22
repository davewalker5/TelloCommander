using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TelloCommander.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class TelemetryDataPoint
    {
        [Key]
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int SessionId { get; set; }
        public long Time { get; set; }
        public int Sequence { get; set; }
        public decimal Value { get; set; }

        public TelemetryProperty Property { get; set; }
        public TelemetrySession Session { get; set; }
    }
}
