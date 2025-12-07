using System.Collections.Generic;
using System.Xml.Serialization;

namespace TelloCommander.CommandDictionaries
{
    public sealed class CommandDefinition
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public ConnectionType ConnectionType { get; set; }

        [XmlElement]
        public ResponseType ResponseType { get; set; }

        [XmlElement]
        public bool IsCustomCommand { get; set; }

        [XmlElement]
        public string MockResponse { get; set; }

        public List<ArgumentDefinition> Arguments { get; set; }

        public CommandDefinition()
        {
            Arguments = new List<ArgumentDefinition>();
        }
    }
}
