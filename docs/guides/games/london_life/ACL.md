# Format ACL (A? CoLor)

Binary format that contains palette colors.

## Specification

| Offset | Size | Description                                         |
| ------ | ---- | --------------------------------------------------- |
| 0x00   | 0x04 | Stamp `ACL `                                        |
| 0x04   | 0x04 | Number of palettes                                  |
| 0x08   | -    | 16-bits palette pointers relative to their position |
| -      | -    | Palette                                             |

Each palette starts with a 32-bits value indicating the number of colors. The
encoding for the colors is standard `BGR555`.
