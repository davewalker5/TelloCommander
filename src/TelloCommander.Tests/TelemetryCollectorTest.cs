using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TelloCommander.Data.Collector;
using TelloCommander.Data.Entities;
using TelloCommander.Data.InMemory;
using TelloCommander.Interfaces;
using TelloCommander.Simulator;

namespace TelloCommander.Tests
{
    [TestClass]
    public class TelemetryCollectorTest
    {
        private const string DroneName = "TELLO-1234";
        private readonly string SessionName = $"{DroneName} telemetry session";

        private IDroneStatusMonitor _monitor;
        private TelloCommanderDbContext _context;
        private TelemetryCollector _collector;

        [TestInitialize]
        public void TestInitialise()
        {
            _context = new TelloCommanderDbContextFactory().CreateDbContext(null);
            _monitor = new MockDroneStatusMonitor();
            _collector = new TelemetryCollector(_context, _monitor);
            _monitor.Listen(0);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _monitor.Stop();
        }

        [TestMethod]
        public void CollectionTest()
        {
            DateTime start = DateTime.Now;
            _collector.Start(DroneName, SessionName, 1000, null);
            DateTime end = DateTime.Now;
            Thread.Sleep(3000);
            _collector.Stop();

            Assert.AreEqual(1, _context.Drones.Count());
            Drone drone = _context.Drones.First();
            Assert.AreEqual(DroneName, drone.Name);

            Assert.AreEqual(1, _context.Sessions.Count());
            TelemetrySession session = _context.Sessions.First();
            Assert.AreEqual(drone.Id, session.DroneId);
            Assert.AreEqual(SessionName, session.Name);
            Assert.IsTrue(session.Start >= start);
            Assert.IsTrue(session.Start <= end);

            Assert.IsTrue(_context.Properties.Any());
            IEnumerable<string> propertyNames = _context.Properties.Select(p => p.Name).Distinct();
            Assert.IsTrue(propertyNames.Count() > 1);

            Assert.IsTrue(_context.DataPoints.Any());
        }

        [TestMethod]
        public void CollectForExistingDroneTest()
        {
            Drone drone = new(){ Name = DroneName };
            _context.Drones.Add(drone);
            _context.SaveChanges();

            _collector.Start(DroneName, SessionName, 1000, null);
            Thread.Sleep(3000);
            _collector.Stop();

            Assert.AreEqual(1, _context.Sessions.Count());
            TelemetrySession session = _context.Sessions.First();
            Assert.AreEqual(drone.Id, session.DroneId);
            Assert.IsTrue(_context.Properties.Any());
            Assert.IsTrue(_context.DataPoints.Any());
        }

        [TestMethod]
        public void  CollectAndLogToConsoleTest()
        {
            string output;

            using (MemoryStream stream = new())
            {
                using (StreamWriter writer = new(stream))
                {
                    TextWriter original = Console.Out;
                    Console.SetOut(writer);

                    _collector.LogToConsole = true;
                    _collector.Start(DroneName, SessionName, 1000, null);
                    Thread.Sleep(3000);
                    _collector.Stop();

                    Console.SetOut(original);
                }

                output = Encoding.UTF8.GetString(stream.ToArray());
            }

            Assert.IsTrue(output.Contains("Writing data for sequence"));
        }
    }
}
