using System;
using System.Diagnostics.CodeAnalysis;

namespace TelloCommander.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class TooLowToMoveDownException : Exception
    {
        public TooLowToMoveDownException()
        {
        }

        public TooLowToMoveDownException(string message) : base(message)
        {
        }

        public TooLowToMoveDownException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

