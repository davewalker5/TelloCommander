using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using TelloCommander.CommandDictionaries;
using TelloCommander.Connections;
using TelloCommander.Exceptions;
using TelloCommander.Interfaces;

[assembly: InternalsVisibleTo("TelloCommander.Tests")]
namespace TelloCommander.Commander
{
    public class DroneCommander
    {
        private const string Comment = "#";
        private const string ResponseOK = "ok";
        private const string ErrorResponseText = "error";
        private const int MinimumHeight = 20;

        private const string ApiModeCommand = "command";
        private const string LandCommand = "land";
        private const string DownCommand = "down";
        private const string GetHeightCommand = "height?";
        private const string RunScriptCommand = "runscript";
        private const string HistoryCommand = "history";
        private const string WriteHistoryCommand = "writehistory";
        private const string WaitCommand = "wait";
        private const string ForceFailCommand = "forcefail";
        private const string ReceiveTimeoutCommand = "receivetimeout";
        private const string SendTimeoutCommand = "sendtimeout";

        private const string HeightUnits = "dm";

        private readonly ITelloConnection _connection;
        private readonly CommandValidator _validator;
        private readonly char[] _separators = { ' ', '\t' };
        private readonly List<string> _history = new List<string>();
        private readonly Stack<string> _scriptPaths = new Stack<string>();

        public DroneCommander(ITelloConnection connection, CommandDictionary dictionary)
        {
            LandOnError = true;
            _connection = connection;
            _validator = new CommandValidator(dictionary);
        }

        /// <summary>
        /// Return the version number of this API
        /// </summary>
        public static string Version { get { return typeof(DroneCommander).Assembly.GetName().Version.ToString(); } }

        /// <summary>
        /// Return the last response from the drone
        /// </summary>
        public string LastResponse { get; set; }

        /// <summary>
        /// Return the command/response history
        /// </summary>
        public IList<string> History { get { return _history; } }

        /// <summary>
        /// When set to true, a "land" command will be sent to the drone if an
        /// error response is received
        /// </summary>
        public bool LandOnError { get; set; }

        /// <summary>
        /// Connect to the drone
        /// </summary>
        public void Connect()
        {
            // Connect to the drone and put it in API mode
            _connection.Connect();
            _connection.SendCommand(ApiModeCommand);

            // Log the API version and connection details to the history
            AddHistory($"Tello Commander Version {Version}");
            AddHistory($"Connected to the drone in API mode");
        }

        /// <summary>
        /// Disconnect from the drone
        /// </summary>
        public void Disconnect()
        {
            _connection.Close();
            AddHistory($"Disconnected");
        }

