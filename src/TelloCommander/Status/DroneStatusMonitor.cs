using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using TelloCommander.Interfaces;
using TelloCommander.Response;
using TelloCommander.Udp;

namespace TelloCommander.Status
{
    [ExcludeFromCodeCoverage]
    public class DroneStatusMonitor : IDroneStatus
    {
        public const int DefaultTelloStatusPort = 8890;

        private CancellationTokenSource _source;

        public int Sequence { get; private set; }
        public string Status { get; private set; }
        public Dictionary<string, string> RawValues { get; private set; }
        public Attitude Attitude { get { return ResponseParser.ParseToAttitude(Status); } }
        public Speed Speed { get { return ResponseParser.ParseToSpeed(Status); } }
        public Temperature Temperature { get { return ResponseParser.ParseTemperatureFromPropertyList(Status); } }
        public decimal TOF { get { return ResponseParser.ParseToNumber(RawValues["tof"]); } }
        public decimal Height { get { return ResponseParser.ParseToNumber(RawValues["h"]); } }
        public decimal Battery { get { return ResponseParser.ParseToNumber(RawValues["bat"]); } }
        public decimal Barometer { get { return ResponseParser.ParseToNumber(RawValues["baro"]); } }
        public decimal Time { get { return ResponseParser.ParseToNumber(RawValues["time"]); } }
        public Acceleration Acceleration { get { return ResponseParser.ParseToAcceleration(Status); } }
        public string Error { get; private set; }

        /// <summary>
        /// Event raised when the event arguments are updated
        /// </summary>
        public event EventHandler<DroneStatusEventArgs> DroneStatusUpdated;

        /// <summary>
        /// Status the status listener on another thread
        /// </summary>
        /// <param name="port"></param>
        public void Listen(int port)
        {
            _source = new CancellationTokenSource();
            CancellationToken token = _source.Token;

            var task = Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();

                TelloUdpListener listener = new TelloUdpListener();
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

                    // Notify subscribers to the status updated event
                    DroneStatusUpdated?.Invoke(this, new DroneStatusEventArgs { Status = this });

                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                        listener.Close();
                        listener.Dispose();
                    }
                }

            }, token);
        }

        /// <summary>
        /// Stop the status listener
        /// </summary>
        public void Stop()
        {
            if (_source != null)
            {
                _source.Cancel();
                _source.Dispose();
            }
        }
    }
}
