# Image format ACB and ACG

Binary format that contains indexed pixel information for one or more images.

## Specification

| Offset | Size | Description                                       |
| ------ | ---- | ------------------------------------------------- |
| 0x00   | 0x04 | Stamp `ACB ` or `ACG`                             |
| 0x04   | 0x04 | Number of images                                  |
| 0x08   | -    | 32-bits image pointers relative to their position |
| -      | -    | Images                                            |

For each image:

| Offset | Size | Description                    |
| ------ | ---- | ------------------------------ |
| 0x00   | 1    | ?                              |
| 0x01   | 2    | ?                              |
| 0x03   | 1    | ?                              |
| 0x04   | 4    | ?                              |
| 0x08   | -    | Indexed pixel information 4bpp |

Images have several tiles of standard 8x8 pixel size.
