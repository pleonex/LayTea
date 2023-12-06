# _LayTea_ [![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://choosealicense.com/licenses/mit/)

**Work-in-progress** modding tools for games of the **_"Professor Layton"_**
saga.

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

## Tool installation

1. Install [.NET 8.0](https://dotnet.microsoft.com/en-us/download)
2. Install the latest version of the tool: `dotnet tool install -g LayTea`
   - You can update it with `dotnet tool update -g LayTea`
   - To use preview versions, add the argument
     `--prerelease --add-source https://pkgs.dev.azure.com/SceneGate/SceneGate/_packaging/SceneGate-Preview/nuget/v3/index.json`

## Library usage

The project delivers a programming .NET (C#) library with the support of the
formats. You can use this library to create programs that manipulates the game
assets.

- `SceneGate.Games.ProfessorLayton`

Stable releases are published in nuget.org.

Preview releases can be found in this
[Azure DevOps package repository](https://dev.azure.com/SceneGate/SceneGate/_packaging?_a=feed&feed=SceneGate-Preview).
To use a preview release, create a file nuget.config in the same directory of
your solution (.sln) file with the following content:

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

## License

The software is licensed under the terms of the
[MIT license](https://choosealicense.com/licenses/mit/). The information and
software provided by this repository is only for educational and research
purpose. Please support the original game developers by buying their games.

Icon by Vincent Le Moign,
[CC BY 4.0](https://creativecommons.org/licenses/by/4.0), via
[Wikimedia Commons](https://commons.wikimedia.org/wiki/File:568-teacup-without-handle.svg).
