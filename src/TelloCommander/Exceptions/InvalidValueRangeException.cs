using System;
using System.Diagnostics.CodeAnalysis;

namespace TelloCommander.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
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