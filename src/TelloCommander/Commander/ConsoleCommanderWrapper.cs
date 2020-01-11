using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using TelloCommander.CommandDictionaries;
using TelloCommander.Connections;
using TelloCommander.Interfaces;

namespace TelloCommander.Commander
{
    [ExcludeFromCodeCoverage]
    public class ConsoleCommanderWrapper
    {
        /// <summary>
        /// Prompt for the data necessary to create the console commander,
        /// create it and run it
        /// </summary>
        public void Run()
        {
            Console.WriteLine($"Tello Commander {DroneCommander.Version}");

            string version = PromptForDictionaryVersion();
            if (!string.IsNullOrEmpty(version))
            {
                ConnectionType connectionType = PromptForConnectionType();
                if (connectionType != ConnectionType.None)
                {
                    CommandDictionary dictionary = CommandDictionary.ReadStandardDictionary(version);
                    switch (connectionType)
                    {
                        case ConnectionType.Mock:
                            ITelloConnection mockConnection = new MockTelloConnection(dictionary);
                            new ConsoleCommander(mockConnection, dictionary).Run(false);
                            break;
                        case ConnectionType.Simulator:
                            ITelloConnection connection = new TelloConnection(IPAddress.Loopback.ToString(), TelloConnection.DefaultTelloPort, connectionType);
                            new ConsoleCommander(connection, dictionary).Run(true);
                            break;
                        case ConnectionType.Drone:
                            new ConsoleCommander(new TelloConnection(), dictionary).Run(true);
                            break;
                        default:
                            break;
                    }

                }
            }
        }

        /// <summary>
        /// Prompt the user for an option from a numbered list
        /// </summary>
        /// <returns></returns>
        private int PromptForOption(string title, string[] options)
        {
            Console.WriteLine();
            Console.WriteLine(title);
            Console.WriteLine();

            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"[{i + 1}] {options[i]}");
            }

            Console.WriteLine();

            int option = -1;
            do
            {
                Console.Write($"Please enter your option between 1 and {options.Length} or enter 0 to quit: ");
                string input = Console.ReadLine();
                int.TryParse(input, out option);
            }
            while ((option < 0) || (option > options.Length));

            return option;
        }

        /// <summary>
        /// Prompt for and return the API (dictionary) version to use
        /// </summary>
        /// <returns></returns>
        private string PromptForDictionaryVersion()
        {
            string[] versions = CommandDictionary.GetAvailableDictionaryVersions();
            string version = null;

            if (versions.Length == 1)
            {
                version = versions[0];
                Console.WriteLine($"Using dictionary version {version}");
            }
            else
            {
                int selection = PromptForOption("Dictionary Version:", versions);
                if (selection > 0)
                {
                    version = versions[selection - 1];
                }
            }

            return version;
        }

        /// <summary>
        /// Prompt for the type of connection  to use
        /// </summary>
        /// <returns></returns>
        private ConnectionType PromptForConnectionType()
        {
            int connectionType = PromptForOption("Connection:", new string[]
            {
                    "Mock connection",
                    "Simulator connection",
                    "Real connection"
            });

            return (ConnectionType)connectionType;
        }
    }
}
