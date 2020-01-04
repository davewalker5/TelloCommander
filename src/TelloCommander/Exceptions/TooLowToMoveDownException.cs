using System;

namespace TelloCommander.Exceptions
{
    [Serializable]
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

