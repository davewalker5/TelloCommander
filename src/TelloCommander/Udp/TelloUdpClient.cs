using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

namespace TelloCommander.Udp
{
    [ExcludeFromCodeCoverage]
    public class TelloUdpClient : TelloUdpConnection
    {
        public void Connect(IPAddress remoteAddress, int port)
        {
            _sendEndpoint = new IPEndPoint(remoteAddress, port);
            _receiveEndpoint = new IPEndPoint(IPAddress.Any, 0);
            _client = new UdpClient();
            _client.Connect(_sendEndpoint);
        }
    }
}
