# _LayTea_ [![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://choosealicense.com/licenses/mit/) ![Build and release](https://github.com/pleonex/LayTea/workflows/Build%20and%20release/badge.svg)

**Work-in-progress** modding tools for games of the _"Professor Layton"_.

Library is available from the
[GitHub packages](https://github.com/users/pleonex/packages?repo_name=LayTea).
Applications are attach in each
[GitHub release](https://github.com/pleonex/LayTea/releases).

<!-- prettier-ignore -->
| Release | Package |
| ------- | ------- |
| Stable  | ![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/pleonex/LayTea?sort=semver) |
| Preview | ![GitHub commits since latest release (by SemVer)](https://img.shields.io/github/commits-since/pleonex/LayTea/latest?sort=semver) |

## Features

Games:

- Documentation for
  [Professor Layton - London Life](https://www.pleonex.dev/LayTea/guides/games/london_life/summary.html)

Formats:

- Professor Layton - London Life (US only):
  - **`DARC` containers**: read and write.
  - **`DENC` containers**: read and write.
  - **`LZSS` compression**: decompress and compress.
  - **`MSG` texts**: read and write.
    - PO export and import.

## Documentation

Feel free to ask any question in the
[project Discussion site!](https://github.com/pleonex/LayTea/discussions)

Check our on-line [documentation](https://pleonex.dev/LayTea) for more
information about the file formats and how to use the tools.

## Build

The project requires to build .NET 5.0 SDK and .NET Core 3.1 runtime. If you
open the project with VS Code and you did install the
[VS Code Remote Containers](https://code.visualstudio.com/docs/remote/containers)
extension, you can have an already pre-configured development environment with
Docker or Podman.

To build, test and generate artifacts run:

```sh
# Only required the first time
dotnet tool restore

# Default target is Stage-Artifacts
dotnet cake
```

To just build and test quickly, run:

```sh
dotnet cake --target=BuildTest
```

## License

The software is licensed under the terms of the
[MIT license](https://choosealicense.com/licenses/mit/). The information and
software provided by this repository is only for educational and research
purpose. Please support the original game developers by buying their games.

Icon by Vincent Le Moign,
[CC BY 4.0](https://creativecommons.org/licenses/by/4.0), via
[Wikimedia Commons](https://commons.wikimedia.org/wiki/File:568-teacup-without-handle.svg).
