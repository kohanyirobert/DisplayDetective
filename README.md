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
- 📦 npm/npx

### 🔨 Tools  & 🧠 Concepts

- [Husky.Net](https://alirezanet.github.io/Husky.Net/)
- [EasyBuild](https://github.com/easybuild-org/)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Conventional Branch](https://conventional-branch.github.io/)
- [CommitLinter](https://github.com/easybuild-org/EasyBuild.CommitLinter)
- [gitmoji](https://gitmoji.dev/) via [devmoji](https://github.com/folke/devmoji)
- [commit-check](https://github.com/commit-check/commit-check) and [its GitHub Action counterpart](https://github.com/commit-check/commit-check-action)
- GitHub [rulesets](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/about-rulesets) and [status checks](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/collaborating-on-repositories-with-code-quality-features/about-status-checks) (see [this repo's rules](https://github.com/kohanyirobert/DisplayDetective/rules))

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

### 📝 Notes

- I don't fully understand what the [*Do not require status checks on creation*](https://docs.github.com/en/enterprise-cloud@latest/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/available-rules-for-rulesets#require-status-checks-to-pass-before-merging) option mean