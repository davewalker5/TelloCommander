using System;

namespace TelloCommander.Exceptions
{
    [Serializable]
    public class ValueOutOfRangeException : Exception
    {
        public ValueOutOfRangeException()
        {
        }

        public ValueOutOfRangeException(string message) : base(message)
        {
        }

        public ValueOutOfRangeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
