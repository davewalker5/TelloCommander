using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TelloCommander.CommandDictionaries;
using TelloCommander.Connections;
using TelloCommander.Response;

namespace TelloCommander.Tests
{
    [TestClass]
    public class MockDroneTest
    {
        private MockDrone _drone;

        [TestInitialize]
        public void TestInitialise()
        {
            CommandDictionary dictionary = CommandDictionary.ReadStandardDictionary("1.3.0.0");
            _drone = new MockDrone(dictionary);
        }

        [TestMethod]
        public void TakeoffTest()
        {
            Assert.AreEqual(0, _drone.Height);
            string response = _drone.ConstructCommandResponse("takeoff");
            Assert.AreEqual("ok", response);
            Assert.IsTrue(_drone.Height > 0);
        }

        [TestMethod]
        public void LandTest()
        {
            _drone.ConstructCommandResponse("takeoff");
            string response = _drone.ConstructCommandResponse("land");
            Assert.AreEqual("ok", response);
            Assert.AreEqual(0, _drone.Height);
        }

        [TestMethod]
        public void NoTimeOfFlightTest()
        {
            Assert.AreEqual(0, _drone.TimeOfFlight);
        }

        [TestMethod]
        public void TimeOfFlightTest()
        {
            DateTime start = DateTime.Now;
            _drone.ConstructCommandResponse("takeoff");
            Thread.Sleep(3000);
            int seconds = (int)(DateTime.Now - start).TotalSeconds;
            Assert.IsTrue(seconds >= 3);
        }

        [TestMethod]
        public void EmergencyStopTest()
        {
            _drone.ConstructCommandResponse("takeoff");
            string response = _drone.ConstructCommandResponse("emergency");
            Assert.AreEqual("ok", response);
            Assert.AreEqual(0, _drone.Height);
        }

        [TestMethod]
        public void MoveUpTest()
        {
            _drone.ConstructCommandResponse("takeoff");
            int height = _drone.Height;
            _drone.ConstructCommandResponse("up 100");
            int expected = ((height * 10) + 100) / 10;
            Assert.AreEqual(expected, _drone.Height);
        }

        [TestMethod]
        public void RotateClockwiseTest()
        {
            _drone.ConstructCommandResponse("takeoff");
            Assert.AreEqual(0, _drone.Heading);
            _drone.ConstructCommandResponse("cw 45");
            Assert.AreEqual(45, _drone.Heading);
        }

        [TestMethod]
        public void RotateAntiClockwiseTest()
        {
            _drone.ConstructCommandResponse("takeoff");
            Assert.AreEqual(0, _drone.Heading);
            _drone.ConstructCommandResponse("ccw 45");
            Assert.AreEqual(315, _drone.Heading);
        }

        [TestMethod]
        public void CurveTest()
        {
            _drone.ConstructCommandResponse("takeoff");
            string response = _drone.ConstructCommandResponse("curve 20 20 20 20 100 20 10");
            Assert.AreEqual("ok", response);
            Assert.AreEqual(10, _drone.Height);
        }

        [TestMethod]
        public void GoTest()
        {
            _drone.ConstructCommandResponse("takeoff");
            string response = _drone.ConstructCommandResponse("go 20 100 20 10");
            Assert.AreEqual("ok", response);
            Assert.AreEqual(10, _drone.Height);
        }

        [TestMethod]
        public void StopSimulatorTest()
        {
            Assert.IsFalse(_drone.Stop);
            string response = _drone.ConstructCommandResponse("stopsimulator");
            Assert.AreEqual("ok", response);
            Assert.IsTrue(_drone.Stop);
        }

        [TestMethod]
        public void ResponseDelayTest()
        {
            Assert.AreEqual(0, _drone.ResponseDelay);
            string response = _drone.ConstructCommandResponse("responsedelay 10");
            Assert.AreEqual("ok", response);
            Assert.AreEqual(10, _drone.ResponseDelay);
        }

        [TestMethod]
        public void GetStatusTest()
        {
            _drone.ConstructCommandResponse("takeoff");
            string status = _drone.GetStatus();

            Dictionary<string, string> properties = ResponseParser.ParseToDictionary(status);
            Assert.AreEqual("0", properties["pitch"]);
            Assert.AreEqual("0", properties["roll"]);
            Assert.AreEqual("0", properties["yaw"]);
            Assert.AreEqual("0", properties["vgx"]);
            Assert.AreEqual("0", properties["vgy"]);
            Assert.AreEqual("0", properties["vgz"]);
            Assert.AreEqual("0", properties["templ"]);
            Assert.AreEqual("0", properties["temph"]);
            Assert.AreEqual("0", properties["tof"]);
            Assert.AreEqual("0", properties["bat"]);
            Assert.AreEqual("0.00", properties["baro"]);
            Assert.AreEqual("0", properties["time"]);
            Assert.AreEqual("0.00", properties["agx"]);
            Assert.AreEqual("0.00", properties["agy"]);
            Assert.AreEqual("0.00", properties["agz"]);

            decimal height = decimal.Parse(properties["h"]);
            Assert.AreEqual(_drone.Height, height);
        }

