using System;
using System.Diagnostics.CodeAnalysis;

namespace TelloCommander.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
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
