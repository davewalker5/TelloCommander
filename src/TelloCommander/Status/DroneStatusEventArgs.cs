using System;
using System.Diagnostics.CodeAnalysis;
using TelloCommander.Interfaces;

namespace TelloCommander.Status
{
    [ExcludeFromCodeCoverage]
    public class DroneStatusEventArgs : EventArgs
    {
        public IDroneStatus Status { get; set; }
	}
}
