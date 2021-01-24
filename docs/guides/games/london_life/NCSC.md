# Format NCSC (Nitro C? SCreen)

Binary format that contains background map tile information.

## Specification

| Offset | Size | Description                          |
| ------ | ---- | ------------------------------------ |
| 0x00   | 0x04 | Stamp `NCSC`                         |
| 0x04   | 0x02 | Endianness marker (must read 0xFEFF) |
| 0x06   | 0x02 | Version                              |
| 0x08   | 0x04 | File size                            |
| 0x0C   | 0x02 | Header size                          |
| 0x0E   | 0x02 | Number of sections                   |
| 0x10   | -    | Sections                             |

### Pixel information section

| Offset | Size | Description                                    |
| ------ | ---- | ---------------------------------------------- |
| 0x00   | 0x04 | Stamp `SCRN`                                   |
| 0x04   | 0x04 | Section size                                   |
| 0x08   | 0x04 | Width in tiles                                 |
| 0x0C   | 0x04 | Height in tiles                                |
| 0x10   | 0x04 | Unknown                                        |
| 0x14   | 0x04 | Unknown                                        |
| 0x18   | -    | Map information in standard NDS 16-bits values |

### ESCR section

| Offset | Size | Description                       |
| ------ | ---- | --------------------------------- |
| 0x00   | 0x04 | Stamp `ESCR` (_Extended SCReen?_) |
| 0x04   | 0x04 | Section size                      |
| 0x08   | 0x04 | Width in tiles                    |
| 0x0C   | 0x04 | Height in tiles                   |
| 0x10   | 0x04 | Unknown                           |
| 0x14   | 0x04 | Unknown                           |
| 0x18   | -    | Unknown 32-bits value per tile    |

### CLRF section

| Offset | Size | Description     |
| ------ | ---- | --------------- |
| 0x00   | 0x04 | Stamp `CLRF`    |
| 0x04   | 0x04 | Section size    |
| 0x08   | 0x04 | Width in tiles  |
| 0x0C   | 0x04 | Height in tiles |
| 0x10   | -    | Unknown         |

### CLRC section

| Offset | Size | Description  |
| ------ | ---- | ------------ |
| 0x00   | 0x04 | Stamp `CLRC` |
| 0x04   | 0x04 | Section size |
| 0x08   | -    | Unknown      |

### GRID section

| Offset | Size | Description  |
| ------ | ---- | ------------ |
| 0x00   | 0x04 | Stamp `GRID` |
| 0x04   | 0x04 | Section size |
| 0x08   | -    | Unknown      |

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
