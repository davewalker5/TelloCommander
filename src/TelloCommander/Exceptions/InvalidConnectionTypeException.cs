using System;
using System.Diagnostics.CodeAnalysis;

namespace TelloCommander.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class InvalidConnectionTypeException : Exception
    {
        public InvalidConnectionTypeException()
        {
        }

        public InvalidConnectionTypeException(string message) : base(message)
        {
        }

        public InvalidConnectionTypeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

