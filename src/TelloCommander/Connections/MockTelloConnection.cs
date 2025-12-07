using System.Runtime.CompilerServices;
using TelloCommander.CommandDictionaries;
using TelloCommander.Interfaces;
using TelloCommander.Simulator;

[assembly: InternalsVisibleTo("TelloCommander.Tests")]
namespace TelloCommander.Connections
{
    public class MockTelloConnection : ITelloConnection
    {
        private MockDrone _drone;

        public ConnectionType ConnectionType { get { return ConnectionType.Mock; } }
        public int ReceiveTimeout { get; set; }
        public int SendTimeout { get; set; }

        public MockTelloConnection(CommandDictionary dictionary)
        {
            _drone = new MockDrone(dictionary);
        }

        /// <summary>
        /// Get the current height. This is made public to make the mock connection testable
        /// </summary>
        public int Height { get { return _drone.Height; } }

        /// <summary>
        /// Force a failure response for the *next* command only. This is here to enable unit testing
        /// of the "land on failure" facility
        /// </summary>
        public bool ForceFail { get; set; }

        /// <summary>
        /// Connect to the drone
        /// </summary>
        public void Connect()
        {
        }

        /// <summary>
        /// Disconnect from the drone
        /// </summary>
        public void Close()
        {
        }

        /// <summary>
        /// Send a command to the drone and read the response
        /// </summary>
        /// <param name="command"></param>
        public string SendCommand(string command)
        {
            string response;

            if (!ForceFail)
            {
                response = _drone.ConstructCommandResponse(command);
            }
            else
            {
                response = "Error : Command force-failed";
                ForceFail = false;
            }

            return response;
        }
    }
}
