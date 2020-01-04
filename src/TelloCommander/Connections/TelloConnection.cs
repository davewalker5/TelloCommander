using System.Net;
using System.Net.Sockets;
using System.Text;
using TelloCommander.CommandDictionaries;
using TelloCommander.Interfaces;

namespace TelloCommander.Connections
{
    public class TelloConnection : ITelloConnection
    {
        public const string DefaultTelloAddress = "192.168.10.1";
        public const int DefaultTelloPort = 8889;

        private const ConnectionType DefaultConnectionType = ConnectionType.Drone;

        private UdpClient _client;
        private IPEndPoint _localEndPoint;
        private IPEndPoint _remoteEndPoint;
        private readonly string _address;
        private readonly int _port;

        public ConnectionType ConnectionType { get; private set; }

        public int ReceiveTimeout
        {
            get { return _client.Client.ReceiveTimeout / 1000; }
            set { _client.Client.ReceiveTimeout = value * 1000; }
        }

        public int SendTimeout
        {
            get { return _client.Client.SendTimeout / 1000; }
            set { _client.Client.SendTimeout = value * 1000; }
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
            _localEndPoint = new IPEndPoint(IPAddress.Parse(_address), _port);
            _remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            _client = new UdpClient();
            _client.Connect(_localEndPoint);
        }

        /// <summary>
        /// Disconnect from the drone
        /// </summary>
        public void Close()
        {
            _client.Dispose();
        }

        /// <summary>
        /// Send a command to the drone and read the response
        /// </summary>
        /// <param name="command"></param>
        public string SendCommand(string command)
        {
            byte[] datagram = Encoding.UTF8.GetBytes(command);
            _client.Send(datagram, datagram.Length);

            byte[] received = _client.Receive(ref _remoteEndPoint);
            string response = Encoding.UTF8.GetString(received);

            return response;
        }
    }
}
