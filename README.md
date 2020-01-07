# TelloCommander

[![Build Status](https://github.com/davewalker5/TelloCommander/workflows/.NET%20Core%20CI%20Build/badge.svg)](https://github.com/davewalker5/TelloCommander/actions)
[![GitHub issues](https://img.shields.io/github/issues/davewalker5/TelloCommander)](https://github.com/davewalker5/TelloCommander/issues)
[![Coverage Status](https://coveralls.io/repos/github/davewalker5/TelloCommander/badge.svg?branch=master)](https://coveralls.io/github/davewalker5/TelloCommander?branch=master)
[![Releases](https://img.shields.io/github/v/release/davewalker5/TelloCommander.svg?include_prereleases)](https://github.com/davewalker5/TelloCommander/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/davewalker5/TelloCommander/blob/master/LICENSE)
[![Language](https://img.shields.io/badge/language-c%23-blue.svg)](https://github.com/davewalker5/TelloCommander/)
[![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/davewalker5/TelloCommander)](https://github.com/davewalker5/TelloCommander/)

Tello Commander is a C# API for controlling a Tello drone, offering the following features:

- A simple interface to connect to, control and disconnect from the drone
- Validation of all commands to ensure only valid commands are sent to the drone
- Command/response history
- Response parser for processing drone "read" command responses
- Execution of commands from pre-prepared scripts

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
