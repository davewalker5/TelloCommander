using System;
using System.Threading;
using System.Threading.Tasks;
using TelloCommander.Interfaces;
using TelloCommander.Response;
using TelloCommander.Status;

namespace TelloCommander.Simulator
{
    public class MockDroneStatusMonitor : StatusMonitorBase, IDroneStatus, IDroneStatusMonitor, IDisposable
    {
        /// <summary>
        /// Event raised when the event arguments are updated
        /// </summary>
        public event EventHandler<DroneStatusEventArgs> DroneStatusUpdated;

        /// <summary>
        /// Start the mock status listener on another thread
        /// </summary>
        /// <param name="port"></param>
        public override void Listen(int port)
        {
            _source = new CancellationTokenSource();
            CancellationToken token = _source.Token;

            var task = Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();

                Sequence = 1;

                while (true)
                {
                    try
                    {
                        Error = null;
                        Status = $"pitch:0;roll:0;yaw:0;vgx:0;vgy:0;vgz:0;templ:0;temph:0;tof:0;h:0;bat:100;baro:0.00;time:0;agx:0.00;agy:0.00;agz:0.00;";
                        RawValues = ResponseParser.ParseToDictionary(Status);
                        Sequence++;
                    }
                    catch (Exception ex)
                    {
                        Error = ex.Message;
                    }

                    if (OutputEnabled)
                    {
                        WriteStatus();
                    }

                    // Notify subscribers to the status updated event
                    DroneStatusUpdated?.Invoke(this, new DroneStatusEventArgs { Status = this });

                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    Thread.Sleep(500);
                }

            }, token);
        }

        #region IDisposable Implementation
        /// <summary>
        /// Override the protected Dispose() implementation
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                base.Dispose(disposing);
            }
        }
        #endregion
    }
}
