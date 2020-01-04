using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TelloCommander.CommandDictionaries;
using TelloCommander.Connections;
using TelloCommander.Interfaces;

namespace TelloCommander.Simulator
{
    public class DroneSimulator
    {
        private const int TimeoutMilliseconds = 5000;

        private MockDrone _drone;

        public DroneSimulator(CommandDictionary dictionary)
        {
            _drone = new MockDrone(dictionary);
        }

        /// <summary>
        /// Return the version number of this simulator
        /// </summary>
        public static string Version { get { return typeof(DroneSimulator).Assembly.GetName().Version.ToString(); } }

        /// <summary>
        /// Start the simulator listener
        /// </summary>
        /// <param name="port"></param>
        public void Listen(int port)
        {
            Console.WriteLine($"Tello Simulator {Version}");
            Console.WriteLine($"Using API/dictionary version {_drone.Dictionary.Version}");

            bool connectedForSending = false;
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, port);
            UdpClient client = new UdpClient(port);
            client.Client.SendTimeout = TimeoutMilliseconds;

            Console.WriteLine($"Listening on port {port}");

            do
            {
                byte[] received = client.Receive(ref endpoint);
                string command = Encoding.UTF8.GetString(received);
                Console.WriteLine($"Received {command}");

                int originalResponseDelay = _drone.ResponseDelay;
                string response = _drone.ConstructCommandResponse(command);

                // If the response delay is set, wait before responding *unless* the
                // command just processed is the one that sets that delay
                if ((_drone.ResponseDelay == originalResponseDelay) && (_drone.ResponseDelay > 0))
                {
                    Console.WriteLine($"Delaying response by {_drone.ResponseDelay} seconds");
                    Thread.Sleep(1000 * _drone.ResponseDelay);
                }

                Console.WriteLine($"Response {response}");

                byte[] datagram = Encoding.UTF8.GetBytes(response);

                if (!connectedForSending)
                {
                    client.Connect(endpoint);
                    connectedForSending = true;
                }

                client.Send(datagram, datagram.Length);
            }
            while (!_drone.Stop);

            client.Dispose();
        }
    }
}
