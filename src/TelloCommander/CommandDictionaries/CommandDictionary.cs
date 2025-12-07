using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using TelloCommander.Exceptions;

namespace TelloCommander.CommandDictionaries
{
    [XmlRoot(ElementName = "CommandDictionary")]
    public sealed class CommandDictionary
    {
        public List<CommandDefinition> Commands { get; private set; }

        [XmlIgnore]
        public string Version { get; private set; }

        public CommandDictionary()
        {
            Commands = new List<CommandDefinition>();
        }

        /// <summary>
        /// Return the mock response for a specified command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string GetMockResponse(string command)
        {
            return Commands.First(c => c.Name.Equals(command)).MockResponse;
        }

        /// <summary>
        /// Write the dictionary to the specified file
        /// </summary>
        /// <param name="file"></param>
        public void Write(string file)
        {
            using (StreamWriter writer = new StreamWriter(file))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(writer))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CommandDictionary));
                    serializer.Serialize(writer, this);
                }
            }
        }

        /// <summary>
        /// Read a dictionary from the specified file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static CommandDictionary Read(string file)
        {
            CommandDictionary dictionary;

            ValidateXml(file);

            using (StreamReader reader = new StreamReader(file))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CommandDictionary));
                dictionary = (CommandDictionary)serializer.Deserialize(reader);
            }

            ValidateArgumentDefinitions(dictionary);
            dictionary.Version = GetVersionFromFilename(file);
            return dictionary;
        }

        /// <summary>
        /// Read a standard dictionary held in the Content folder of the API
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static CommandDictionary ReadStandardDictionary(string version)
        {
            string file = GetContentFilePath($"CommandDictionary-{version}.xml");
            return Read(file);
        }

        /// <summary>
        /// Return an array of the available dictionary versions
        /// </summary>
        /// <returns></returns>
        public static string[] GetAvailableDictionaryVersions()
        {
            // List files matching the dictionary pattern in the Content folder
            string contentFolder = Path.Combine(GetAssemblyLocation(), "Content");
            string[] matches = Directory.GetFiles(contentFolder, "CommandDictionary-*.xml");
            string[] versions = matches.Select(m => GetVersionFromFilename(m)).ToArray();
            return versions;
        }

        /// <summary>
        /// Validate an XML file against the command dictionary XSD
        /// </summary>
        /// <param name="file"></param>
        private static void ValidateXml(string file)
        {
            // Construct the path to the XSD file
            string xsd = GetContentFilePath("CommandDictionary.xsd");

            // Configure validation settings
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add("", xsd);
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationEventHandler += OnValidationEvent;

            using (XmlReader reader = XmlReader.Create(file, settings))
            {
                while (reader.Read())
                {
                }
            }
        }

        /// <summary>
        /// Respond to an XML validation event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnValidationEvent(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
            {
                throw new InvalidDictionaryXmlException(e.Message);
            }
        }

        /// <summary>
        /// Perform post-load validation checks on the argument definitions
        /// </summary>
        /// <param name="dictionary"></param>
        private static void ValidateArgumentDefinitions(CommandDictionary dictionary)
        {
            foreach (CommandDefinition command in dictionary.Commands.Where(c => c.Arguments.Any()))
            {
                if (command.Arguments.Count > 1)
                {
                    ValidateNonMandatoryArguments(command);
                }

                ValidateMinimumMaximumOrder(command);
            }
        }

        /// <summary>
        /// For a given command, check that all optional arguments appear at the end of the
        /// argument list
        /// </summary>
        /// <param name="command"></param>
        private static void ValidateNonMandatoryArguments(CommandDefinition command)
        {
            ArgumentDefinition firstOptional = command.Arguments.FirstOrDefault(a => !a.Required);
            if (firstOptional != null)
            {
                ArgumentDefinition lastMandatory = command.Arguments.LastOrDefault(a => a.Required);
                if (lastMandatory != null)
                {
                    int firstOptionalIndex = command.Arguments.IndexOf(firstOptional);
                    int lastMandatoryIndex = command.Arguments.IndexOf(lastMandatory);
                    if (lastMandatoryIndex > firstOptionalIndex)
                    {
                        string message = $"Optional arguments must be at the end of the argument list for command {command.Name}";
                        throw new OptionalArgumentPositionException(message);
                    }
                }
            }
        }

        /// <summary>
        /// Check the minimum and maximum occur in the right order
        /// </summary>
        /// <param name="command"></param>
        private static void ValidateMinimumMaximumOrder(CommandDefinition command)
        {
            foreach (ArgumentDefinition argument in command.Arguments)
            {
                if (argument.Minimum > argument.Maximum)
                {
                    string message = $"Invalid argument value range for argument {argument.Name}, command {command.Name}";
                    throw new InvalidValueRangeException(message);
                }
            }
        }

        /// <summary>
        /// Get the version number from the dictionary filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static string GetVersionFromFilename(string filename)
        {
            return Path.GetFileNameWithoutExtension(filename).Replace("CommandDictionary-", "");
        }

        /// <summary>
        /// Get the location of the current assembly
        /// </summary>
        /// <returns></returns>
        private static string GetAssemblyLocation()
        {
            string codeBase = Assembly.GetExecutingAssembly().Location;
            string location = Path.GetDirectoryName(codeBase);
            return location;
        }

        /// <summary>
        /// Get the path to a file held in the Content folder
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static string GetContentFilePath(string file)
        {
            string path = Path.Combine(GetAssemblyLocation(), "Content", file);
            return path;
        }
    }
}
