using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using TelloCommander.Interfaces;
using TelloCommander.Response;
using TelloCommander.Udp;

namespace TelloCommander.Status
{
    [ExcludeFromCodeCoverage]
    public class DroneStatusMonitor : StatusMonitorBase, IDroneStatus, IDroneStatusMonitor, IDisposable
    {
        /// <summary>
        /// Event raised when the event arguments are updated
        /// </summary>
        public event EventHandler<DroneStatusEventArgs> DroneStatusUpdated;

        /// <summary>
        /// Status the status listener on another thread
        /// </summary>
        /// <param name="port"></param>
        public override void Listen(int port)
        {
            _source = new CancellationTokenSource();
            CancellationToken token = _source.Token;

            var task = Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();

                TelloUdpListener listener = new();
                listener.Connect(port);

                Sequence = 1;

                while (true)
                {
                    try
                    {
                        Error = null;
                        Status = listener.Read();
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
                        StopCapture();
                        listener.Close();
                        listener.Dispose();
                    }
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
