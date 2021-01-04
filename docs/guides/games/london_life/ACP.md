# Format ACP (A? Character P?)

Binary format that contains unknown data related to images.

## Specification

| Offset | Size | Description                                 |
| ------ | ---- | ------------------------------------------- |
| 0x00   | 0x04 | Stamp `ACP `                                |
| 0x04   | 0x04 | Number of sections                          |
| 0x08   | -    | 16-bits pointers relative to their position |
| -      | -    | Sprites definition                          |

Sprite format:

| Offset | Size | Description                           |
| ------ | ---- | ------------------------------------- |
| 0x00   | 0x01 | Number of segments (8 bytes each one) |
| 0x01   | 0x01 | ?                                     |
| 0x02   | 0x02 | ?                                     |

Segment value 1:

| Bits | Description |
| ---- | ----------- |
| 0-3  | ?           |
| 4-7  | ?           |
| 8-31 |             |

Segment value 2:

| Bits  | Description |
| ----- | ----------- |
| 0-8   |             |
| 9-12  | ?           |
| 13-16 | ?           |
| 17-26 | ?           |
| 27-31 |             |

The index of the sprite to load is in the `ANM` file.

The OAM are processed in the subroutine `0x020d27a0` from overlay 48.
