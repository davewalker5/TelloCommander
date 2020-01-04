using Microsoft.VisualStudio.TestTools.UnitTesting;
using TelloCommander.CommandDictionaries;
using TelloCommander.Connections;

namespace TelloCommander.Tests
{
    [TestClass]
    public class MockTelloConnectionTest
    {
        private MockTelloConnection _connection;

        [TestInitialize]
        public void TestInitialize()
        {
            CommandDictionary dictionary = CommandDictionary.ReadStandardDictionary("1.3.0.0");
            _connection = new MockTelloConnection(dictionary);
        }

        [TestMethod]
        public void InitialHeightIsZeroTest()
        {
            Assert.AreEqual(0, _connection.Height);
        }

        [TestMethod]
        public void TakeOffHeightTest()
        {
            _connection.SendCommand("takeoff");
            Assert.IsTrue(_connection.Height > 0);
        }

        [TestMethod]
        public void MoveUpHeightTest()
        {
            _connection.SendCommand("takeoff");
            int takeOffHeight = _connection.Height;

            _connection.SendCommand("up 50");
            int expected = (10 * takeOffHeight + 50) / 10;
            Assert.AreEqual(expected, _connection.Height);
        }

        [TestMethod]
        public void MoveDownHeightTest()
        {
            _connection.SendCommand("takeoff");
            int takeOffHeight = _connection.Height;

            _connection.SendCommand("up 50");
            _connection.SendCommand("down 50");
            Assert.AreEqual(takeOffHeight, _connection.Height);
        }

        [TestMethod]
        public void GetHeightTest()
        {
            _connection.SendCommand("takeoff");
            string expected = $"{_connection.Height}dm";
            string actual = _connection.SendCommand("height?");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ForceFailCommandTest()
        {
            _connection.ForceFail = true;
            string response = _connection.SendCommand("takeoff");
            Assert.IsTrue(response.ToLower().Contains("error"));
        }
    }
}
