using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TelloCommander.Simulator;

namespace TelloCommander.Tests
{
    [TestClass]
    public class MockStatusMonitorTest
    {
        private MockDroneStatusMonitor _monitor;

        [TestInitialize]
        public void TestInitialise()
        {
            _monitor = new MockDroneStatusMonitor();
            _monitor.Listen(0);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _monitor.Stop();
        }

        [TestMethod]
        public void StatusCaptureTest()
        {
            string file = Path.GetTempFileName();
            _monitor.StartCapture(file, 1000);
            Thread.Sleep(5000);
            _monitor.StopCapture();

            string[] lines = File.ReadAllLines(file);
            File.Delete(file);

            Assert.IsTrue(lines.Length > 0);

            Regex regex = new Regex(@"^[0-9]{4}(-[0-9]{2}){2} ([0-9]{2}:){2}[0-9]{2}.[0-9]{3},(""[0-9.]+"",){17}""""$");
            foreach (string line in lines)
            {
                bool matches = regex.Matches(line).Any();
                Assert.IsTrue(matches);
            }
        }
    }
}
