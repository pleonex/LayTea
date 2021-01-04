# Image format ACS

Binary format that contains background map tile information.

## Specification

| Offset | Size | Description                          |
| ------ | ---- | ------------------------------------ |
| 0x00   | 4    | Stamp `ASC `                         |
| 0x04   | 2    | Sub-image tile data?                 |
| 0x06   | 2    | ?                                    |
| 0x08   | 4    | ?                                    |
| 0x0C   | 4    | Map data size                        |
| 0x10   | -    | Standard NDS 16-bits map information |

The subroutine that copies the map is in overlay 62 at `0x020ec9fc`. It does:
`0x10 + (id1 / 4) * ([0x04] / 8) + (id0 / 4)`
