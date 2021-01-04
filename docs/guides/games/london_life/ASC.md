# Format ASC (A? SCreen)

Binary format that contains background map tile information.

## Specification

| Offset | Size | Description                          |
| ------ | ---- | ------------------------------------ |
| 0x00   | 4    | Stamp `ASC `                         |
| 0x04   | 2    | Width                                |
| 0x06   | 2    | Height                               |
| 0x08   | 2    | Number of maps                       |
| 0x0A   | 2    | Map data offset                      |
| 0x0C   | 4    | Map data size                        |
| 0x10   | -    | Standard NDS 16-bits map information |

It is not possible to confirm fields `0x08` and `0x0A` as the game doesn't read
them. The offset to the map size is hard-coded in other code locations as
`0x10`. The content of these fields seem to be always the same `1` and `16`.

The subroutine that copies the map is in overlay 62 at `0x020ec9fc`. It gets the
map info given a pixel coordinate (x, y): it converts the pixel coordinate to
tile coordinate (tile size 8x8) and multiply by the map info size (2)
`0x10 + (y / 8) * (width / 8) * 2 + (id0 / 8) * 2`
