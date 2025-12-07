using System;
using System.Diagnostics.CodeAnalysis;

namespace TelloCommander.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
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