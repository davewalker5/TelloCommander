using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TelloCommander.CommandDictionaries;
using TelloCommander.Exceptions;

namespace TelloCommander.Tests
{
    [TestClass]
    public class CommandDictionaryTest
    {
        private string _location;

        [TestInitialize]
        public void TestInitialise()
        {
            string location = Assembly.GetExecutingAssembly().Location;
            _location = Path.GetDirectoryName(location);
        }

        [TestMethod]
        public void GetVersionTest()
        {
            string version = CommandDictionary.GetAvailableDictionaryVersions()[0];
            CommandDictionary dictionary = CommandDictionary.ReadStandardDictionary(version);
            Assert.AreEqual(version, dictionary.Version);
        }

        [TestMethod]
        public void GetMockResponseTest()
        {
            string[] versions = CommandDictionary.GetAvailableDictionaryVersions();
            CommandDictionary dictionary = CommandDictionary.ReadStandardDictionary(versions[0]);
  
            foreach (CommandDefinition command in dictionary.Commands)
            {
                string response = dictionary.GetMockResponse(command.Name);
                Assert.AreEqual(command.MockResponse, response);
            }
        }

        [TestMethod]
        public void WriteAndReadCommandWithArgumentsTest()
        {
            CommandDefinition definition = new CommandDefinition
            {
                Name = "move",
                ConnectionType = ConnectionType.Any,
                ResponseType = ResponseType.OK,
                IsCustomCommand = false,
                MockResponse = "ok",
                Arguments = new List<ArgumentDefinition>
                {
                    new ArgumentDefinition
                    {
                        Name = "direction",
                        ArgumentType = ArgumentType.String,
                        Required = true,
                        AllowedValues = { "l", "r", "f", "b" }
                    },
                    new ArgumentDefinition
                    {
                        Name = "distance",
                        ArgumentType = ArgumentType.Number,
                        Required = true,
                        Minimum = 50,
                        Maximum = 500
                    }
                }
            };

            CommandDictionary dictionary = new CommandDictionary();
            dictionary.Commands.Add(definition);

            string file = Path.GetTempFileName();
            dictionary.Write(file);

            CommandDictionary read = CommandDictionary.Read(file);
            File.Delete(file);

            Assert.AreEqual(1, read.Commands.Count);
            Assert.AreEqual("move", read.Commands[0].Name);
            Assert.AreEqual(ResponseType.OK, read.Commands[0].ResponseType);
            Assert.IsFalse(read.Commands[0].IsCustomCommand);
            Assert.AreEqual("ok", read.Commands[0].MockResponse);
        }

        [TestMethod]
        public void WriteAndReadCommandWithNoArgumentsTest()
        {
            CommandDefinition definition = new CommandDefinition
            {
                Name = "command",
                ConnectionType = ConnectionType.Any,
                ResponseType = ResponseType.OK,
                IsCustomCommand = false,
                MockResponse = "ok"
            };

            CommandDictionary dictionary = new CommandDictionary();
            dictionary.Commands.Add(definition);

            string file = Path.GetTempFileName();
            dictionary.Write(file);

            CommandDictionary read = CommandDictionary.Read(file);
            File.Delete(file);

            Assert.AreEqual(1, read.Commands.Count);
            Assert.AreEqual("command", read.Commands[0].Name);
            Assert.AreEqual(ResponseType.OK, read.Commands[0].ResponseType);
            Assert.IsFalse(read.Commands[0].IsCustomCommand);
            Assert.AreEqual("ok", read.Commands[0].MockResponse);
        }

        [TestMethod]
        public void ReadStandardDictionaryTest()
        {
            string[] versions = CommandDictionary.GetAvailableDictionaryVersions();
            Assert.IsTrue(versions.Length > 0);

            foreach (string version in versions)
            {
                CommandDictionary dictionary = CommandDictionary.ReadStandardDictionary(version);
                Assert.IsTrue(dictionary.Commands.Count > 0);
                Assert.AreEqual(version, dictionary.Version);
            }
        }

        [TestMethod]
        public void GetAvailableDictionaryVersionsTest()
        {
            string[] versions = CommandDictionary.GetAvailableDictionaryVersions();
            Assert.AreEqual(2, versions.Length);
            Assert.IsTrue(versions.Contains("1.3.0.0"));
            Assert.IsTrue(versions.Contains("2.0.0.0"));
        }

        [TestMethod, ExpectedException(typeof(InvalidDictionaryXmlException))]
        public  void EmptyDictionaryTest()
        {
            string file = Path.Combine(_location, "Content", "EmptyDictionary.xml");
            CommandDictionary.Read(file);
        }

        [TestMethod, ExpectedException(typeof(InvalidDictionaryXmlException))]
        public void EmptyCommandsSectionTest()
        {
            string file = Path.Combine(_location, "Content", "EmptyCommandsSection.xml");
            CommandDictionary.Read(file);
        }

        [TestMethod, ExpectedException(typeof(InvalidDictionaryXmlException))]
        public void EmptyCommandDefinitionTest()
        {
            string file = Path.Combine(_location, "Content", "EmptyCommandDefinition.xml");
            CommandDictionary.Read(file);
        }

