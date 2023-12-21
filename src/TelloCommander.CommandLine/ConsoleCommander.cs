using System;
using System.Diagnostics.CodeAnalysis;
using TelloCommander.CommandDictionaries;
using TelloCommander.Commander;
using TelloCommander.Data.Collector;
using TelloCommander.Data.Sqlite;
using TelloCommander.Interfaces;
using TelloCommander.Status;

namespace TelloCommander.CommandLine
{
    [ExcludeFromCodeCoverage]
    public class ConsoleCommander : DroneCommander
    {
        private readonly DroneStatusMonitor _monitor = new();

        public ConsoleCommander(ITelloConnection connection, CommandDictionary dictionary) : base(connection, dictionary)
        {
        }

        /// <summary>
        /// Connect to the drone then repeatedly prompt for and send commands
        /// </summary>
        /// <param name="enableStatusMonitor"></param>
        public void Run(bool enableStatusMonitor = true)
        {
            char[] separators = { ' ' };
            TelemetryCollector collector = null;
            TelloCommanderDbContext context = null;

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
                            string[] words = command.Trim().Split(separators, StringSplitOptions.RemoveEmptyEntries);
                            switch (words[0])
                            {
                                case "?":
                                    ReportDroneStatus();
                                    LastResponse = "ok";
                                    break;
                                case "startcapture":
                                    _ = int.TryParse(words[2], out int intervalMilliseconds);
                                    _monitor.StartCapture(words[1], intervalMilliseconds);
                                    LastResponse = "ok";
                                    break;
                                case "stopcapture":
                                    _monitor.StopCapture();
                                    LastResponse = "ok";
                                    break;
                                case "startdbcapture":
                                    _ = int.TryParse(words[3], out int collectionInterval);
                                    string[] filters = (words.Length > 4) ? words[4].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : null;
                                    context = new TelloCommanderDbContextFactory().CreateDbContext(null);
                                    collector = new TelemetryCollector(context, _monitor);
                                    collector.Start(words[1], words[2], collectionInterval, filters);
                                    LastResponse = "ok";
                                    break;
                                case "stopdbcapture":
                                    if (collector != null)
                                    {
                                        collector.Stop();
                                        context.Dispose();
                                        collector = null;
                                    }
                                    LastResponse = "ok";
                                    break;
                                default:
                                    RunCommand(command);
                                    break;
                            }

                            Console.WriteLine($"Response: {LastResponse}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error : {ex.Message}");
                        }
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
            Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff} Status : {_monitor.Status}");
            if (!string.IsNullOrEmpty(_monitor.Error))
            {
                Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff} Error  : {_monitor.Error}");
            }
            else if (!string.IsNullOrEmpty(_monitor.Status))
            {
                // If there's no error and the status has a value, it's been successfully read and
                // the public properties on the monitor will be populated. The sequence number is
                // incremented every time the status is read from the drone. If it stops increasing
                // for any length of time, then either the monitor has stopped or the drone has
                // stopped  broadcasting its status
                Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff} Sequence     : {_monitor.Sequence}");
                Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff} Attitude     : {_monitor.Attitude}");
                Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff} Speed        : {_monitor.Speed}");
                Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff} Temperature  : {_monitor.Temperature}");
                Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff} TOF          : {_monitor.TOF}");
                Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff} Height       : {_monitor.Height}");
                Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff} Battery      : {_monitor.Battery}");
                Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff} Barometer    : {_monitor.Barometer}");
                Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff} Time         : {_monitor.Time}");
                Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff} Acceleration : {_monitor.Acceleration}");
            }
        }
    }
}