        /// <summary>
        /// Run the specified command, which may be a drone command or a custom
        /// command that is not sent to the drone
        /// </summary>
        /// <param name="command"></param>
        public void RunCommand(string command)
        {
            command = command?.Trim();
            if (!string.IsNullOrEmpty(command))
            {
                try
                {
                    // Confirm the command is valid (this will throw an exception on error)
                    string[] words = command.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
                    _validator.ValidateCommand(_connection.ConnectionType, words);

                    switch (words[0])
                    {
                        case RunScriptCommand:
                            RunCommandsFromFile(words[1]);
                            break;
                        case HistoryCommand:
                            WriteHistory(Console.Out);
                            break;
                        case WriteHistoryCommand:
                            WriteHistory(words[1]);
                            break;
                        case WaitCommand:
                            AddHistory(command);
                            Thread.Sleep(1000 * int.Parse(words[1]));
                            AddHistory(ResponseOK);
                            LastResponse = ResponseOK;
                            break;
                        case ForceFailCommand:
                            MockTelloConnection mock = _connection as MockTelloConnection;
                            if (mock != null)
                            {
                                mock.ForceFail = true;
                                LastResponse = ResponseOK;
                            }
                            break;
                        case ReceiveTimeoutCommand:
                            _connection.ReceiveTimeout = int.Parse(words[1]);
                            LastResponse = ResponseOK;
                            break;
                        case SendTimeoutCommand:
                            _connection.SendTimeout = int.Parse(words[1]);
                            LastResponse = ResponseOK;
                            break;
                        default:
                            SendCommand(command);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LastResponse = ex.Message;
                    AddHistory(ex.Message);
                    throw;
                }
            }
            else
            {
                string message = $"Invalid command '{command ?? ""}'";
                LastResponse = message;
                AddHistory(message);
                throw new InvalidCommandException(message);
            }
        }

        /// <summary>
        /// Run the commands in the specified file
        /// </summary>
        /// <param name="file"></param>
        public void RunScript(string file)
        {
            RunCommand($"{RunScriptCommand} {file}");
        }

        /// <summary>
        /// Write the command/response history to the specified stream
        /// </summary>
        /// <param name="writer"></param>
        public void WriteHistory(TextWriter writer)
        {
            for (int i = 0; i < History.Count; i++)
            {
                writer.WriteLine($"{i + 1:000000} {History[i]}");
            }
        }

        /// <summary>
        /// Write the command/response history to the specified file
        /// </summary>
        /// <param name="file"></param>
        public void WriteHistory(string file)
        {
            using (StreamWriter writer = new StreamWriter(file))
            {
                WriteHistory(writer);
            }
        }

        /// <summary>
        /// Add a timestamped entry to the history
        /// </summary>
        /// <param name="entry"></param>
        private void AddHistory(string entry)
        {
            _history.Add($"{DateTime.Now.ToString("yyyy-MMM-dd hh:mm:ss")} {entry}");
        }

        /// <summary>
        /// Run the commands held in the specified text file
        /// </summary>
        /// <param name="file"></param>
        private void RunCommandsFromFile(string file)
        {
            // Get the full path to the file, using the script path stack to achieve
            // this for relative paths, and push the containing folder for the file
            // onto the stack. This is then used to resolve the path for scripts
            // called from within the current script
            string fullPath = GetFullScriptPath(file);
            string folder = Path.GetDirectoryName(fullPath);
            _scriptPaths.Push(folder);

            AddHistory($"Script Folder : {folder}");
            AddHistory($"Start Script  : {file}");

            using (StreamReader reader = new StreamReader(fullPath))
            {
                while (!reader.EndOfStream)
                {
                    string command = reader.ReadLine().Trim();

                    // See if the command contains the comment character and, if so, truncate
                    // from that character on
                    int startComment = command.IndexOf(Comment, StringComparison.CurrentCulture);
                    if (startComment > -1)
                    {
                        command = command.Substring(0, Math.Max(startComment - 1, 0)).Trim();
                    }

                    if (!string.IsNullOrEmpty(command))
                    {
                        RunCommand(command);
                    }
                }
            }

            // Pop the latest script path off the stack
            _scriptPaths.Pop();

            AddHistory($"End Script  : {file}");
            LastResponse = ResponseOK;
        }

        /// <summary>
        /// Return the full path to a specified script file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string GetFullScriptPath(string file)
        {
            string fullPath = file;

            if (!Path.IsPathRooted(file))
            {
                if (_scriptPaths.Count == 0)
                {
                    // Get the folder containing the current assembly
                    string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder builder = new UriBuilder(codeBase);
                    string folder = Path.GetDirectoryName(Uri.UnescapeDataString(builder.Path));

                    // Combine with the specified file to give the full path
                    fullPath = Path.Combine(folder, file);
                }
                else
                {
                    // Combine the last entry on the stack with the specified file
                    fullPath = Path.Combine(_scriptPaths.Peek(), file);
                }
            }

            return fullPath;
        }

        /// <summary>
        /// Send the specified command to the drone and display the response
        /// </summary>
        /// <param name="command"></param>
        private void SendCommand(string command)
        {
            AddHistory(command);

            // If this is a "move down" command, check the drone is high enough to
            // do the move. Otherwise, it doesn't move and becomes unresponsive
            string[] words = command.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
            if ((words[0] == DownCommand) && (_validator.Dictionary.Commands.FirstOrDefault(c => c.Name == GetHeightCommand) != null))
            {
                // Move down validation will throw an exception if the move is not possible
                ValidateMoveDown(words);
            }

            // Send the command to the drone, await the response and add it to the
            // history
            LastResponse = _connection.SendCommand(command);
            AddHistory(LastResponse);

            // The response should not contain the word "error". If it does, and the
            // LandOnError property is true, attempt to land the drone
            if (LastResponse.ToLower().Contains(ErrorResponseText) && LandOnError)
            {
                AddHistory(LandCommand);
                string response = _connection.SendCommand(LandCommand);
                AddHistory(response);
            }
        }

        /// <summary>
        /// Check it's possible to execute a "down" command at the current height
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private void ValidateMoveDown(string[] command)
        {
            bool proceed = true;

            // Parse the second word of the command to an integer decrement in height
            int.TryParse(command[1], out int amountToMoveDown);

            // Get the current drone height
            int height = GetHeight();

            // Check it's at or above the minimum plus the amount we want to move down by
            int requiredMinimumHeight = MinimumHeight + amountToMoveDown;
            proceed = height >= requiredMinimumHeight;

            if (!proceed)
            {
                string message = $"Height is {height} cm. Too low  to move down by {command[1]}";
                throw new TooLowToMoveDownException(message);
            }
        }

        /// <summary>
        /// Return the drone height in cm
        /// </summary>
        /// <returns></returns>
        private int GetHeight()
        {
            string response = _connection.SendCommand(GetHeightCommand).Replace(HeightUnits, "");
            int.TryParse(response, out int height);
            return height * 10;
        }
    }
}
