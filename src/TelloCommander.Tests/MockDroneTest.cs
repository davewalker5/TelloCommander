using System;
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
        public void EmergencyStopTest()
        {
            Assert.AreEqual(0, _drone.Height);
            string response = _drone.ConstructCommandResponse("takeoff");
            Assert.AreEqual("ok", response);
            Assert.IsTrue(_drone.Height > 0);

            response = _drone.ConstructCommandResponse("emergency");
            Assert.AreEqual("ok", response);
            Assert.AreEqual(0, _drone.Height);
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
