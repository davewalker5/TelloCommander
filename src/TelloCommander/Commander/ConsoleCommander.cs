using System;
using System.Diagnostics.CodeAnalysis;
using TelloCommander.CommandDictionaries;
using TelloCommander.Interfaces;

namespace TelloCommander.Commander
{
    [ExcludeFromCodeCoverage]
    [Obsolete("TelloCommander.Commander.ConsoleCommander has moved to the TelloCommander.CommandLine library", true)]
    public class ConsoleCommander : DroneCommander
    {
        public ConsoleCommander(ITelloConnection connection, CommandDictionary dictionary) : base(connection, dictionary)
        {

        }

        /// <summary>
        /// Connect to the drone then repeatedly prompt for and send commands
        /// </summary>
        /// <param name="enableStatusMonitor"></param>
        public void Run(bool enableStatusMonitor = true)
        {
        }
    }
}
