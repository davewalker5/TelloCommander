using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TelloCommander.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class TelemetryError
    {
        [Key]
        public int Id { get; set; }
        public long Time { get; set; }
        public string Message { get; set; }
    }
}
