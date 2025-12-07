using System;
using System.Diagnostics.CodeAnalysis;

namespace TelloCommander.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
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
