using Microsoft.VisualStudio.TestTools.UnitTesting;
using TelloCommander.CommandDictionaries;
using TelloCommander.Connections;

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
        public void EmergencyStopTest()
        {
            string response = _drone.ConstructCommandResponse("takeoff");
            response = _drone.ConstructCommandResponse("emergency");
            Assert.AreEqual("ok", response);
            Assert.AreEqual(0, _drone.Height);
        }

        [TestMethod]
        public void MoveUpTest()
        {
            string response = _drone.ConstructCommandResponse("takeoff");
            int height = _drone.Height;
            response = _drone.ConstructCommandResponse("up 100");
            int expected = ((height * 10) + 100) / 10;
            Assert.AreEqual(expected, _drone.Height);
        }

        [TestMethod]
        public void CurveTest()
        {
            string response = _drone.ConstructCommandResponse("curve 20 20 20 20 100 20 10");
            Assert.AreEqual("ok", response);
            Assert.AreEqual(10, _drone.Height);
        }

        [TestMethod]
        public void GoTest()
        {
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
    }
}
