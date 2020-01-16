# TelloCommander

[![Build Status](https://github.com/davewalker5/TelloCommander/workflows/.NET%20Core%20CI%20Build/badge.svg)](https://github.com/davewalker5/TelloCommander/actions)
[![GitHub issues](https://img.shields.io/github/issues/davewalker5/TelloCommander)](https://github.com/davewalker5/TelloCommander/issues)
[![Coverage Status](https://coveralls.io/repos/github/davewalker5/TelloCommander/badge.svg?branch=master)](https://coveralls.io/github/davewalker5/TelloCommander?branch=master)
[![Releases](https://img.shields.io/github/v/release/davewalker5/TelloCommander.svg?include_prereleases)](https://github.com/davewalker5/TelloCommander/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/davewalker5/TelloCommander/blob/master/LICENSE)
[![Language](https://img.shields.io/badge/language-c%23-blue.svg)](https://github.com/davewalker5/TelloCommander/)
[![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/davewalker5/TelloCommander)](https://github.com/davewalker5/TelloCommander/)

## About

Tello Commander is a C# API for controlling a Tello drone, offering the following features:

- A simple interface to connect to, control and disconnect from the drone
- Validation of all commands to ensure only valid commands are sent to the drone
- Command/response history
- Response parser for processing drone "read" command responses
- Execution of commands from pre-prepared scripts
- Background monitoring and reporting of drone status
- Capture of status information to CSV

## Getting Started

Once the API is referenced by a project, you should include the following "using" statements to import the necessary types:

```csharp
using TelloCommander.CommandDictionaries;
using TelloCommander.Commander;
using TelloCommander.Connections;
using TelloCommander.Interfaces;
```

The following code snippet can be pasted into the Main() method of a C# console application to demonstrate connection to the drone, command processing and disconnection from the drone:

```csharp
// Connect to the drone
var dictionary = CommandDictionary.ReadStandardDictionary("1.3.0.0");
var commander = new DroneCommander(new TelloConnection(), dictionary);
commander.Connect();

// Ask for a command to process and process it. Repeat until the an empty
// command is entered
bool isEmpty;
do
{
    Console.Write("Please enter a command or press [ENTER] to quit : ");
    string command = Console.ReadLine().Trim();
    isEmpty = string.IsNullOrEmpty(command);
    if (!isEmpty)
    {
        try
        {
            // Process the command using the API
            Console.WriteLine($"Command  : {command}");
            commander.RunCommand(command);
            Console.WriteLine($"Response : {commander.LastResponse}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
while (!isEmpty);

// Disconnect from the drone
commander.Disconnect();
```

The argument passed to the "ReadStandardDictionary" is the Tello API version number and defines the set of available commands (see the [wiki](https://github.com/davewalker5/TelloCommander/wiki/Home)) for more  details.

The following is example output for a simple takeoff, height query and landing:

```
Please enter a command or press [ENTER] to quit : takeoff
Command  : takeoff
Response : ok
Please enter a command or press [ENTER] to quit : height?
Command  : height?
Response : 6dm
Please enter a command or press [ENTER] to quit : land
Command  : land
Response : ok
```

## Monitoring the Status of the Drone

The DroneStatusMonitor class reads the status from the drone, parses the raw status and presents those values to the calling application. It also provides an event subscription model for receiving updates to the drone status.

To use it, first reference the required namespaces:

```csharp
using TelloCommander.Status;
```

The following code snippet can be cut and pasted into a console application's "Program" class to demonstrate the status monitor:

```csharp
static void Main(string[] args)
{
    // Note that the drone must already be in API mode for the monitoring
    // to work. Create an instance of the monitor and start listening in
    // the background. Subscribe to status update events to report status
    DroneStatusMonitor monitor = new DroneStatusMonitor();
    monitor.Listen(DroneStatusMonitor.DefaultTelloStatusPort);
    monitor.DroneStatusUpdated += OnDroneStatusUpdated;

    // Optionally, capture the status to a CSV file
    // monitor.StartCapture("path_to_csv", interval_in_milliseconds);

    while (true)
    {
        Thread.Sleep(1000);
    }
}

private static void OnDroneStatusUpdated(object sender, DroneStatusEventArgs e)
{
    Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Status : {e.Status.Status}");
    if (!string.IsNullOrEmpty(e.Status.Error))
    {
        Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Error  : {e.Status.Error}");
    }
    else if (!string.IsNullOrEmpty(e.Status.Status))
    {
        // If there's no error and the status has a value, it's been successfully read and
        // the public properties on the monitor will be populated. The sequence number is
        // incremented every time the status is read from the drone. If it stops increasing
        // for any length of time, then either the monitor has stopped or the drone has
        // stopped  broadcasting its status
        Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Sequence     : {e.Status.Sequence}");
        Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Attitude     : {e.Status.Attitude.ToString()}");
        Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Speed        : {e.Status.Speed.ToString()}");
        Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Temperature  : {e.Status.Temperature.ToString()}");
        Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} TOF          : {e.Status.TOF}");
        Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Height       : {e.Status.Height}");
        Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Battery      : {e.Status.Battery}");
        Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Barometer    : {e.Status.Battery}");
        Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Time         : {e.Status.Time}");
        Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff")} Acceleration : {e.Status.Acceleration.ToString()}");
    }
}
```

The status is reported as follows:

```
09:54:23.639 Status : pitch:0;roll:0;yaw:0;vgx:0;vgy:0;vgz:0;templ:0;temph:0;tof:0;h:0;bat:0;baro:0.00;time:0;agx:0.00;agy:0.00;agz:0.00;
09:54:23.639 Sequence     : 5
09:54:23.639 Attitude     : Pitch: 0 Roll: 0 Yaw: 0
09:54:23.640 Speed        : X: 0 Y: 0 Z: 0
09:54:23.640 Temperature  : Minimum: 0 Maximum: 0
09:54:23.640 TOF          : 0
09:54:23.640 Height       : 0
09:54:23.640 Battery      : 0
09:54:23.640 Barometer    : 0
09:54:23.640 Time         : 0
09:54:23.640 Acceleration : X: 0.00 Y: 0.00 Z: 0.00
```

Please note that the drone must be in API mode in order for drone status to be reported. This can be achieved by creating a DroneCommander and connecting to the drone (see above).

## Putting it Together

The ConsoleCommander class demonstrates background status monitoring with an interactive console-based drone commander. To use it, first reference the required namespaces:

```csharp
using TelloCommander.CommandDictionaries;
using TelloCommander.Connections;
using TelloCommander.Interfaces;
```

The following snippet can then be pasted into the Main() method of a console application to run the application:

```csharp
CommandDictionary dictionary = CommandDictionary.ReadStandardDictionary("1.3.0.0");
new ConsoleCommander(new TelloConnection(), dictionary).Run(true);
```

The argument passed to the "ReadStandardDictionary" is the Tello API version number and defines the set of available commands (see the [wiki](https://github.com/davewalker5/TelloCommander/wiki/Home)) for more  details.

In addition to the commands supported by the drone and custom commands supported by the DroneCommander, you can enter "?" to report the status of the drone.

## Wiki

More complete information on the capabilities and use of the API are provided in the Wiki:

* [About TelloCommander](https://github.com/davewalker5/TelloCommander/wiki/Home)
* [Building and testing the API](https://github.com/davewalker5/TelloCommander/wiki/Building-and-Testing-the-API)
* [The demonstration application](https://github.com/davewalker5/TelloCommander/wiki/Demonstration-Application)
* [The drone simulator](https://github.com/davewalker5/TelloCommander/wiki/Drone-Simulator)
* [Command dictionaries](https://github.com/davewalker5/TelloCommander/wiki/Command-Dictionaries)
* [Custom commands and scripting](https://github.com/davewalker5/TelloCommander/wiki/Custom-Commands-And-Scripting)
* [Creating applications using the API](https://github.com/davewalker5/TelloCommander/wiki/Creating-Applications-With-the-Api)
* [Streaming Video](https://github.com/davewalker5/TelloCommander/wiki/Streaming-Video)

## Authors

- **Dave Walker** - *Initial work* - [LinkedIn](https://www.linkedin.com/in/davewalker5/)

## Feedback

To file issues or suggestions, please use the [Issues](https://github.com/davewalker5/TelloCommander/issues) page for this project on GitHub.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## Trello

*  [TelloCommander on Trello](https://trello.com/b/VCFq6tAk)
