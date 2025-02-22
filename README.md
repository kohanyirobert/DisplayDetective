# 🕵️‍♂️ DisplayDetective

## 📃 Overview

DisplayDetective is a CLI tool to monitor display devices by their IDs. It executes specified commands on display connection or disconnection.

## 🎯 Project Goals

- 🖥️ **Monitor Displays** detect and respond to display connections and disconnections
- 🛠️ **Run Commands** execute commands on such events
- 💻 **Terminal Usage** usable from the terminal
- ⚙️ **Configuration** configuration via `appsettings.json`
- 🧑‍💻 **Practice C#** improve C#, .NET skills
- 🔧 **Open Source** use VS Code and C# extensions
- 🤖 **AI** utilize AI technologies during development

## ⚒️ Development

### ☝️ Prerequisites

- 🛠️ .NET SDK 9.0
- 🖥️ Visual Studio Code with C# extensions

### 🔨 Tools

- [Husky.Net](https://alirezanet.github.io/Husky.Net/)

## ⚙️ Configuration

The application uses [`appsettings.json`](appsettings.json) for configuration, use it as a reference.

Since `appsettings{,.Development}.json` file(s) are checked into VCS during development
it's best to edit your user secrets file ([see related documentation](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)).

## 📚 Usage

### 🏃‍♂️ Running

```sh
git clone ... repo
cd repo
dotnet build
dotnet test
cd DisplayDetective.CommandLineApp
dotnet run -- list
dotnet run -- monitor
```

## 🤝 Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

## 📄 License

This project is licensed under the MIT License.