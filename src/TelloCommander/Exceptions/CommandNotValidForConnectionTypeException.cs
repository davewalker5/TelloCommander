using System;

namespace TelloCommander.Exceptions
{
    [Serializable]
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