        [TestMethod]
        public void PositionXTest()
        {
            _drone.ConstructCommandResponse("takeoff");
            Assert.AreEqual(0, _drone.Position.X);
            _drone.ConstructCommandResponse("left 50");
            Assert.AreEqual(-50, _drone.Position.X);
            _drone.ConstructCommandResponse("right 100");
            Assert.AreEqual(50, _drone.Position.X);
        }

        [TestMethod]
        public void PositionZTest()
        {
            _drone.ConstructCommandResponse("takeoff");
            Assert.AreEqual(0, _drone.Position.Z);
            _drone.ConstructCommandResponse("forward 50");
            Assert.AreEqual(-50, _drone.Position.Z);
            _drone.ConstructCommandResponse("back 100");
            Assert.AreEqual(50, _drone.Position.Z);
        }

        [TestMethod]
        public void PositionYTest()
        {
            Assert.AreEqual(0, _drone.Position.Y);
            _drone.ConstructCommandResponse("takeoff");
            Assert.AreEqual(60, _drone.Position.Y);
        }

        [TestMethod]
        public void ParseToStringTest()
        {
            // Populate the position with some data rather than leaving it at 0,0,0
            _drone.ConstructCommandResponse("takeoff");
            _drone.ConstructCommandResponse("forward 50");
            _drone.ConstructCommandResponse("right 50");

            string text = _drone.Position.ToString();
            string csv = _drone.Position.ToCsv();

            Regex regex = new Regex(@"^X: -?[0-9]+ Y: -?[0-9]+ Z: -?[0-9]+$");
            bool matches = regex.Matches(text).Any();
            Assert.IsTrue(matches);

            regex = new Regex(@"^""-?[0-9]+"",""-?[0-9]+"",""-?[0-9]+""$");
            matches = regex.Matches(csv).Any();
            Assert.IsTrue(matches);
        }

        [TestMethod]
        public void LandWithoutTakeoffTest()
        {
            string response = _drone.ConstructCommandResponse("land");
            Assert.IsTrue(response.ToLower().Contains("not in flight"));
        }

        [TestMethod]
        public void EmergencyWithoutTakeoffTest()
        {
            string response = _drone.ConstructCommandResponse("emergency");
            Assert.IsTrue(response.ToLower().Contains("not in flight"));
        }

        [TestMethod]
        public void MoveUpWithoutTakeoffTest()
        {
            string response = _drone.ConstructCommandResponse("up 50");
            Assert.IsTrue(response.ToLower().Contains("not in flight"));
        }

        [TestMethod]
        public void MoveDownWithoutTakeoffTest()
        {
            string response = _drone.ConstructCommandResponse("down 50");
            Assert.IsTrue(response.ToLower().Contains("not in flight"));
        }

        [TestMethod]
        public void MoveForwardWithoutTakeoffTest()
        {
            string response = _drone.ConstructCommandResponse("forward 50");
            Assert.IsTrue(response.ToLower().Contains("not in flight"));
        }

        [TestMethod]
        public void MoveBackWithoutTakeoffTest()
        {
            string response = _drone.ConstructCommandResponse("back 50");
            Assert.IsTrue(response.ToLower().Contains("not in flight"));
        }

        [TestMethod]
        public void MoveLeftWithoutTakeoffTest()
        {
            string response = _drone.ConstructCommandResponse("left 50");
            Assert.IsTrue(response.ToLower().Contains("not in flight"));
        }

        [TestMethod]
        public void MoveRightWithoutTakeoffTest()
        {
            string response = _drone.ConstructCommandResponse("right 50");
            Assert.IsTrue(response.ToLower().Contains("not in flight"));
        }

        [TestMethod]
        public void CurveWithoutTakeoffTest()
        {
            string response = _drone.ConstructCommandResponse("curve 20 20 20 20 100 20 10");
            Assert.IsTrue(response.ToLower().Contains("not in flight"));
        }

        [TestMethod]
        public void GoWithoutTakeoffTest()
        {
            string response = _drone.ConstructCommandResponse("go 20 100 20 10");
            Assert.IsTrue(response.ToLower().Contains("not in flight"));
        }

        [TestMethod]
        public void RotateClockwiseWithoutTakeoffTest()
        {
            string response = _drone.ConstructCommandResponse("cw 180");
            Assert.IsTrue(response.ToLower().Contains("not in flight"));
        }

        [TestMethod]
        public void RotateAnticlockwiseWithoutTakeoffTest()
        {
            string response = _drone.ConstructCommandResponse("ccw 180");
            Assert.IsTrue(response.ToLower().Contains("not in flight"));
        }
    }
}
