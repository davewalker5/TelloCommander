using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            string text = acceleration.ToString();

            Assert.AreEqual(-65, acceleration.X);
            Assert.AreEqual(31, acceleration.Y);
            Assert.AreEqual(-994, acceleration.Z);

            Regex regex = new Regex(@"^X: -?[0-9]+.[0-9]+ Y: -?[0-9]+.[0-9]+ Z: -?[0-9]+.[0-9]+$");
            bool matches = regex.Matches(text).Any();
            Assert.IsTrue(matches);
        }

        [TestMethod]
        public void ParseToAttitudeTest()
        {
            Attitude attitude = ResponseParser.ParseToAttitude("pitch:9;roll:-36;yaw:-51;");
            string text = attitude.ToString();

            Assert.AreEqual(9, attitude.Pitch);
            Assert.AreEqual(-36, attitude.Roll);
            Assert.AreEqual(-51, attitude.Yaw);

            Regex regex = new Regex(@"^Pitch: -?[0-9]+ Roll: -?[0-9]+ Yaw: -?[0-9]+$");
            bool matches = regex.Matches(text).Any();
            Assert.IsTrue(matches);
        }

        [TestMethod]
        public void ParseToTemperatureTest()
        {
            Temperature temperature = ResponseParser.ParseToTemperature("60~61C");
            string text = temperature.ToString();

            Assert.AreEqual(60, temperature.Minimum);
            Assert.AreEqual(61, temperature.Maximum);

            Regex regex = new Regex(@"^Minimum: -?[0-9]+ Maximum: -?[0-9]+$");
            bool matches = regex.Matches(text).Any();
            Assert.IsTrue(matches);
        }

        [TestMethod]
        public void ParseTemperatureFromPropertyListTest()
        {
            Temperature temperature = ResponseParser.ParseTemperatureFromPropertyList("templ:60;temph:61;");
            string text = temperature.ToString();

            Assert.AreEqual(60, temperature.Minimum);
            Assert.AreEqual(61, temperature.Maximum);

            Regex regex = new Regex(@"^Minimum: -?[0-9]+ Maximum: -?[0-9]+$");
            bool matches = regex.Matches(text).Any();
            Assert.IsTrue(matches);
        }

        [TestMethod]
        public void ParseToSpeedTest()
        {
            Speed speed = ResponseParser.ParseToSpeed("vgx:10.00;vgy:11.00;vgz:12.00;");
            string text = speed.ToString();

            Assert.AreEqual(10, speed.X);
            Assert.AreEqual(11, speed.Y);
            Assert.AreEqual(12, speed.Z);

            Regex regex = new Regex(@"^X: -?[0-9]+.[0-9]+ Y: -?[0-9]+.[0-9]+ Z: -?[0-9]+.[0-9]+$");
            bool matches = regex.Matches(text).Any();
            Assert.IsTrue(matches);
        }
    }
}
