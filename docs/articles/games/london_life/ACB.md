# Formats ACB (A? Character B?) and ACG (A? Character backGround)

Binary format that contains indexed pixel information for one or more images.

## Specification

| Offset | Size | Description                                       |
| ------ | ---- | ------------------------------------------------- |
| 0x00   | 0x04 | Stamp `ACB ` or `ACG `                            |
| 0x04   | 0x04 | Number of images                                  |
| 0x08   | -    | 32-bits image pointers relative to their position |
| -      | -    | Images                                            |

For each image:

| Offset | Size | Description                    |
| ------ | ---- | ------------------------------ |
| 0x00   | 4    | Flags (see below)              |
| 0x04   | 4    | Image size                     |
| 0x08   | -    | Indexed pixel information 4bpp |

Flags:

| Bits  | Description                                |
| ----- | ------------------------------------------ |
| 0-10  | Width                                      |
| 11-21 | Height                                     |
| 22-30 | Number of colors <=0x10: 4bpp, >0x10: 8bpp |
| 31    | ? Maybe if its tiled or not                |

Images have several tiles of standard 8x8 pixel size.
