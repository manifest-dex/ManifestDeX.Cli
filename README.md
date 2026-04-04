# ManifestDeX CLI - Open Source .NET 10 CLI for Manifest, App, and Depot Key Queries

ManifestDeX CLI is an open-source C# .NET 10 command-line client for ManifestDeX.

Official website: [https://manifestdex.com](https://manifestdex.com)  
Developer API key portal: [https://manifestdex.com/developer/cli](https://manifestdex.com/developer/cli)  
GitHub repository: [https://github.com/manifest-dex/ManifestDeX.Cli](https://github.com/manifest-dex/ManifestDeX.Cli)

## Why ManifestDeX CLI

- Clean Architecture (`Domain`, `Application`, `Infrastructure`, `Presentation`, `Bootstrapper`).
- Spectre.Console.Cli based command system.
- Single EXE publish target for Windows x64.
- Human-readable table output and script-friendly JSON output.
- Server-side auth, quota, and anti-abuse enforcement.

## Access and API Key

- The repository is open source.
- API access requires a valid key.
- Keys are created from: [https://manifestdex.com/developer/cli](https://manifestdex.com/developer/cli)
- Subscription and usage policy are enforced by ManifestDeX backend.

## Requirements

- Windows x64
- .NET SDK 10.x (for build/test)
- Valid ManifestDeX CLI API key

## Build

```powershell
dotnet build .\ManifestDeX.Cli.slnx -v minimal
```

## Publish Single EXE

```powershell
dotnet publish .\src\ManifestDeX.Cli\ManifestDeX.Cli.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

Published file:

`.\src\ManifestDeX.Cli\bin\Release\net10.0\win-x64\publish\ManifestDeX.Cli.exe`

## Quick Start

From publish directory:

```powershell
ManifestDeX.Cli.exe auth set-key <your_key>
ManifestDeX.Cli.exe auth status
ManifestDeX.Cli.exe search "counter strike"
```

## Command Reference

| Command              | Aliases      | Description                                                                       |
| -------------------- | ------------ | --------------------------------------------------------------------------------- |
| `search <query>`     | `find`, `s`  | Search games and return appId + key count.                                        |
| `info <appId>`       | `i`          | Return app details (`name`, `headerImageUrl`, `totalDecryptionKeys`, depot list). |
| `get <appId>`        | `keys`, `g`  | Return depot keys (`depotId:key`).                                                |
| `usage`              | `quota`, `u` | Return daily usage and reset time.                                                |
| `health`             | `ping`       | Return API health status.                                                         |
| `help`               | `h`          | Show detailed CLI help.                                                           |
| `auth set-key <key>` | `auth set`   | Save local API key.                                                               |
| `auth status`        | `auth check` | Validate key and print usage snapshot.                                            |

## Output Modes

- `table` (default)
- `json` (`--json` or `--output json`)

Examples:

```powershell
ManifestDeX.Cli.exe info 730
ManifestDeX.Cli.exe get 730 --json
ManifestDeX.Cli.exe usage --output table
ManifestDeX.Cli.exe health --json
```

## Real Example (Table Output)

Command:

```powershell
ManifestDeX.Cli.exe info 730
```

Example output:

```text
┌───────────────────────┬──────────────────────────────────────────────────────────────────────────────────────────────┐
│ Field                 │ Value                                                                                        │
├───────────────────────┼──────────────────────────────────────────────────────────────────────────────────────────────┤
│ AppId                 │ 730                                                                                          │
│ Name                  │ Counter-Strike 2                                                                             │
│ Header image URL      │ https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/730/header.jpg            │
│ Total decryption keys │ 13                                                                                           │
│ Depots                │ 731, 732, 733, 734, 735, 228988, 228990, 2347770, 2347771, 2347772, 2347773, 2347774,        │
│                       │ 2347779                                                                                      │
└───────────────────────┴──────────────────────────────────────────────────────────────────────────────────────────────┘
```

## Real Example (JSON Output)

Command:

```powershell
ManifestDeX.Cli.exe get 730 --json
```

Example output:

```json
[
  {
    "AppId": 730,
    "DepotId": 731,
    "Key": "********************************"
  }
]
```

## Configuration

### API Key File

Stored at:

`%LOCALAPPDATA%\ManifestDeX\cli-config.json`

## Exit Codes

| Code | Name            | Meaning                             |
| ---- | --------------- | ----------------------------------- |
| `0`  | Success         | Command completed.                  |
| `2`  | ValidationError | Invalid input or missing API key.   |
| `3`  | Unauthorized    | API key is invalid or unauthorized. |
| `4`  | Forbidden       | Access denied by policy.            |
| `5`  | RateLimited     | Rate limit reached.                 |
| `6`  | NetworkError    | Network/connectivity error.         |
| `10` | UnknownError    | Unexpected error.                   |

## Project Structure

```text
ManifestDeX.Cli
|- src
|  |- ManifestDeX.Cli.Domain
|  |- ManifestDeX.Cli.Application
|  |- ManifestDeX.Cli.Infrastructure
|  |- ManifestDeX.Cli.Presentation
|  |- ManifestDeX.Cli.Bootstrapper
|  `- ManifestDeX.Cli
`- tests
   |- ManifestDeX.Cli.UnitTests
   `- ManifestDeX.Cli.IntegrationTests
```

## Test

```powershell
dotnet test .\ManifestDeX.Cli.slnx -v minimal
```

## License

MIT License. See [LICENSE](./LICENSE).
