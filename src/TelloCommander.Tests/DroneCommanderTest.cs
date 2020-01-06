using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TelloCommander.CommandDictionaries;
using TelloCommander.Commander;
using TelloCommander.Connections;
using TelloCommander.Exceptions;

namespace TelloCommander.Tests
{
    [TestClass]
    public class DroneCommanderTest
    {
        private MockTelloConnection _connection;
        private DroneCommander _commander;

        [TestInitialize]
        public void TestInitialize()
        {
            CommandDictionary dictionary = CommandDictionary.ReadStandardDictionary("1.3.0.0");
            _connection = new MockTelloConnection(dictionary);
            _commander = new DroneCommander(_connection, dictionary);
        }

        [TestMethod]
        public void GetVersionTest()
        {
            string[] words = DroneCommander.Version.Split(new char[] { '.' });
            Assert.AreEqual(4, words.Length);
            foreach (string word in words)
            {
                Assert.IsTrue(int.TryParse(word, out int value));
            }
        }

        /// <summary>
        /// Condirm that the specified record in the history ends with the specified text
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text"></param>
        private void AssertHistoryEndsWith(int index, string text)
        {
            Assert.IsTrue(_commander.History[index].EndsWith(text, StringComparison.CurrentCulture));
        }

        [TestMethod]
        public void ConnectTest()
        {
            _commander.Connect();

            Assert.AreEqual(2, _commander.History.Count);
            Assert.IsTrue(_commander.History[0].Contains("Tello Commander Version"));
            Assert.IsTrue(_commander.History[0].Contains(DroneCommander.Version));
            Assert.IsTrue(_commander.History[1].Contains("Connected to the drone in API mode"));
        }

        [TestMethod]
        public void DisconnectTest()
        {
            _commander.Disconnect();

            Assert.AreEqual(1, _commander.History.Count);
            Assert.IsTrue(_commander.History[0].Contains("Disconnected"));
        }

        [TestMethod]
        public void RunCommandTest()
        {
            _commander.RunCommand("takeoff");

            Assert.AreEqual("ok", _commander.LastResponse);
            Assert.AreEqual(2, _commander.History.Count);
            AssertHistoryEndsWith(0, "takeoff");
            AssertHistoryEndsWith(1, "ok");
        }

        [TestMethod]
        public void RunCommandWithErrorResponseTest()
        {
            _commander.RunCommand("takeoff");
            _connection.ForceFail = true;
            _commander.RunCommand("cw 180");

            Assert.AreEqual(6, _commander.History.Count);
            AssertHistoryEndsWith(0, "takeoff");
            AssertHistoryEndsWith(1, "ok");
            AssertHistoryEndsWith(2, "cw 180");
            Assert.IsTrue(_commander.History[3].ToLower().Contains("error"));
            AssertHistoryEndsWith(4, "land");
            AssertHistoryEndsWith(5, "ok");
        }

        [TestMethod, ExpectedException(typeof(InvalidCommandException))]
        public void RunEmptyCommandTest()
        {
            _commander.RunCommand("");
        }

        [TestMethod, ExpectedException(typeof(InvalidCommandException))]
        public void RunNullCommandTest()
        {
            _commander.RunCommand(null);
        }

        [TestMethod, ExpectedException(typeof(TooLowToMoveDownException))]
        public void MoveDownTooFarTest()
        {
            _commander.RunCommand("takeoff");
            for (int i  = 0; i < 5; i++)
            {
                _commander.RunCommand("up 50");
            }

            while (true)
            {
                _commander.RunCommand("down 20");
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidArgumentException))]
        public void MoveDownInvalidAmountTest()
        {
            _commander.RunCommand("takeoff");
            _commander.RunCommand("down invalid");
        }

        [TestMethod]
        public void RunScriptTest()
        {
            string script = Path.Combine("Content", "tello.txt");
            _commander.RunScript(script);

            // The history for running a script has extra "Script Folder" and "Start Script"
            // entries at the beginning and an extra "End Script" entry at the end. The test
            // script looks like this:
            //
            // takeoff
            // land
            //
            Assert.AreEqual("ok", _commander.LastResponse);
            Assert.AreEqual(7, _commander.History.Count);
            Assert.IsTrue(_commander.History[0].Contains("Script Folder"));
            Assert.IsTrue(_commander.History[1].Contains("Start Script"));
            AssertHistoryEndsWith(2, "takeoff");
            AssertHistoryEndsWith(3, "ok");
            AssertHistoryEndsWith(4, "land");
            AssertHistoryEndsWith(5, "ok");
            Assert.IsTrue(_commander.History[6].Contains("End Script"));
        }

        [TestMethod]
        public void RunScriptCommandTest()
        {
            string script = Path.Combine("Content", "tello.txt");
            _commander.RunCommand($"runscript {script}");

            // The history for running a script has extra "Script Folder" and "Start Script"
            // entries at the beginning and an extra "End Script" entry at the end. The test
            // script looks like this:
            //
            // takeoff
            // land
            //
            Assert.AreEqual("ok", _commander.LastResponse);
            Assert.AreEqual(7, _commander.History.Count);
            Assert.IsTrue(_commander.History[0].Contains("Script Folder"));
            Assert.IsTrue(_commander.History[1].Contains("Start Script"));
            AssertHistoryEndsWith(2, "takeoff");
            AssertHistoryEndsWith(3, "ok");
            AssertHistoryEndsWith(4, "land");
            AssertHistoryEndsWith(5, "ok");
            Assert.IsTrue(_commander.History[6].Contains("End Script"));
        }

        [TestMethod]
        public void WaitCommandTest()
        {
            DateTime start = DateTime.Now;
            _commander.RunCommand("wait 3");
            int milliseconds = (int)(DateTime.Now - start).TotalMilliseconds;

            Assert.AreEqual("ok", _commander.LastResponse);
            Assert.AreEqual(2, _commander.History.Count);
            AssertHistoryEndsWith(0, "wait 3");
            AssertHistoryEndsWith(1, "ok");
            Assert.IsTrue(milliseconds >= 3000);
        }

        [TestMethod]
        public void WriteHistoryTest()
        {
            string file = Path.GetTempFileName();
            _commander.RunCommand("takeoff");
            _commander.WriteHistory(file);
            string[] history = File.ReadLines(file).ToArray();
            File.Delete(file);

            Assert.AreEqual(2, history.Length);
            AssertHistoryEndsWith(0, "takeoff");
            AssertHistoryEndsWith(1, "ok");
        }

        [TestMethod]
        public void WriteHistoryCommandTest()
        {
            string file = Path.GetTempFileName();
            _commander.RunCommand("takeoff");
            _commander.RunCommand($"writehistory {file}");
            string[] history = File.ReadLines(file).ToArray();
            File.Delete(file);

            Assert.AreEqual(2, history.Length);
            AssertHistoryEndsWith(0, "takeoff");
            AssertHistoryEndsWith(1, "ok");
        }

        [TestMethod]
        public void ForceFailCommandTest()
        {
            _commander.RunCommand("forcefail");
            _commander.RunCommand("takeoff");

            Assert.IsTrue(_commander.LastResponse.Contains("force-failed"));
        }
    }
}
