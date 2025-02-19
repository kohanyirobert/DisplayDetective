# DisplayDetective 🕵️‍♂️

## Overview

DisplayDetective is a CLI tool to monitor display devices by their IDs. It executes specified commands on display connection or disconnection.

## Project Goals

- 🖥️ **Monitor Displays** detect and respond to display connections and disconnections
- 🛠️ **Run Commands** execute commands on such events
- 💻 **Terminal Usage** usable from the terminal
- ⚙️ **Configuration** configuration via `appsettings.json`
- 🧑‍💻 **Practice C#** improve C#, .NET skills
- 🔧 **Open Source** use VS Code and C# extensions
- 🤖 **AI** utilize AI technologies during development

## Usage

### Running the Application

1. **List Displays:**
   ```sh
   dotnet run --project DisplayDetective.CommandLineApp list
   ```

2. **Monitor a Display:**
   ```sh
   dotnet run --project DisplayDetective.CommandLineApp monitor --device-id <DeviceID>
   ```

### Configuration

The application uses `appsettings.json` for configuration.

Example `appsettings.json`:
```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft": "Warning",
            "Microsoft.Hosting.Lifetime": "Warning"
        },
        "Console": {
            "FormatterName": "Simple",
            "FormatterOptions": {
                "SingleLine": true,
                "TimestampFormat": "HH:mm:ss ",
                "UseUtcTimestamp": false
            }
        }
    },
    "DisplayDetective": {
        "Watches": {
            "<DeviceID>": {
                "CreateCommand": "path/to/create/command",
                "CreateArguments": ["arg1", "arg2"],
                "DeleteCommand": "path/to/delete/command",
                "DeleteArguments": ["arg1", "arg2"]
            }       
        }
    }
}
```

Replace `<DeviceID>` with the actual device ID of the display you want to monitor.

## Development

### Prerequisites

- .NET SDK 9.0
- Visual Studio Code with C# extensions

### Building the Solution

```sh
dotnet build
```

### Running Tests

```sh
dotnet test
```

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

## License

This project is licensed under the MIT License.