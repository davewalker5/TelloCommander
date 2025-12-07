using System;
using System.Diagnostics.CodeAnalysis;

namespace TelloCommander.Simulator
{
    [ExcludeFromCodeCoverage]
    public class BatteryCriticalEventArgs : EventArgs
    {
        public int Battery { get; set; }
        public int TimeOfFlight { get; set; }
    }
}
