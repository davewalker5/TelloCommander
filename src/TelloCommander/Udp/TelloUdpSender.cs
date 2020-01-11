using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

namespace TelloCommander.Udp
{
    [ExcludeFromCodeCoverage]
    public class TelloUdpSender : TelloUdpConnection
    {
        public void Connect(int port)
        {
            _sendEndpoint = new IPEndPoint(IPAddress.Broadcast, port);
            _client = new UdpClient();
            _client.Connect(_sendEndpoint);
        }
    }
}
