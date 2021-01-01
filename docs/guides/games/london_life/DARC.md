# Container format DARC (D ARChive)

Binary format that contains files without name. Every file is compressed with
the format [`DENC`](DENC.md).

## Specification

| Offset | Size | Description                                       |
| ------ | ---- | ------------------------------------------------- |
| 0x00   | 4    | Stamp `DARC`                                      |
| 0x04   | 4    | Number of files                                   |
| -      | -    | 32-bits file pointers, relative to their position |
| -      | -    | File data                                         |

The first four bytes of the file data is a 32-bits value (`uint`) with the file
size. Then it follows the file data.
