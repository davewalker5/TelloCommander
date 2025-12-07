using System;
using System.Diagnostics.CodeAnalysis;

namespace TelloCommander.Commander
{
    [ExcludeFromCodeCoverage]
    [Obsolete("TelloCommander.Commander.ConsoleCommanderWrapper has moved to the TelloCommander.CommandLine library", true)]
    public class ConsoleCommanderWrapper
    {
        /// <summary>
        /// Prompt for the data necessary to create the console commander,
        /// create it and run it
        /// </summary>
        public void Run()
        {
        }
    }
}
