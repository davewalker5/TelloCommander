using System;
using TelloCommander.CommandDictionaries;
using TelloCommander.Connections;
using TelloCommander.Simulator;
using TelloCommander.Status;

namespace TelloSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                CommandDictionary dictionary = CommandDictionary.ReadStandardDictionary(args[0]);
                new DroneSimulator(dictionary).Listen(TelloConnection.DefaultTelloPort, DroneStatusMonitor.DefaultTelloStatusPort);
            }
            else
            {
                string location = typeof(Program).Assembly.Location;
                string executable = System.IO.Path.GetFileNameWithoutExtension(location);
                Console.WriteLine($"Usage : {executable} <dictionary_version>");
            }
        }
    }
}
