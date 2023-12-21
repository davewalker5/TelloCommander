using System;
using System.Collections.Generic;
using System.Linq;
using TelloCommander.Data.Entities;
using TelloCommander.Data.Interfaces;
using TelloCommander.Interfaces;
using TelloCommander.Response;
using TelloCommander.Status;

namespace TelloCommander.Data.Collector
{
    public class TelemetryCollector
    {
        private readonly Dictionary<string, TelemetryProperty> _properties = new Dictionary<string, TelemetryProperty>();
        private readonly static object _lock = new object();
        private ITelloCommanderDbContext _context;
        private TelemetrySession _session;
        private IDroneStatusMonitor _monitor;
        private IEnumerable<string> _propertiesToCapture;
        private long _intervalMilliseconds;
        private long _lastWritten;

        public bool LogToConsole { get; set; }

        public TelemetryCollector(ITelloCommanderDbContext context, IDroneStatusMonitor monitor)
        {
            _context = context;
            _monitor = monitor;
        }

        /// <summary>
        /// Start a new telemetry collection session, collecting at the specified interval
        /// </summary>
        /// <param name="droneName"></param>
        /// <param name="sessionName"></param>
        /// <param name="intervalMilliseconds"></param>
        /// <param name="propertyNames"></param>
        public void Start(string droneName, string sessionName, long intervalMilliseconds, IEnumerable<string> propertyNames = null)
        {
            Drone drone = _context.Drones.FirstOrDefault(d => d.Name == droneName);
            if (drone == null)
            {
                drone = new Drone { Name = droneName };
                _context.Drones.Add(drone);
                _context.SaveChanges();
            }

            _session = new TelemetrySession
            {
                DroneId = drone.Id,
                Name = sessionName,
                Start = DateTime.Now
            };

            _context.Sessions.Add(_session);
            _context.SaveChanges();

            _propertiesToCapture = propertyNames;
            _intervalMilliseconds = intervalMilliseconds;
            _lastWritten = 0;
            _monitor.DroneStatusUpdated += OnDroneStatusUpdated;
        }

        /// <summary>
        /// Stop collecting data
        /// </summary>
        public void Stop()
        {
            _monitor.DroneStatusUpdated -= OnDroneStatusUpdated;
        }

        /// <summary>
        /// Callback to handle data collection events from the status monitor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDroneStatusUpdated(object sender, DroneStatusEventArgs e)
        {
            lock (_lock)
            {
                DateTime now = DateTime.Now;
                long timeSinceStart = (long)(now - _session.Start).TotalMilliseconds;
                long interval = timeSinceStart - _lastWritten;
                if (interval >= _intervalMilliseconds)
                {
                    if (LogToConsole)
                    {
                        Console.WriteLine($"Writing data for sequence {e.Status.Sequence}");
                    }

                    if (!string.IsNullOrEmpty(e.Status.Error))
                    {
                        _context.Errors.Add(new TelemetryError { Time = timeSinceStart, Message = e.Status.Error });
                    }
                    else if (!string.IsNullOrEmpty(e.Status.Status))
                    {
                        foreach (string key in e.Status.RawValues.Keys)
                        {
                            if ((_propertiesToCapture == null) || ((_propertiesToCapture != null) && (_propertiesToCapture.Contains(key))))
                            {
                                _context.DataPoints.Add(new TelemetryDataPoint
                                {
                                    Property = GetProperty(key),
                                    SessionId = _session.Id,
                                    Time = timeSinceStart,
                                    Sequence = e.Status.Sequence,
                                    Value = ResponseParser.ParseToNumber(e.Status.RawValues[key])
                                });
                            }
                        }
                    }

                    _context.SaveChanges();
                    _lastWritten = timeSinceStart;
                }
            }
        }

        /// <summary>
        /// Get the property corresponding to a specified key using the local cache of
        /// properties first and falling back to retrieval from the database and, if
        /// required, creation of a new property
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private TelemetryProperty GetProperty(string key)
        {
            TelemetryProperty property;

            if (_properties.ContainsKey(key))
            {
                property = _properties[key];
            }
            else
            {
                property = _context.Properties.FirstOrDefault(p => p.Name == key);
                if (property == null)
                {
                    property = new TelemetryProperty { Name = key };
                    _context.Properties.Add(property);
                }
                _properties.Add(key, property);
            }

            return property;
        }
    }
}
