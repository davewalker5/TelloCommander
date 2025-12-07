using System.Collections.Generic;
using TelloCommander.Response;

namespace TelloCommander.Interfaces
{
    public interface IDroneStatus
    {
        int Sequence { get; }
        string Status { get; }
        Dictionary<string, string> RawValues { get; }
        Attitude Attitude { get; }
        Speed Speed { get; }
        Temperature Temperature { get; }
        decimal TOF { get; }
        decimal Height { get; }
        decimal Battery { get; }
        decimal Barometer { get; }
        decimal Time { get; }
        Acceleration Acceleration { get; }
        string Error { get; }
    }
}