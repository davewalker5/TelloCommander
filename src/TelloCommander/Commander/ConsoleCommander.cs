using System;
using System.Diagnostics.CodeAnalysis;
using TelloCommander.CommandDictionaries;
using TelloCommander.Interfaces;
using TelloCommander.Status;

namespace TelloCommander.Commander
{
    [ExcludeFromCodeCoverage]
    public class ConsoleCommander : DroneCommander
    {
        private readonly DroneStatusMonitor _monitor = new DroneStatusMonitor();

        public ConsoleCommander(ITelloConnection connection, CommandDictionary dictionary) : base(connection, dictionary)
        {

        }

        /// <summary>
        /// Connect to the drone then repeatedly prompt for and send commands
        /// </summary>
        /// <param name="enableStatusMonitor"></param>
        public void Run(bool enableStatusMonitor = true)
        {
            try
            {
                Connect();

                if (enableStatusMonitor)
                {
                    _monitor.Listen(DroneStatusMonitor.DefaultTelloStatusPort);
                }

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
                            if (command.Trim() == "?")
                            {
                                ReportDroneStatus();
                                LastResponse = "ok";
                            }
                            else
                            {
                                RunCommand(command);
                            }
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

                _monitor.Stop();
                Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Connecting: {ex.Message}");
            }
        }

        /// <summary>
        /// Report the current status of the drone
        /// </summary>
        private void ReportDroneStatus()
        {
            Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Status : {_monitor.Status}");
            if (!string.IsNullOrEmpty(_monitor.Error))
            {
                Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Error  : {_monitor.Error}");
            }
            else if (!string.IsNullOrEmpty(_monitor.Status))
            {
                // If there's no error and the status has a value, it's been successfully read and
                // the public properties on the monitor will be populated. The sequence number is
                // incremented every time the status is read from the drone. If it stops increasing
                // for any length of time, then either the monitor has stopped or the drone has
                // stopped  broadcasting its status
                Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Sequence     : {_monitor.Sequence}");
                Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Attitude     : {_monitor.Attitude.ToString()}");
                Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Speed        : {_monitor.Speed.ToString()}");
                Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Temperature  : {_monitor.Temperature.ToString()}");
                Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} TOF          : {_monitor.TOF}");
                Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Height       : {_monitor.Height}");
                Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Battery      : {_monitor.Battery}");
                Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Barometer    : {_monitor.Battery}");
                Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Time         : {_monitor.Time}");
                Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Acceleration : {_monitor.Acceleration.ToString()}");
            }
        }
    }
}
