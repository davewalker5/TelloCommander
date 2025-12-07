using System.Collections.Generic;
using System.Xml.Serialization;

namespace TelloCommander.CommandDictionaries
{
    public sealed class ArgumentDefinition
    {
        [XmlElement]
        public string Name { get; set; }

        [XmlElement]
        public ArgumentType ArgumentType { get; set; }

        [XmlElement]
        public bool Required { get; set; }

        [XmlElement(IsNullable = true)]
        public decimal? Minimum { get; set; }

        [XmlElement(IsNullable = true)]
        public decimal? Maximum { get; set; }

        public List<string> AllowedValues { get; set; }

        public ArgumentDefinition()
        {
            AllowedValues = new List<string>();
        }
    }
}
