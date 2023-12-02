# Format NCCG (Nitro C? Character backGround)

Binary format that contains indexed pixel information for one image.

## Specification

| Offset | Size | Description                          |
| ------ | ---- | ------------------------------------ |
| 0x00   | 0x04 | Stamp `NCCG`                         |
| 0x04   | 0x02 | Endianness marker (must read 0xFEFF) |
| 0x06   | 0x02 | Version                              |
| 0x08   | 0x04 | File size                            |
| 0x0C   | 0x02 | Header size                          |
| 0x0E   | 0x02 | Number of sections                   |
| 0x10   | -    | Sections                             |

### Pixel information section

Pixel layout in tiles of 8x8 pixels size.

| Offset | Size | Description                          |
| ------ | ---- | ------------------------------------ |
| 0x00   | 0x04 | Stamp `CHAR`                         |
| 0x04   | 0x04 | Section size                         |
| 0x08   | 0x04 | Width in tiles                       |
| 0x0C   | 0x04 | Height in tiles                      |
| 0x10   | 0x04 | Pixel format: 0=4bpp, otherwise 8bpp |
| 0x14   | -    | Pixel information                    |

### Attribute section

| Offset | Size | Description  |
| ------ | ---- | ------------ |
| 0x00   | 0x04 | Stamp `ATTR` |
| 0x04   | 0x04 | Section size |
| 0x08   | 0x04 | Width        |
| 0x0C   | 0x04 | Height       |
| 0x10   | -    | Unknown      |

### Link section

| Offset | Size | Description                                       |
| ------ | ---- | ------------------------------------------------- |
| 0x00   | 0x04 | Stamp `LINK`                                      |
| 0x04   | 0x04 | Section size                                      |
| 0x08   | -    | Null-terminated string with the palette file name |

### CMNT section

| Offset | Size | Description             |
| ------ | ---- | ----------------------- |
| 0x00   | 0x04 | Stamp `CMNT`            |
| 0x04   | 0x04 | Section size            |
| 0x08   | -    | Unknown (4 bytes only?) |
