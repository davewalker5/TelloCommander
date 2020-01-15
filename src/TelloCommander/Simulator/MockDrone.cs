using System;
using System.Timers;
using TelloCommander.CommandDictionaries;
using TelloCommander.Exceptions;
using TelloCommander.Response;

namespace TelloCommander.Simulator
{
    internal class MockDrone
    {
        private DateTime _startOfFlight;
        private readonly Timer _timer = new Timer();

        public MockDrone(CommandDictionary dictionary)
        {
            Dictionary = dictionary;
            Position = new Position();

            Battery = 100;
            CriticalBatteryLevel = 10;
            SingleChargeFlightTimeSeconds = 300;

            _timer.Interval = 1000;
            _timer.Elapsed += OnBatteryTimer;
        }

        /// <summary>
        /// Event raised when the event arguments are updated
        /// </summary>
        public event EventHandler<BatteryCriticalEventArgs> BatteryCritical;

        /// <summary>
        /// Current command dictionary
        /// </summary>
        public CommandDictionary Dictionary { get; private set; }

        /// <summary>
        /// Return true if the drone
        /// </summary>
        public bool InFlight { get; private set; }

        /// <summary>
        /// Get the current height in dm
        /// </summary>
        public int Height { get { return (int)(Position.Y / 10); } }

        /// <summary>
        /// Get the current position of the drone (X, Y, Z)
        /// </summary>
        public Position Position { get; private set; }

        /// <summary>
        /// Get the current heading of the drone
        /// </summary>
        public int Heading { get; set; }

        /// <summary>
        /// Time of flight
        /// </summary>
        public int TimeOfFlight
        {
            get { return (InFlight) ? (int)(DateTime.Now - _startOfFlight).TotalSeconds : 0; }
        }

        /// <summary>
        /// Return the current battery charge
        /// </summary>
        public int Battery { get; private set; }

        /// <summary>
        /// Set the % battery charge at which the drone lands automatically
        /// </summary>
        public int CriticalBatteryLevel { get; set; }

        /// <summary>
        /// Set the flight time (in seconds) from a single battery charge
        /// </summary>
        public int SingleChargeFlightTimeSeconds { get; set; }

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

