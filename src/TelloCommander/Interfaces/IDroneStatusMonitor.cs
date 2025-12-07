using System;
using TelloCommander.Status;

namespace TelloCommander.Interfaces
{
    public interface IDroneStatusMonitor
    {
        bool OutputEnabled { get; }

        event EventHandler<DroneStatusEventArgs> DroneStatusUpdated;
        void Listen(int port);
        void StartCapture(string file, int intervalMilliseconds);
        void Stop();
        void StopCapture();
    }
}
