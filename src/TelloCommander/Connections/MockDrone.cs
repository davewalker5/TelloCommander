using System;
using TelloCommander.CommandDictionaries;

namespace TelloCommander.Connections
{
    internal class MockDrone
    {
        public CommandDictionary Dictionary { get; private set; }

        public MockDrone(CommandDictionary dictionary)
        {
            Dictionary = dictionary;
        }

        /// <summary>
        /// Get the current height
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Set to true by the "stop" command, intended to stop the simulator
        /// </summary>
        public bool Stop { get; private set; }

        /// <summary>
        /// Response delay, in seconds, for responses from the simulator. Used in
        /// timeout simulation
        /// </summary>
        public int ResponseDelay { get; private set; }

        /// <summary>
        /// Construct a mock response for the specified  command
        /// </summary>
        /// <param name="command"></param>
        public string ConstructCommandResponse(string command)
        {
            string response;

            string[] words = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            switch (words[0])
            {
                case "takeoff":
                    Height = 6;
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                case "land":
                    Height = 0;
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                case "emergency":
                    Height = 0;
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                case "up":
                    int.TryParse(words[1], out int upAmount);
                    Height = (10 * Height + upAmount) / 10;
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                case "down":
                    int.TryParse(words[1], out int downAmount);
                    Height = (10 * Height - downAmount) / 10;
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                case "curve":
                    int.TryParse(words[5], out int curveFinalHeight);
                    Height = curveFinalHeight / 10;
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                case "go":
                    int.TryParse(words[2], out int goHeight);
                    Height = goHeight / 10;
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                case "height?":
                    response = $"{Height}dm";
                    break;
                case "stopsimulator":
                    Stop = true;
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                case "responsedelay":
                    int.TryParse(words[1], out int delay);
                    ResponseDelay = delay;
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                default:
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
            }

            return response;
        }

        /// <summary>
        /// Return a string representing the current status of the drone
        /// </summary>
        /// <returns></returns>
        public string GetStatus()
        {
            string status = $"pitch:0;roll:0;yaw:0;vgx:0;vgy:0;vgz:0;templ:0;temph:0;tof:0;h:{Height};bat:0;baro:0.00;time:0;agx:0.00;agy:0.00;agz:0.00;";
            return status;
        }
    }
}
