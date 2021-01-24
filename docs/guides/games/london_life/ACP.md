# Format ACP (A? Character P?)

Binary format that contains the definition of one or more sprites.

## Specification

| Offset | Size | Description                                 |
| ------ | ---- | ------------------------------------------- |
| 0x00   | 0x04 | Stamp `ACP `                                |
| 0x04   | 0x04 | Number of sections                          |
| 0x08   | -    | 16-bits pointers relative to their position |
| -      | -    | Sprites definition                          |

### Sprite

| Bits  | Format | Description                               |
| ----- | ------ | ----------------------------------------- |
| 0-7   | int    | Number of segments                        |
| 8-13  | float9 | Position X start (by 6.0 only fractional) |
| 14-18 | -      | Not used                                  |
| 19-24 | float9 | Position X end (by 6.0 only fractional)   |
| 25-29 | float9 | Position Z (by 6.0 only fractional)       |
| 30-31 | -      | Not used                                  |
| -     | -      | Segments                                  |

For each segment, there are two values of 32-bits (or one of 64-bits):

Value 1:

| Bits  | Format | Description                |
| ----- | ------ | -------------------------- |
| 0-3   | int    | Left S coordinate in tiles |
| 4-7   | int    | Top T coordinate int tiles |
| 8-16  | float6 | Coordinate X               |
| 17-25 | float6 | Coordinate Y               |
| 26-31 | int    | Texture format from table  |

Value 2:

| Bits  | Format | Description                    |
| ----- | ------ | ------------------------------ |
| 0-8   | float9 | Coordinate Z (only fractional) |
| 9-12  | int    | Right S coordinate in tiles    |
| 13-16 | int    | Bottom T coordinate in tiles   |
| 17-26 | -      | Not used                       |
| 27-31 | int    | Palette index                  |

### Notes

- `float6` is a decimal format unsigned, 3-bits integer part, 6-bits fractional
  part.
- `float9` is a decimal format unsigned with only 9-bits of fractional part.
- The texture size comes from tables using the largest start or end coordinate.
  - T-size table is at `0x020DBFBC`
  - S-size table is at `0x020DBF74`
- The texture format table is at `0x020DBF98`.
- The color 0 of the palette is always transparent.
- The sprites are converted into 3D commands in the subroutine `0x020D27A0` from
  overlay 48.
- The sprite to load is defined in the `ANM` file.
