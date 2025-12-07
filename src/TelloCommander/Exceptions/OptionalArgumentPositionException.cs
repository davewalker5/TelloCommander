using System;
using System.Diagnostics.CodeAnalysis;

namespace TelloCommander.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class OptionalArgumentPositionException : Exception
    {
        public OptionalArgumentPositionException()
        {
        }

        public OptionalArgumentPositionException(string message) : base(message)
        {
        }

        public OptionalArgumentPositionException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}