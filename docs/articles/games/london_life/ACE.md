# Format ACE (A? Character E?)

Binary format that contains unknown data related to images.

## Specification

| Offset | Size | Description                                 |
| ------ | ---- | ------------------------------------------- |
| 0x00   | 0x04 | Stamp `ACE `                                |
| 0x04   | 0x04 | Number of sections                          |
| 0x08   | -    | 16-bits pointers relative to their position |
| -      | -    | Sprites definition                          |

TODO: sprite definition similar but not equal to [ACP](ACP.md).
