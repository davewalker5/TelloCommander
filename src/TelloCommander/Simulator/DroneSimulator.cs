using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TelloCommander.CommandDictionaries;
using TelloCommander.Connections;
using TelloCommander.Udp;

namespace TelloCommander.Simulator
{
    [ExcludeFromCodeCoverage]
    public class DroneSimulator
    {
        private const int TimeoutSeconds = 5;
        private const int StatusSendIntervalMilliseconds = 1000;

        private CancellationTokenSource _source;
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
        /// Run the simulator
        /// </summary>
        /// <param name="controlPort"></param>
        /// <param name="statusPort"></param>
        public void Listen(int controlPort, int statusPort)
        {
            Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Tello Simulator {Version}");
            Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Using API/dictionary version {_drone.Dictionary.Version}");

            StartStatusSender(statusPort);
            StartCommandListener(controlPort);
        }

        /// <summary>
        /// Start the command listener
        /// </summary>
        /// <param name="port"></param>
        private void StartCommandListener(int port)
        {
            TelloUdpServer server = new TelloUdpServer();
            server.Connect(port);
            server.SendTimeout = TimeoutSeconds;

            Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Listening for commands on port {port}");

            do
            {
                string command = server.Read();
                Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Received {command}");

                int originalResponseDelay = _drone.ResponseDelay;
                string response = _drone.ConstructCommandResponse(command);

                // If the response delay is set, wait before responding *unless* the
                // command just processed is the one that sets that delay
                if ((_drone.ResponseDelay == originalResponseDelay) && (_drone.ResponseDelay > 0))
                {
                    Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Delaying response by {_drone.ResponseDelay} seconds");
                    Thread.Sleep(1000 * _drone.ResponseDelay);
                }

                Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Response {response}");
                server.Send(response);
            }
            while (!_drone.Stop);

            _source.Cancel();
            _source.Dispose();
            server.Close();
        }

        /// <summary>
        /// Start the drone status broadcaster
        /// </summary>
        /// <param name="port"></param>
        private void StartStatusSender(int port)
        {
            _source = new CancellationTokenSource();
            CancellationToken token = _source.Token;

            var task = Task.Run(() =>
            {
                token.ThrowIfCancellationRequested();

                TelloUdpSender client = new TelloUdpSender();
                client.Connect(port);

                Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Reporting status on port {port}");

                while (true)
                {
                    try
                    {
                        int sent = client.Send(_drone.GetStatus());
                        Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Sent {sent} status bytes");
                        Thread.Sleep(StatusSendIntervalMilliseconds);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Status Sender : {ex.Message}");
                    }

                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                        client.Close();
                    }
                }

            }, token);
        }
    }
}
