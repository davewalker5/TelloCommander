using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelloCommander.Interfaces;
using TelloCommander.Response;
using TelloCommander.Udp;

namespace TelloCommander.Status
{
    [ExcludeFromCodeCoverage]
    public class DroneStatusMonitor : IDroneStatus, IDisposable
    {
        public const int DefaultTelloStatusPort = 8890;

        private CancellationTokenSource _source;
        private StreamWriter _output;
        private DateTime _lastOutput;
        private int _outputIntervalMilliseconds;
        private bool _outputEnabled;

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

                    if (_outputEnabled)
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

        /// <summary>
        /// Start capturing the status to the specified file (in CSV format)
        /// </summary>
        /// <param name="file"></param>
        /// <param name="intervalMilliseconds"></param>
        public void StartCapture(string file, int intervalMilliseconds)
        {
            bool needsHeader = !File.Exists(file);
            _output = new StreamWriter(file, true, Encoding.UTF8);
            _outputIntervalMilliseconds = (intervalMilliseconds > 0) ? intervalMilliseconds : 1000;

            if (needsHeader)
            {
                _output.Write("Date,");
                _output.Write("Sequence,");
                _output.Write("Pitch,");
                _output.Write("Roll,");
                _output.Write("Yaw,");
                _output.Write("Speed X,");
                _output.Write("Speed Y,");
                _output.Write("Speed Z,");
                _output.Write("Temperature Minimum,");
                _output.Write("Temperature Maximum,");
                _output.Write("TOF,");
                _output.Write("Height,");
                _output.Write("Battery,");
                _output.Write("Barometer,");
                _output.Write("Time,");
                _output.Write("Acceleration X,");
                _output.Write("Acceleration Y,");
                _output.Write("Acceleration Z,");
                _output.WriteLine("Error");
            }

            // Write the first status record immediately
            WriteStatusRecord();

            _lastOutput = DateTime.Now;
            _outputEnabled = true;
        }

        /// <summary>
        /// Stop capturing status
        /// </summary>
        public void StopCapture()
        {
            if (_outputEnabled)
            {
                _outputEnabled = false;
                _output.Close();
                _output.Dispose();
                _output = null;
            }
        }

        /// <summary>
        /// Write the drone status to the output stream
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WriteStatus()
        {
            int interval = (int)(DateTime.Now - _lastOutput).TotalMilliseconds;
            if (interval >= _outputIntervalMilliseconds)
            {
                WriteStatusRecord();
                _lastOutput = DateTime.Now;
            }
        }

        /// <summary>
        /// Write a status record to the capture file
        /// </summary>
        private void WriteStatusRecord()
        {
            _output.Write($"{DateTime.Now.ToString("yyyy-mmm-dd hh:mm:ss.fff")},");
            _output.Write($"\"{Sequence}\",");
            _output.Write($"{Attitude.ToCsv()},");
            _output.Write($"{Speed.ToCsv()},");
            _output.Write($"{Temperature.ToCsv()},");
            _output.Write($"\"{TOF}\",");
            _output.Write($"\"{Height}\",");
            _output.Write($"\"{Battery}\",");
            _output.Write($"\"{Barometer}\",");
            _output.Write($"\"{Time}\",");
            _output.Write($"{Acceleration.ToCsv()},");
            _output.WriteLine($"\"{Error}\"");
            _output.Flush();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_output != null)
                {
                    try
                    {
                        _output.Close();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        _output.Dispose();
                    }
                }
            }
        }
    }
}
