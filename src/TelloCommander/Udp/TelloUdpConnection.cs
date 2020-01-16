using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TelloCommander.Udp
{
    [ExcludeFromCodeCoverage]
    public abstract class TelloUdpConnection : IDisposable
    {
        protected UdpClient _client;
        protected IPEndPoint _sendEndpoint;
        protected IPEndPoint _receiveEndpoint;

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

        public int Available { get { return _client.Available; } }

        public void Close()
        {
            _client.Dispose();
        }

        public virtual int Send(string data)
        {
            byte[] datagram = Encoding.UTF8.GetBytes(data);
            int sent = _client.Send(datagram, datagram.Length);
            return sent;
        }

        public byte[] ReadBytes()
        {
            byte[] received = _client.Receive(ref _receiveEndpoint);
            return received;
        }

        public string Read()
        {
            byte[] received = ReadBytes();
            string data = Encoding.UTF8.GetString(received);
            return data;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_client != null)
                {
                    try
                    {
                        _client.Close();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        _client.Dispose();
                    }
                }
            }
        }
    }
}
