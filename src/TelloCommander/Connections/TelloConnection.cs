using System.Diagnostics.CodeAnalysis;
using System.Net;
using TelloCommander.CommandDictionaries;
using TelloCommander.Interfaces;
using TelloCommander.Udp;

namespace TelloCommander.Connections
{
    [ExcludeFromCodeCoverage]
    public class TelloConnection : ITelloConnection
    {
        public const string DefaultTelloAddress = "192.168.10.1";
        public const int DefaultTelloPort = 8889;

        private const ConnectionType DefaultConnectionType = ConnectionType.Drone;

        private readonly string _address;
        private readonly int _port;
        private readonly TelloUdpClient _client = new TelloUdpClient();

        public ConnectionType ConnectionType { get; private set; }

        public int ReceiveTimeout
        {
            get { return _client.ReceiveTimeout; }
            set { _client.ReceiveTimeout = value; }
        }

        public int SendTimeout
        {
            get { return _client.SendTimeout; }
            set { _client.SendTimeout = value; }
        }

        public TelloConnection()
        {
            _address = DefaultTelloAddress;
            _port = DefaultTelloPort;
            ConnectionType = DefaultConnectionType;
        }

        public TelloConnection(string address, int port, ConnectionType connectionType)
        {
            _address = address;
            _port = port;
            ConnectionType = connectionType;
        }

        /// <summary>
        /// Connect to the drone
        /// </summary>
        public void Connect()
        {
            _client.Connect(IPAddress.Parse(_address), _port);
        }

        /// <summary>
        /// Disconnect from the drone
        /// </summary>
        public void Close()
        {
            _client.Close();
        }

        /// <summary>
        /// Send a command to the drone and read the response
        /// </summary>
        /// <param name="command"></param>
        public string SendCommand(string command)
        {
            _client.Send(command);
            string response = _client.Read();
            return response;
        }
    }
}
