using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TelloCommander.Simulator;

namespace TelloCommander.Tests
{
    [TestClass]
    [Ignore("Tests in this class work consistently locally but fails intermittently on GitHub")]
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
            _monitor.Dispose();
        }

        [TestMethod]
        public void StatusCaptureTest()
        {
            // Getting the temporary file name creates it. Delete it again to force
            // output of the  headers
            string file = Path.GetTempFileName();
            File.Delete(file);

            string[] lines = CaptureData(file, 2000);
            File.Delete(file);

            ValidateFileContent(lines);
        }

        [TestMethod]
        public void CaptureWithInvalidInterval()
        {
            // Getting the temporary file name creates it. Delete it again to force
            // output of the  headers
            string file = Path.GetTempFileName();
            File.Delete(file);

            // This will apply a default  interval
            string[] lines = CaptureData(file, 0);
            File.Delete(file);

            ValidateFileContent(lines);
        }

        [TestMethod]
        public void RestartCaptureTest()
        {
            // Getting the temporary file name creates it. Delete it again to force
            // output of the  headers
            string file = Path.GetTempFileName();
            File.Delete(file);

            string[] lines = CaptureData(file, 2000);
            int firstLineCount = lines.Length;

            lines = CaptureData(file, 2000);
            File.Delete(file);

            Assert.IsTrue(lines.Length > 0);
            Assert.IsTrue(lines.Length > firstLineCount);
            ValidateFileContent(lines);
        }

        private string[] CaptureData(string file, int duration)
        {
            _monitor.StartCapture(file, 500);
            Thread.Sleep(duration);
            _monitor.StopCapture();
            return File.ReadAllLines(file);
        }

        private void ValidateFileContent(string[] lines)
        {
            Regex header = new Regex(@"^([0-9a-zA-Z_ .]+,){18}[0-9a-zA-Z_ .]+$");
            Regex regex = new Regex(@"^[0-9]{4}(-[0-9]{2}){2} ([0-9]{2}:){2}[0-9]{2}.[0-9]{3},(""[0-9.]+"",){17}""""$");
            for (int i = 0; i < lines.Length; i++)
            {
                bool matches = (i ==0) ?  header.Matches(lines[i]).Any() : regex.Matches(lines[i]).Any();
                Assert.IsTrue(matches);
            }
        }

    }
}
