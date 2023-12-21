using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using TelloCommander.Response;

namespace TelloCommander.Tests
{
    [TestClass]
    [SuppressMessage("GeneratedRegex", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
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
            (decimal minimum, decimal maximum) = ResponseParser.ParseToRange("60~61C");
            Assert.AreEqual(60, minimum);
            Assert.AreEqual(61, maximum);
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
            string csv = acceleration.ToCsv();

            Assert.AreEqual(-65, acceleration.X);
            Assert.AreEqual(31, acceleration.Y);
            Assert.AreEqual(-994, acceleration.Z);

            Regex regex = new(@"^X: -?[0-9]+.[0-9]+ Y: -?[0-9]+.[0-9]+ Z: -?[0-9]+.[0-9]+$");
            bool matches = regex.Matches(text).Any();
            Assert.IsTrue(matches);

            regex = new(@"^""-?[0-9]+.[0-9]+"",""-?[0-9]+.[0-9]+"",""-?[0-9]+.[0-9]+""$");
            matches = regex.Matches(csv).Any();
            Assert.IsTrue(matches);
        }

        [TestMethod]
        public void ParseToAttitudeTest()
        {
            Attitude attitude = ResponseParser.ParseToAttitude("pitch:9;roll:-36;yaw:-51;");
            string text = attitude.ToString();
            string csv = attitude.ToCsv();

            Assert.AreEqual(9, attitude.Pitch);
            Assert.AreEqual(-36, attitude.Roll);
            Assert.AreEqual(-51, attitude.Yaw);

            Regex regex = new(@"^Pitch: -?[0-9]+ Roll: -?[0-9]+ Yaw: -?[0-9]+$");
            bool matches = regex.Matches(text).Any();
            Assert.IsTrue(matches);

            regex = new(@"^""-?[0-9]+"",""-?[0-9]+"",""-?[0-9]+""$");
            matches = regex.Matches(csv).Any();
            Assert.IsTrue(matches);
        }

        [TestMethod]
        public void ParseToTemperatureTest()
        {
            Temperature temperature = ResponseParser.ParseToTemperature("60~61C");
            string text = temperature.ToString();
            string csv = temperature.ToCsv();

            Assert.AreEqual(60, temperature.Minimum);
            Assert.AreEqual(61, temperature.Maximum);

            Regex regex = new(@"^Minimum: -?[0-9]+ Maximum: -?[0-9]+$");
            bool matches = regex.Matches(text).Any();
            Assert.IsTrue(matches);

            regex = new(@"^""-?[0-9]+"",""-?[0-9]+""$");
            matches = regex.Matches(csv).Any();
            Assert.IsTrue(matches);
        }

        [TestMethod]
        public void ParseTemperatureFromPropertyListTest()
        {
            Temperature temperature = ResponseParser.ParseTemperatureFromPropertyList("templ:60;temph:61;");
            string text = temperature.ToString();
            string csv = temperature.ToCsv();

            Assert.AreEqual(60, temperature.Minimum);
            Assert.AreEqual(61, temperature.Maximum);

            Regex regex = new(@"^Minimum: -?[0-9]+ Maximum: -?[0-9]+$");
            bool matches = regex.Matches(text).Any();
            Assert.IsTrue(matches);

            regex = new(@"^""-?[0-9]+"",""-?[0-9]+""$");
            matches = regex.Matches(csv).Any();
            Assert.IsTrue(matches);
        }

        [TestMethod]
        public void ParseToSpeedTest()
        {
            Speed speed = ResponseParser.ParseToSpeed("vgx:10.00;vgy:11.00;vgz:12.00;");
            string text = speed.ToString();
            string csv = speed.ToCsv();

            Assert.AreEqual(10, speed.X);
            Assert.AreEqual(11, speed.Y);
            Assert.AreEqual(12, speed.Z);

            Regex regex = new(@"^X: -?[0-9]+.[0-9]+ Y: -?[0-9]+.[0-9]+ Z: -?[0-9]+.[0-9]+$");
            bool matches = regex.Matches(text).Any();
            Assert.IsTrue(matches);

            regex = new(@"^""-?[0-9]+.[0-9]+"",""-?[0-9]+.[0-9]+"",""-?[0-9]+.[0-9]+""$");
            matches = regex.Matches(csv).Any();
            Assert.IsTrue(matches);
        }
    }
}
