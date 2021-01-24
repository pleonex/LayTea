# Format NCSC (Nitro C? OBject)

Binary format that contains one or more sprites including its image data.

## Specification

| Offset | Size | Description                          |
| ------ | ---- | ------------------------------------ |
| 0x00   | 0x04 | Stamp `NCOB`                         |
| 0x04   | 0x02 | Endianness marker (must read 0xFEFF) |
| 0x06   | 0x02 | Version                              |
| 0x08   | 0x04 | File size                            |
| 0x0C   | 0x02 | Header size                          |
| 0x0E   | 0x02 | Number of sections                   |
| 0x10   | -    | Sections                             |

### Cell information section

| Offset | Size | Description       |
| ------ | ---- | ----------------- |
| 0x00   | 0x04 | Stamp `CELL`      |
| 0x04   | 0x04 | Section size      |
| 0x08   | 0x04 | Number of sprites |
| 0x0C   | -    | Sprite definition |

Sprite definition:

| Offset | Size | Description    |
| ------ | ---- | -------------- |
| 0x00   | 0x04 | Number of OAMs |
| 0x04   | -    | OAMs           |

OAM definition:

| Offset | Size | Description                  |
| ------ | ---- | ---------------------------- |
| 0x00   | 0x02 | X offset                     |
| 0x02   | 0x02 | Y offset                     |
| 0x04   | 0x02 | Unknown (0 always? padding?) |
| 0x06   | 0x01 | Flip X                       |
| 0x07   | 0x01 | Flip Y                       |
| 0x08   | 0x04 | Unknown (0 always? padding?) |
| 0x0C   | 0x01 | OAM shape                    |
| 0x0D   | 0x01 | OAM size                     |
| 0x0E   | 0x01 | OAM priority                 |
| 0x0F   | 0x01 | Palette index                |
| 0x10   | 0x04 | Tile offset                  |

### Character section

Pixel format is 4bpp in tile layout.

| Offset | Size | Description  |
| ------ | ---- | ------------ |
| 0x00   | 0x04 | Stamp `CHAR` |
| 0x04   | 0x04 | Section size |
| 0x08   | 0x04 | Unknown      |
| 0x0C   | 0x04 | Data size    |
| 0x10   | -    | Image data   |

### Link section

| Offset | Size | Description                                          |
| ------ | ---- | ---------------------------------------------------- |
| 0x00   | 0x04 | Stamp `LINK`                                         |
| 0x04   | 0x04 | Section size                                         |
| 0x08   | -    | Null-terminated string with the pixel info file name |

### CMNT section

| Offset | Size | Description             |
| ------ | ---- | ----------------------- |
| 0x00   | 0x04 | Stamp `CMNT`            |
| 0x04   | 0x04 | Section size            |
| 0x08   | -    | Unknown (4 bytes only?) |

### Other sections to investigate

- `CNUM`
- `GRP `
- `ANIM`
- `ACTL`
- `MODE`
- `LABL`
- `CCMT`
- `ECMT`
- `FCMT`
- `CLBL`
- `EXTR`
