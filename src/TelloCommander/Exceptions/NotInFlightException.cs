using System;
using System.Diagnostics.CodeAnalysis;

namespace TelloCommander.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class NotInFlightException : Exception
    {
        public NotInFlightException()
        {
        }

        public NotInFlightException(string message) : base(message)
        {
        }

        public NotInFlightException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
