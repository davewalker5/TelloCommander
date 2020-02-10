# TelloCommanderConsole

[![Build Status](https://github.com/davewalker5/TelloCommanderConsole/workflows/.NET%20Core%20CI%20Build/badge.svg)](https://github.com/davewalker5/TelloCommanderConsole/actions)
[![GitHub issues](https://img.shields.io/github/issues/davewalker5/TelloCommanderConsole)](https://github.com/davewalker5/TelloCommander/issues)
[![Releases](https://img.shields.io/github/v/release/davewalker5/TelloCommanderConsole.svg?include_prereleases)](https://github.com/davewalker5/TelloCommanderConsole/releases)
[![NuGet](https://img.shields.io/nuget/v/TelloCommander.CommandLine)](https://www.nuget.org/packages?q=tellocommander.commandline)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/davewalker5/TelloCommanderConsole/blob/master/LICENSE)
[![Language](https://img.shields.io/badge/language-c%23-blue.svg)](https://github.com/davewalker5/TelloCommanderConsole/)
[![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/davewalker5/TelloCommanderConsole)](https://github.com/davewalker5/TelloCommanderConsole/)

## About

TelloCommanderConsole is a console application that demonstrates use of the [TelloCommander API](https://github.com/davewalker5/TelloCommander) to connect to and communicate with a drone.

It provides the following connection types:

| Type | Purpose |
| --- | --- |
| Mock | Uses a mock that simulates responses from the drone without establishing a connection |
| Simulator | The application is connected to the simulator, running on the same machine |
| Drone | The application is connected to a real drone |

It is based on the ConsoleCommander class provided by the API.

## Pre-requisites

You will need .NET Core 3.1.101 and the .NET CLI installed to build and run the application. Instructions for downloading and installing are on the .NET website:

[https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)

## Building the Application

* Clone or download the repository
* Open a terminal window and CD to the folder containing the TelloCommanderConsole.sln solution file
* Enter the following command to build the application:

```
dotnet build
```

## Creating the Telemetry Capture Database

The application includes support for writing drone telemetry to a SQLite database. If you intend to use this facility, you must first fulfil the [prerequisites for SQLite database capture](https://github.com/davewalker5/TelloCommander/wiki/Writing-Drone-Telemetry-to-a-SQLite-Database).

## Running the Application

To run the application, open a terminal window, change to the folder containing the compiled output for the TelloCommanderConsole project and run the following command:

```
TelloCommanderConsole
```

The following will be displayed:

```
Tello Commander 1.0.0.0

Dictionary Version:

[1] 1.3.0.0
[2] 2.0.0.0

Please enter your option between 1 and 2 or enter 0 to quit:
```

### Selecting an API Version

The dictionary version determines the set of commands available to run. For mock or simulator connections, any version can be used but for connection to a real drone it's important to choose the correct version:

| Drone | Version |
|  --- | --- |
| Tello | 1.3.0.0 |
| Tello EDU | 2.0.0.0 |

### Selecting a Connection Type

Once you have selected the version, you are prompted to enter the required connection type:

```
Connection:

[1] Mock connection
[2] Simulator connection
[3] Real connection

Please enter your option between 1 and 3 or enter 0 to quit:
```

The simulator is part of the [TelloCommander project](https://github.com/davewalker5/TelloCommander) and instructions on how to start it are given in the [project wiki](https://github.com/davewalker5/TelloCommander/wiki/Home). If you're connecting to the simulator, start it before entering the connection type.

If you're connecting to a drone, switch it on and connect to its WiFi before entering the connection type.

Once a connection type's been entered, the application will respond as follows, indicating it's connected and ready to receive and send commands:

```
You are connected to the Tello in API mode

Enter command or hit ENTER to quit :
```

### Entering Commands

The application repeatedly prompts for and sends commands, echoing the reponse from the connection, until instructed to stop. For example:

```
Enter command or hit ENTER to quit : takeoff
Command : takeoff
Response: ok

Enter command or hit ENTER to quit : land
Command : land
Response: ok

Enter command or hit ENTER to quit :
```

In addition to the commands supported by the drone and the custom commands provided by the [TelloCommander API](https://github.com/davewalker5/TelloCommander), the console application also supports the following commands:

| Command | Parameters | Purpose |
|  --- | --- | --- |
| ? | None | Report the current status to the console |
| startcapture | file interval | Start writing telemetry to the specified file (in CSV format) at the specified interval (in milliseconds) |
| stopcapture | None | Stop file capture if it's in progress |
| startdbcapture | drone session interval [property list] | Start writing telemetry to th SQLite database defined in the appsettings.json file |
| stopdbcapture | None | Stop writing telemetry to a database if it's in  progress |

The arguments to the "startdbcapture" command are as follows:

| Name | Description |
| --- | --- |
| drone | Name of the drone, as a single word e.g. TELLO-12345 |
| session | Name of the capture session, as a single word e.g. LoopTheLoop-001 |
| interval | The collection interval, in milliseconds e.g. 1000 = 1 second. A value <= 0 defaults to 1000 ms |
| property list | A comma-separated list of the properties to capture e.g. pitch,roll,yaw |

Valid properties for the property list are those returned in the Tello status, as documented in the drone APIs:

* [Tello SDK Documentation EN_1.3.pdf](https://dl-cdn.ryzerobotics.com/downloads/tello/20180910/Tello%20SDK%20Documentation%20EN_1.3.pdf)

For example, to collect the height and battery charge at 1s intervals, use the folowing:

```
startdbcapture MY-TELLO My-Session-1 1000 h,bat
```

### Closing the Connection

Before closing the application, please:

- Land the drone, if you are connected to a real drone
- Stop the simulator, by issuing the "stopsimulator" command, if you are using a simulator connection

To close the connection and stop the application, enter an empty command in response to the prompt.

## Wiki

* [TelloCommander Wiki](https://github.com/davewalker5/TelloCommander/wiki/Home)

## Authors

- **Dave Walker** - *Initial work* - [LinkedIn](https://www.linkedin.com/in/davewalker5/)

## Feedback

To file issues or suggestions, please use the [Issues](https://github.com/davewalker5/TelloCommander/issues) page for this project on GitHub.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## Trello

*  [TelloCommander on Trello](https://trello.com/b/VCFq6tAk)
