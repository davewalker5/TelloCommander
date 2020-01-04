using System.Collections.Generic;
using System.Linq;
using TelloCommander.Exceptions;

namespace TelloCommander.CommandDictionaries
{
    internal class CommandValidator
    {
        public CommandDictionary Dictionary { get; private set; }

        public CommandValidator(CommandDictionary dictionary)
        {
            Dictionary = dictionary;
        }

        /// <summary>
        /// Validate a command and its arguments represented by the array of words for the
        /// specified connection type
        /// </summary>
        /// <param name="connectionType"></param>
        /// <param name="words"></param>
        public void ValidateCommand(ConnectionType connectionType, string[] words)
        {
            CommandDefinition command = Dictionary.Commands.FirstOrDefault(c => c.Name.Equals(words[0]));
            if (command != null)
            {
                // Check the command is valid for the specified connection type
                if ((command.ConnectionType != ConnectionType.Any) && (command.ConnectionType != connectionType))
                {
                    string message = $"Command '{command.Name}' is not valid for connection type {connectionType.ToString()}";
                    throw new CommandNotValidForConnectionTypeException(message);
                }

                // Check the required number of arguments has been given
                if (command.Arguments.Count == words.Length - 1)
                {
                    for (int i = 1; i < words.Length; i++)
                    {
                        ValidateArgument(i, command.Arguments[i - 1], words[i]);
                    }
                }
                else
                {
                    string message = $"Incorrect argument count {words.Length - 1} for command  '{command.Name}'";
                    throw new InvalidArgumentCountException(message);
                }
            }
            else
            {
                string message = $"Command '{words[0]}' is not  recognised";
                throw new InvalidCommandException(message);
            }
        }

        /// <summary>
        /// Check the argument at the specified position is valid according to the given definition
        /// </summary>
        /// <param name="position"></param>
        /// <param name="definition"></param>
        /// <param name="value"></param>
        private void ValidateArgument(int position, ArgumentDefinition definition, string value)
        {
            if (definition.AllowedValues.Count > 0)
            {
                ValidateAllowedValue(position, definition.Name, definition.AllowedValues, value);
            }
            else if (definition.ArgumentType == ArgumentType.Number)
            {
                ValidateNumericArgument(position, definition.Name, definition.Minimum, definition.Maximum, value);
            }
        }

        /// <summary>
        /// Check that a decimal value is in a range
        /// </summary>
        /// <param name="position"></param>
        /// <param name="name"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="value"></param>
        private void ValidateNumericArgument(int position, string name, decimal? minimum, decimal? maximum, string value)
        {
            decimal checkValue;

            try
            {
                checkValue = decimal.Parse(value);
            }
            catch
            {
                string message = $"Value '{value}' is invalid for {name} at position {position}";
                throw new InvalidArgumentException(message);
            }

            if (((minimum != null) && (checkValue < minimum)) || ((maximum != null) && (checkValue > maximum)))
            {
                string message = $"Value '{value}' out of range for {name} at position {position}";
                throw new ValueOutOfRangeException(message);
            }
        }

        /// <summary>
        /// Check a value is in a set of allowed values
        /// </summary>
        /// <param name="position"></param>
        /// <param name="name"></param>
        /// <param name="allowed"></param>
        /// <param name="value"></param>
        private void ValidateAllowedValue(int position, string name, List<string> allowed, string value)
        {
            if (!allowed.Contains(value))
            {
                string message = $"Value '{value}' is not valid for {name} at position {position}";
                throw new InvalidArgumentException(message);
            }
        }
    }
}
