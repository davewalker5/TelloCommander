using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TelloCommander.Response;

namespace TelloCommander.Tests
{
    [TestClass]
    public class ResponseParserTest
    {
        [TestMethod]
        public void ParseToNumberTest()
        {
            decimal result = ResponseParser.ParseToNumber("6dm");
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void ParseToRangeTest()
        {
            (decimal minimum, decimal maximum) range = ResponseParser.ParseToRange("60~61C");
            Assert.AreEqual(60, range.minimum);
            Assert.AreEqual(61, range.maximum);
        }

        [TestMethod]
        public void ParseToDictionaryTest()
        {
            Dictionary<string, string> dictionary = ResponseParser.ParseToDictionary("pitch:9;roll:-36;yaw:-51;");
            Assert.AreEqual("9", dictionary["pitch"]);
            Assert.AreEqual("-36", dictionary["roll"]);
            Assert.AreEqual("-51", dictionary["yaw"]);
        }

        [TestMethod]
        public void ParseToAccelerationTest()
        {
            Acceleration acceleration = ResponseParser.ParseToAcceleration("agx:-65.00;agy:31.00;agz:-994.00;");
            Assert.AreEqual(-65, acceleration.X);
            Assert.AreEqual(31, acceleration.Y);
            Assert.AreEqual(-994, acceleration.Z);
        }

        [TestMethod]
        public void ParseToAttitudeTest()
        {
            Attitude attitude = ResponseParser.ParseToAttitude("pitch:9;roll:-36;yaw:-51;");
            Assert.AreEqual(9, attitude.Pitch);
            Assert.AreEqual(-36, attitude.Roll);
            Assert.AreEqual(-51, attitude.Yaw);
        }

        [TestMethod]
        public void ParseToTemperatureTest()
        {
            Temperature temperature = ResponseParser.ParseToTemperature("60~61C");
            Assert.AreEqual(60, temperature.Minimum);
            Assert.AreEqual(61, temperature.Maximum);
        }
    }
}
