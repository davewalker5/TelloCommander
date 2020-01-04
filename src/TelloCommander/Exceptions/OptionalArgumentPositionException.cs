using System;

namespace TelloCommander.Exceptions
{
    [Serializable]
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