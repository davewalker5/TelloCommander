using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using TelloCommander.Response;

namespace TelloCommander.Status
{
    [ExcludeFromCodeCoverage]
    public abstract class StatusMonitorBase : IDisposable
    {
        public const int DefaultTelloStatusPort = 8890;

        protected CancellationTokenSource _source;
        private StreamWriter _output;
        private DateTime _lastOutput;
        private int _outputIntervalMilliseconds;

        public int Sequence { get; set; }
        public string Status { get; set; }
        public Dictionary<string, string> RawValues { get; set; }
        public Attitude Attitude { get { return ResponseParser.ParseToAttitude(Status); } }
        public Speed Speed { get { return ResponseParser.ParseToSpeed(Status); } }
        public Temperature Temperature { get { return ResponseParser.ParseTemperatureFromPropertyList(Status); } }
        public decimal TOF { get { return ResponseParser.ParseToNumber(RawValues["tof"]); } }
        public decimal Height { get { return ResponseParser.ParseToNumber(RawValues["h"]); } }
        public decimal Battery { get { return ResponseParser.ParseToNumber(RawValues["bat"]); } }
        public decimal Barometer { get { return ResponseParser.ParseToNumber(RawValues["baro"]); } }
        public decimal Time { get { return ResponseParser.ParseToNumber(RawValues["time"]); } }
        public Acceleration Acceleration { get { return ResponseParser.ParseToAcceleration(Status); } }
        public string Error { get; set; }

        public bool OutputEnabled { get; private set; }

        /// <summary>
        /// Starts the status listener on another thread
        /// </summary>
        /// <param name="port"></param>
        public abstract void Listen(int port);

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
            OutputEnabled = true;
        }

        /// <summary>
        /// Stop capturing status
        /// </summary>
        public void StopCapture()
        {
            if (OutputEnabled)
            {
                OutputEnabled = false;
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
        protected void WriteStatus()
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
            _output.Write($"{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff},");
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

        #region IDisposable Implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [ExcludeFromCodeCoverage]
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
        #endregion
    }
}
