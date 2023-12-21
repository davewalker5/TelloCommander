using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TelloCommander.Data.Entities
{
    [ExcludeFromCodeCoverage]
    public class TelemetryProperty
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
