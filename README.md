# _LayTea_ [![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://choosealicense.com/licenses/mit/) ![Build and release](https://github.com/pleonex/LayTea/workflows/Build%20and%20release/badge.svg)

**Work-in-progress** modding tools for games of the _"Professor Layton"_.

## Features

🎮 Games:

- Documentation for
  [Professor Layton - London Life](https://www.pleonex.dev/LayTea/guides/games/london_life/summary.html)

📃 Formats:

- Professor Layton - London Life (US only):
  - **`DARC` containers**: read and write.
  - **`DENC` containers**: read and write.
  - **`ARCHIVE` containers**: read and write.
  - **`DENC-LZSS` compression**: decompress and compress.
  - **`LZX` compression**: decompress.
  - **`MSG` texts**: read and write.
    - PO export and import.
  - **`ACL` palettes**: read.
  - **`ACG` and `ACB` pixels**: read.
  - **`ASC` screen maps**: read.
  - **`NCCL` palettes**: read.
  - **`NCCG` pixels**: read.
  - **`NCSC` screen maps**: read.

## Documentation

Feel free to ask any question in the
[project Discussion site!](https://github.com/pleonex/LayTea/discussions)

Check our on-line [documentation](https://pleonex.dev/LayTea) for more
information about the file formats and how to use the tools.

## Usage

The project delivers a programming .NET library with the support of the formats
and a console program to extract and import the game files.

- Library:
  - `SceneGate.Games.ProfessorLayton`: available to download via the
    [SceneGate AzureDevOps NuGet feed](https://dev.azure.com/SceneGate/SceneGate/_artifacts/feed/SceneGate-Preview).
- Program:
  - _LayTeaConsole_: it is not available to download yet.

To use the .NET library adds a `nuget.config` file in the same directory as your
solution file (.sln) with the following content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear/>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="SceneGate-Preview" value="https://pkgs.dev.azure.com/SceneGate/SceneGate/_packaging/SceneGate-Preview/nuget/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="nuget.org">
      <package pattern="*" />
    </packageSource>

    <packageSource key=" SceneGate-Preview">
      <package pattern="Yarhl*" />
      <package pattern="SceneGate.*" />
      <package pattern="Texim" />
    </packageSource>
  </packageSourceMapping>
</configuration>
```

## Contributing

The repository requires to build .NET 8.0 SDK.

To build, test and generate artifacts run:

```sh
# Build and run tests
dotnet run --project build/orchestrator

# (Optional) Create bundles (nuget, zips, docs)
dotnet run --project build/orchestrator -- --target=Bundle
```

Additionally you can use _Visual Studio_ or _JetBrains Rider_ as any other .NET
project.

To contribute follow the [contributing guidelines](CONTRIBUTING.md).

## License

The software is licensed under the terms of the
[MIT license](https://choosealicense.com/licenses/mit/). The information and
software provided by this repository is only for educational and research
purpose. Please support the original game developers by buying their games.

Icon by Vincent Le Moign,
[CC BY 4.0](https://creativecommons.org/licenses/by/4.0), via
[Wikimedia Commons](https://commons.wikimedia.org/wiki/File:568-teacup-without-handle.svg).
