# TelloCommander

TelloCommander is a C# API for controlling a Tello Drone consisting of the following components:

| Project Name | Type | Targets | Comments |
| --- | --- | --- | --- |
| TelloCommander | Class library | .NET Standard 2.1 | API |
| TelloCommander.TestApp | Console | .NET Core 3.0 | Demonstration interactive console application for controlling the drone |
| TelloCommander.Tests | MS Test | .NET  Core 3.0 | Unit Tests |
| TelloSimulator | Console | .NET Core 3.0 | Simulator that can be used with the test application to simulate control of the drone |

## Prerequisites

You will need .NET Core 3.0 and the .NET CLI installed to build and run the API and test applications. Instructions for downloading and installing are on the .NET website:

[https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)

This documentation demonstrates use of the .NET command line, rather than an IDE, for building, testing and running the API and test applications.

## Building the Projects

To build the API and test applications, open a terminal window, change to the "src" folder of your working copy and run the following:

```shell
dotnet restore TelloCommander.sln
dotnet build TelloCommander.sln
```

## Running the Unit Tests

To run the unit tests, open a terminal window, change to the "src" folder of your working copy and run the following:

```shell
dotnet test TelloCommander.sln
```

## The Test Application

### Running the Application

To run the demonstration/test application, open a terminal window, change to the folder containing the compiled output for the TelloCommander.TestApp project and run the following command:

```shell
TelloCommander.TestApp
```

The following will be displayed:

```shell
Tello Commander 1.0.0.0

Dictionary Version:

[1] 1.3.0.0
[2] 2.0.0.0

Please enter your option between 1 and 2 or enter 0 to quit:
```

### Selecting an API Version

The dictionary (or API) version determines the set of commands available to run. For mock or simulator connections (see below), any version can be used but for connection to a real drone it's important to choose the  correct version:

| Drone | Version |
|  --- | --- |
| Tello | 1.3.0.0 |
| Tello EDU | 2.0.0.0 |

### Selecting a Connection Type

Once you have selected the API version, you are prompted to enter the required connection type:

```shell
Connection:

[1] Mock connection
[2] Simulator connection
[3] Real connection

Please enter your option between 1 and 3 or enter 0 to quit:
```

| Type | Purpose |
| --- | --- |
| Mock | Uses a mock connection that simulates responses from the drone without establishing a connection |
| Simulator | Connects to the simulator, running on the same machine |
| Drone | Connects to a real drone |

If you're connecting to the simulator, start it before entering the connection type (see instructions, below).

If you're connecting to a drone, switch it on and connect to its WiFi before entering the connection type.

Once a connection type's been entered, the test application will respond as follows, indicating it's connected and ready to receive and send commands:

```shell
You are connected to the Tello in API mode

Enter command or hit ENTER to quit :
```

### Entering Commands

The test application repeatedly prompts for and sends commands, echoing the reponse from the connection, until instructed to stop. For example:

```shell
Enter command or hit ENTER to quit : takeoff
Command : takeoff
Response: ok

Enter command or hit ENTER to quit : land
Command : land
Response: ok

Enter command or hit ENTER to quit :
```

The available commands are those documented in the API documentation for the drone plus custom commands, that are documented in the repository Wiki.

### Closing the Connection

Before closing the test application, please:

- Land the drone, if you are connected to a real drone
- Stop the simulator, by issuing the "stopsimulator" command, if you are using a simulator connection

To close the connection and stop the test application, enter an empty command in response to the prompt.

## The Drone Simulator

The drone simulator is a console application that listens on port 8889 on the local machine for commands, responding to them in the same manner as a real drone.

Unlike the mock connection (see above), it tests a real connection from the test application and API without requiring a real drone.

### Starting the Simulator

To run the simulator, open a terminal window, change to the folder containing the compiled output for the TelloSimulator project and run the following command:

```shell
TelloSimulator <version>
```

Where "version" is one of the supported API versions (see above). For example:

```shell
TelloSimulator 1.3.0.0
```

The following will be displayed, indicating the simulator has started and is ready to receive commands:

```shell
Tello Simulator 1.0.0.0
Using API/dictionary version 1.3.0.0
Listening on port 8889
```

### Command Receipt and Processing

When an incoming command is received from the test application, the simulator responds by echoing the command and the response to the console:

```shell
Received command
Response ok
Received takeoff
Response ok
Received land
Response ok
```

### Stopping the Simulator

To stop the simulator from the test application, just enter the following command in a connected test application:

```shell
stopsimulator
```

The simulator will respond as follows and will then exit, returning to the terminal prompt:

```shell
Received stopsimulator
Response ok
```

Alternatively, simply use CTRL+C to stop the simulator.

## Further Information

Further information on the commands available and how to use the API to build your own application are in the Wiki for this repository.

## Authors

- **Dave Walker** - *Initial work* - [LinkedIn](https://www.linkedin.com/in/davewalker5/)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
