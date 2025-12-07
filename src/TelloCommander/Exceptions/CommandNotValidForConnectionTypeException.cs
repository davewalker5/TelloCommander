using System;
using System.Diagnostics.CodeAnalysis;

namespace TelloCommander.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class CommandNotValidForConnectionTypeException : Exception
    {
        public CommandNotValidForConnectionTypeException()
        {
        }

        public CommandNotValidForConnectionTypeException(string message) : base(message)
        {
        }

        public CommandNotValidForConnectionTypeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

