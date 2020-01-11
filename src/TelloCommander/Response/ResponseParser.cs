using System;
using System.Collections.Generic;
using System.Linq;

namespace TelloCommander.Response
{
    public static class ResponseParser
    {
        private static readonly char[] _numeric = { '-', '.' };

        /// <summary>
        /// Parse a value string to a number
        /// </summary>
        /// <param name="valueString"></param>
        /// <returns></returns>
        public static decimal ParseToNumber(string valueString)
        {
            string numericString = new string(valueString.Where(c => char.IsDigit(c) || (_numeric.Contains(c))).ToArray());
            decimal value = decimal.Parse(numericString);
            return value;
        }

        /// <summary>
        /// Parse a range response into a tuple of numbers
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static (decimal minimum, decimal maximum) ParseToRange(string response)
        {
            // A typical range might look like this:
            //
            // 65~66C
            string[] words = response.Trim().Split(new char[] { '~' }, StringSplitOptions.None);
            decimal minimum = ParseToNumber(words[0]);
            decimal maximum = ParseToNumber(words[1]);

            return (minimum, maximum);
        }

        /// <summary>
        /// Parse a multi-property response into a dictionary of properties
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseToDictionary(string response)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            // A typical response might look like this, with the property name and value
            // in pairs separated by ":" and each property separated from the last by ";"
            //
            // agx:118.00;agy:-54.00;agz:-991.00;

            // If the response ends with the key-value pair ";" separator, we need to
            // remove it so we don't end up with a spurious empty entry when we split
            // the string. We can't use the string split option to remove empty entries
            // as there may be legitimate blank values in the response
            if (response.EndsWith(";", StringComparison.CurrentCulture))
            {
                response = response.Substring(0, response.Length - 1);
            }

            // Split the response on the value and pair separators
            string[] words = response.Trim().Split(new char[] { ':', ';' }, StringSplitOptions.None);
            for (int i = 0; i <= words.Length - 2; i += 2)
            {
                values.Add(words[i], words[i + 1]);
            }

            return values;
        }

        /// <summary>
        /// Parse a response into an acceleration
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static Acceleration ParseToAcceleration(string response)
        {
            Dictionary<string, string> properties = ParseToDictionary(response);

            Acceleration acceleration = new Acceleration
            {
                X = ParseToNumber(properties["agx"]),
                Y = ParseToNumber(properties["agy"]),
                Z = ParseToNumber(properties["agz"])
            };

            return acceleration;
        }

        /// <summary>
        /// Parse a response into an attitude
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static Attitude ParseToAttitude(string response)
        {
            Dictionary<string, string> properties = ParseToDictionary(response);

            Attitude attitude = new Attitude
            {
                Pitch = ParseToNumber(properties["pitch"]),
                Roll = ParseToNumber(properties["roll"]),
                Yaw = ParseToNumber(properties["yaw"])
            };

            return attitude;
        }

        /// <summary>
        /// Parse a response into a speed
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static Speed ParseToSpeed(string response)
        {
            Dictionary<string, string> properties = ParseToDictionary(response);

            Speed speed = new Speed
            {
                X = ResponseParser.ParseToNumber(properties["vgx"]),
                Y = ResponseParser.ParseToNumber(properties["vgy"]),
                Z = ResponseParser.ParseToNumber(properties["vgz"])
            };

            return speed;
        }

        /// <summary>
        /// Parse a response containing a range into a temperature range
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static Temperature ParseToTemperature(string response)
        {
            (decimal minimum, decimal maximum) tuple = ParseToRange(response);

            Temperature temperature = new Temperature
            {
                Minimum = tuple.minimum,
                Maximum = tuple.maximum
            };

            return temperature;
        }

        /// <summary>
        /// Parse a response containing a list of properties into a temperature range
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static Temperature ParseTemperatureFromPropertyList(string response)
        {
            Dictionary<string, string> properties = ParseToDictionary(response);

            Temperature temperature = new Temperature
            {
                Minimum = ResponseParser.ParseToNumber(properties["templ"]),
                Maximum = ResponseParser.ParseToNumber(properties["temph"])
            };

            return temperature;
        }
    }
}
