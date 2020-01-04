using System;
using TelloCommander.CommandDictionaries;
using TelloCommander.Interfaces;

namespace TelloCommander.Commander
{
    public class ConsoleCommander : DroneCommander
    {
        public ConsoleCommander(ITelloConnection connection, CommandDictionary dictionary) : base(connection, dictionary)
        {

        }

        /// <summary>
        /// Connect to the drone then repeatedly prompt for and send commands
        /// </summary>
        public void Run()
        {
            try
            {
                Connect();

                Console.WriteLine();
                Console.WriteLine("You are connected to the Tello in API mode");

                bool haveCommand;
                do
                {
                    Console.WriteLine();
                    Console.Write("Enter command or hit ENTER to quit : ");
                    string command = Console.ReadLine();

                    haveCommand = !string.IsNullOrEmpty(command);
                    if (haveCommand)
                    {
                        Console.WriteLine($"Command : {command}");

                        try
                        {
                            RunCommand(command);
                        }
#pragma warning disable RECS0022
                        catch
#pragma warning restore RECS0022
                        {
                            // The error is logged to the history and set as the last response,
                            // which is output below, so no action is required here
                        }

                        Console.WriteLine($"Response: {LastResponse}");
                    }
                }
                while (haveCommand);

                Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Connecting: {ex.Message}");
            }
        }
    }
}
