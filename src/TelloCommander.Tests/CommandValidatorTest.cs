using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TelloCommander.CommandDictionaries;
using TelloCommander.Exceptions;

namespace TelloCommander.Tests
{
    [TestClass]
    public class CommandValidatorTest
    {
        private CommandDictionary _dictionary;
        private CommandValidator _validator;

        [TestInitialize]
        public void TestInitialise()
        {
            _dictionary = CommandDictionary.ReadStandardDictionary("1.3.0.0");
            _validator = new CommandValidator(_dictionary);
        }

        /// <summary>
        /// Construct a valid command from the spdecified definition
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private string[] ConstructCommand(CommandDefinition command)
        {
            char[] separators = { ' ' };
            StringBuilder builder = new StringBuilder(command.Name);
            foreach (ArgumentDefinition argument in command.Arguments)
            {
                builder.Append(" ");
                if (argument.ArgumentType == ArgumentType.Number)
                {
                    if (argument.Minimum != null)
                    {
                        builder.Append(argument.Minimum);
                    }
                    else if (argument.Maximum != null)
                    {
                        builder.Append(argument.Maximum);
                    }
                    else
                    {
                        builder.Append("0");
                    }
                }
                else if (argument.AllowedValues.Any())
                {
                    builder.Append(argument.AllowedValues[0]);
                }
                else
                {
                    builder.Append("test");
                }
            }

            string[] words = builder.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return words;
        }

        [TestMethod]
        public void ValidateValidCommandTest()
        {
            foreach (CommandDefinition command in _dictionary.Commands)
            {
                string[] words = ConstructCommand(command);
                _validator.ValidateCommand(command.ConnectionType, words);
            }

            // If we get here, it's not thrown any exceptions while attempting the whole
            // dictionary so the test passes
        }

        [TestMethod, ExpectedException(typeof(InvalidCommandException))]
        public void InvalidCommandTest()
        {
            string[] words = { "not-a-valid-command" };
            _validator.ValidateCommand(ConnectionType.Mock, words);
        }

        [TestMethod, ExpectedException(typeof(CommandNotValidForConnectionTypeException))]
        public void CommandInvalidForConnectionTypeTest()
        {
            string[] words = { "stopsimulator" };
            _validator.ValidateCommand(ConnectionType.Mock, words);
        }

        [TestMethod, ExpectedException(typeof(InvalidArgumentCountException))]
        public void TooManyArgumentsTest()
        {
            CommandDefinition command = _dictionary.Commands.First(c => (c.ConnectionType == ConnectionType.Any) && (c.Arguments.Count > 0));
            string[] words = ConstructCommand(command);
            words = words.Append("this-should-not-be-here").ToArray();
            _validator.ValidateCommand(ConnectionType.Mock, words);
        }

        [TestMethod, ExpectedException(typeof(InvalidArgumentCountException))]
        public void TooFewArgumentsTest()
        {
            CommandDefinition command = _dictionary.Commands.First(c => (c.ConnectionType == ConnectionType.Any) && (c.Arguments.Count > 0));
            string[] words = { command.Name };
            _validator.ValidateCommand(ConnectionType.Mock, words);
        }

        [TestMethod, ExpectedException(typeof(InvalidArgumentException))]
        public void InvalidNumericArgumentTest()
        {
            CommandDefinition command = _dictionary.Commands.First(c => (c.ConnectionType == ConnectionType.Any) && (c.Arguments.Count == 1) && (c.Arguments[0].ArgumentType == ArgumentType.Number));
            string[] words = { command.Name, "not-a-valid-number" };
            _validator.ValidateCommand(ConnectionType.Mock, words);
        }

        [TestMethod, ExpectedException(typeof(ValueOutOfRangeException))]
        public void ValueTooSmallTest()
        {
            CommandDefinition command = _dictionary.Commands.First(c => (c.ConnectionType == ConnectionType.Any) && (c.Arguments.Count == 1) && (c.Arguments[0].ArgumentType == ArgumentType.Number) && (c.Arguments[0].Minimum != null));
            decimal testValue = (command.Arguments[0].Minimum ?? 0 ) - 1;
            string[] words = { command.Name, testValue.ToString() };
            _validator.ValidateCommand(ConnectionType.Mock, words);
        }

        [TestMethod, ExpectedException(typeof(ValueOutOfRangeException))]
        public void ValueTooLargeTest()
        {
            CommandDefinition command = _dictionary.Commands.First(c => (c.ConnectionType == ConnectionType.Any) && (c.Arguments.Count == 1) && (c.Arguments[0].ArgumentType == ArgumentType.Number) && (c.Arguments[0].Maximum != null));
            decimal testValue = (command.Arguments[0].Maximum ?? 0) + 1;
            string[] words = { command.Name, testValue.ToString() };
            _validator.ValidateCommand(ConnectionType.Mock, words);
        }

        [TestMethod, ExpectedException(typeof(InvalidArgumentException))]
        public void NotAllowedArgumentTest()
        {
            CommandDefinition command = _dictionary.Commands.First(c => (c.ConnectionType == ConnectionType.Any) && (c.Arguments.Count == 1) && (c.Arguments[0].ArgumentType == ArgumentType.String) && c.Arguments[0].AllowedValues.Any());
            string[] words = { command.Name, "this-is-not-an-allowed-value" };
            _validator.ValidateCommand(ConnectionType.Mock, words);
        }
    }
}
