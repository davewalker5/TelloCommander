using System;

namespace TelloCommander.Exceptions
{
    [Serializable]
    public class InvalidValueRangeException : Exception
    {
        public InvalidValueRangeException()
        {
        }

        public InvalidValueRangeException(string message) : base(message)
        {
        }

        public InvalidValueRangeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}