        [TestMethod, ExpectedException(typeof(InvalidDictionaryXmlException))]
        public void MissingCommandNameTest()
        {
            string file = Path.Combine(_location, "Content", "MissingCommandName.xml");
            CommandDictionary.Read(file);
        }

        [TestMethod, ExpectedException(typeof(InvalidDictionaryXmlException))]
        public void MissingConnectionTypeTest()
        {
            string file = Path.Combine(_location, "Content", "MissingConnectionType.xml");
            CommandDictionary.Read(file);
        }

        [TestMethod, ExpectedException(typeof(InvalidDictionaryXmlException))]
        public void InvalidConnectionTypeTest()
        {
            string file = Path.Combine(_location, "Content", "InvalidConnectionType.xml");
            CommandDictionary.Read(file);
        }

        [TestMethod, ExpectedException(typeof(InvalidDictionaryXmlException))]
        public void MissingResponseTypeTest()
        {
            string file = Path.Combine(_location, "Content", "MissingResponseType.xml");
            CommandDictionary.Read(file);
        }

        [TestMethod, ExpectedException(typeof(InvalidDictionaryXmlException))]
        public void InvalidResponseTypeTest()
        {
            string file = Path.Combine(_location, "Content", "InvalidResponseType.xml");
            CommandDictionary.Read(file);
        }

        [TestMethod, ExpectedException(typeof(InvalidDictionaryXmlException))]
        public void MissingIsCustomCommandTest()
        {
            string file = Path.Combine(_location, "Content", "MissingIsCustomCommand.xml");
            CommandDictionary.Read(file);
        }

        [TestMethod, ExpectedException(typeof(InvalidDictionaryXmlException))]
        public void MissingMockResponseTest()
        {
            string file = Path.Combine(_location, "Content", "MissingMockResponse.xml");
            CommandDictionary.Read(file);
        }

        [TestMethod]
        public void MissingArgumentsTest()
        {
            string file = Path.Combine(_location, "Content", "MissingArguments.xml");
            CommandDictionary dictionary = CommandDictionary.Read(file);
            Assert.AreEqual(1, dictionary.Commands.Count);
            Assert.AreEqual(0, dictionary.Commands[0].Arguments.Count);
        }

        [TestMethod, ExpectedException(typeof(InvalidDictionaryXmlException))]
        public void MissingArgumentNameTest()
        {
            string file = Path.Combine(_location, "Content", "MissingArgumentName.xml");
            CommandDictionary.Read(file);
        }

        [TestMethod, ExpectedException(typeof(InvalidDictionaryXmlException))]
        public void MissingArgumentTypeTest()
        {
            string file = Path.Combine(_location, "Content", "MissingArgumentType.xml");
            CommandDictionary.Read(file);
        }

        [TestMethod, ExpectedException(typeof(InvalidDictionaryXmlException))]
        public void InvalidArgumentTypeTest()
        {
            string file = Path.Combine(_location, "Content", "InvalidArgumentType.xml");
            CommandDictionary.Read(file);
        }

        [TestMethod, ExpectedException(typeof(InvalidDictionaryXmlException))]
        public void MissingRequiredTest()
        {
            string file = Path.Combine(_location, "Content", "MissingRequired.xml");
            CommandDictionary.Read(file);
        }

        [TestMethod, ExpectedException(typeof(OptionalArgumentPositionException))]
        public void OptionalArgumentPositionErrorTest()
        {
            string file = Path.Combine(_location, "Content", "OptionalArgumentPositionError.xml");
            CommandDictionary.Read(file);
        }

        [TestMethod, ExpectedException(typeof(InvalidValueRangeException))]
        public void InvalidArgumentValueRangeTest()
        {
            string file = Path.Combine(_location, "Content", "InvalidArgumentValueRange.xml");
            CommandDictionary.Read(file);
        }

        [TestMethod]
        public void MissingMinimumValueTest()
        {
            string file = Path.Combine(_location, "Content", "MissingMinimumValue.xml");
            CommandDictionary dictionary = CommandDictionary.Read(file);
            Assert.AreEqual(1, dictionary.Commands[0].Arguments.Count);
            Assert.IsNull(dictionary.Commands[0].Arguments[0].Minimum);
        }

        [TestMethod]
        public void MissingMaximumValueTest()
        {
            string file = Path.Combine(_location, "Content", "MissingMaximumValue.xml");
            CommandDictionary dictionary = CommandDictionary.Read(file);
            Assert.AreEqual(1, dictionary.Commands[0].Arguments.Count);
            Assert.IsNull(dictionary.Commands[0].Arguments[0].Maximum);
        }

        [TestMethod]
        public void MissingAllowedValuesTest()
        {
            string file = Path.Combine(_location, "Content", "MissingAllowedValues.xml");
            CommandDictionary dictionary = CommandDictionary.Read(file);
            Assert.AreEqual(1, dictionary.Commands[0].Arguments.Count);
            Assert.AreEqual(0, dictionary.Commands[0].Arguments[0].AllowedValues.Count);
        }
    }
}
