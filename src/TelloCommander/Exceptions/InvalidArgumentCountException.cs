using System;

namespace TelloCommander.Exceptions
{
    [Serializable]
    public class InvalidArgumentCountException : Exception
    {
        public InvalidArgumentCountException()
        {
        }

        public InvalidArgumentCountException(string message) : base(message)
        {
        }

        public InvalidArgumentCountException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