            try
            {
                string[] words = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                response = RespondToControlCommand(words);
                if (string.IsNullOrEmpty(response))
                {
                    response = RespondToMovementCommand(words);
                    if (string.IsNullOrEmpty(response))
                    {
                        response = RespondToReadCommand(words);
                        if (string.IsNullOrEmpty(response))
                        {
                            response = Dictionary.GetMockResponse(words[0]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = $"Error : {ex.Message}";
            }

            return response;
        }

        /// <summary>
        /// Return a string representing the current status of the drone
        /// </summary>
        /// <returns></returns>
        public string GetStatus()
        {
            string status = $"pitch:0;roll:0;yaw:0;vgx:0;vgy:0;vgz:0;templ:0;temph:0;tof:0;h:{Height};bat:{Battery};baro:0.00;time:0;agx:0.00;agy:0.00;agz:0.00;";
            return status;
        }

        /// <summary>
        /// Respond to a drone control command
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        private string RespondToControlCommand(string[] words)
        {
            string response = "";

            switch (words[0])
            {
                case "takeoff":
                    InFlight = true;
                    _startOfFlight = DateTime.Now;
                    Position.X = 0;
                    Position.Y = 60;
                    Position.Z = 0;
                    Battery = 100;
                    response = Dictionary.GetMockResponse(words[0]);
                    _timer.Enabled = true;
                    break;
                case "land":
                    AssertIsInFlight();
                    Land();
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                case "emergency":
                    AssertIsInFlight();
                    Position.Y = 0;
                    InFlight = false;
                    response = Dictionary.GetMockResponse(words[0]);
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
                    break;
            }

            return response;
        }

        /// <summary>
        /// Respond to a drone movement command
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        private string RespondToMovementCommand(string[] words)
        {
            string response = "";

            switch (words[0])
            {
                case "up":
                    AssertIsInFlight();
                    int.TryParse(words[1], out int upAmount);
                    Position.Y += upAmount;
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                case "down":
                    AssertIsInFlight();
                    int.TryParse(words[1], out int downAmount);
                    Position.Y -= downAmount;
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                case "forward":
                    AssertIsInFlight();
                    int.TryParse(words[1], out int forwardAmount);
                    MoveForwardOrBackward(forwardAmount, true);
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                case "back":
                    AssertIsInFlight();
                    int.TryParse(words[1], out int backAmount);
                    MoveForwardOrBackward(backAmount, false);
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                case "left":
                    AssertIsInFlight();
                    int.TryParse(words[1], out int leftAmount);
                    Position.X -= leftAmount;
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                case "right":
                    AssertIsInFlight();
                    int.TryParse(words[1], out int rightAmount);
                    Position.X += rightAmount;
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                case "curve":
                    AssertIsInFlight();
                    int.TryParse(words[5], out int curveFinalHeight);
                    Position.Y = curveFinalHeight;
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                case "go":
                    AssertIsInFlight();
                    int.TryParse(words[2], out int goHeight);
                    Position.Y = goHeight;
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                case "cw":
                    AssertIsInFlight();
                    int.TryParse(words[1], out int clockwise);
                    Heading = WrapAngle(Heading + clockwise);
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                case "ccw":
                    AssertIsInFlight();
                    int.TryParse(words[1], out int anticlockwise);
                    Heading = WrapAngle(Heading - anticlockwise);
                    response = Dictionary.GetMockResponse(words[0]);
                    break;
                default:
                    break;
            }

            return response;
        }

        /// <summary>
        /// Respond to a command that queries the drone's status
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        private string RespondToReadCommand(string[] words)
        {
            string response = "";

            switch (words[0])
            {
                case "height?":
                    response = $"{Height}dm";
                    break;
                default:
                    break;
            }

            return response;
        }

        /// <summary>
        /// Land the drone
        /// </summary>
        private void Land()
        {
            _timer.Enabled = false;
            Position.Y = 0;
            InFlight = false;
        }

        /// <summary>
        /// Move the drone forward on the current heading
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="isForward"></param>
        private void MoveForwardOrBackward(decimal amount, bool isForward)
        {
            decimal movementHeading = (isForward) ? Heading : WrapAngle(Heading + 180);
            switch (movementHeading)
            {
                case 0:
                    Position.Z -= amount;
                    break;
                case 90:
                    Position.X += amount;
                    break;
                case 180:
                    Position.Z += amount;
                    break;
                case 270:
                    Position.X -= amount;
                    break;
                default:
                    double radians = Math.PI * (double)movementHeading / 180.0;
                    Position.X += Decimal.Round(amount * (decimal)Math.Sin(radians), 3);
                    Position.Z -= Decimal.Round(amount * (decimal)Math.Cos(radians), 3);
                    break;
            }
        }

        /// <summary>
        /// Wrap an angle into the range 0-360 degrees
        /// </summary>
        /// <param name="heading"></param>
        /// <returns></returns>
        private int WrapAngle(int heading)
        {
            heading %= 360;
            if (heading < 0)
            {
                heading += 360;
            }

            return heading;
        }

        /// <summary>
        /// Confirm that the drone is in flight, throwing an exception if not
        /// </summary>
        private void AssertIsInFlight()
        {
            if (!InFlight)
            {
                throw new NotInFlightException("The drone is not in flight");
            }
        }

        /// <summary>
        /// Drain the battery
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnBatteryTimer(object sender, ElapsedEventArgs e)
        {
            decimal batteryDropPerSecond = (100M - (decimal)CriticalBatteryLevel) / (decimal)SingleChargeFlightTimeSeconds;
            Battery = (int)(100M - TimeOfFlight * batteryDropPerSecond);
            if (Battery <= CriticalBatteryLevel)
            {
                BatteryCritical?.Invoke(this, new BatteryCriticalEventArgs { Battery = Battery, TimeOfFlight = TimeOfFlight });
                Land();
            }
        }
    }
}
