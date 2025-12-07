using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

namespace TelloCommander.Udp
{
    [ExcludeFromCodeCoverage]
    public class TelloUdpServer : TelloUdpConnection
    {
        private bool _connectedForSending;

        public void Connect(int port)
        {
            _receiveEndpoint = new IPEndPoint(IPAddress.Any, 0);
            _client = new UdpClient(port);
            _connectedForSending = false;
        }

        public override int Send(string data)
        {
            if (!_connectedForSending)
            {
                _client.Connect(_receiveEndpoint);
                _connectedForSending = true;
            }

            return base.Send(data);
        }
    }
}
