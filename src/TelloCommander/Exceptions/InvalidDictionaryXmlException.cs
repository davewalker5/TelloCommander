using System;

namespace TelloCommander.Exceptions
{
    [Serializable]
    public class InvalidDictionaryXmlException : Exception
    {
        public InvalidDictionaryXmlException()
        {
        }

        public InvalidDictionaryXmlException(string message) : base(message)
        {
        }

        public InvalidDictionaryXmlException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}