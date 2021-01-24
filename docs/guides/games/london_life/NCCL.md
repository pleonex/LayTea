# Format NCCL (Nitro C? CoLor)

Binary format that contains palette colors.

## Specification

| Offset | Size | Description                          |
| ------ | ---- | ------------------------------------ |
| 0x00   | 0x04 | Stamp `NCCL`                         |
| 0x04   | 0x02 | Endianness marker (must read 0xFEFF) |
| 0x06   | 0x02 | Version                              |
| 0x08   | 0x04 | File size                            |
| 0x0C   | 0x02 | Header size                          |
| 0x0E   | 0x02 | Number of sections                   |
| 0x10   | -    | Sections                             |

### Palette section

| Offset | Size | Description                  |
| ------ | ---- | ---------------------------- |
| 0x00   | 0x04 | Stamp `PALT`                 |
| 0x04   | 0x04 | Section size                 |
| 0x08   | 0x04 | Number of colors per palette |
| 0x0C   | 0x04 | Number of palettes           |
| 0x10   | -    | Colors in `BGR555` format    |

### CMNT section

| Offset | Size | Description             |
| ------ | ---- | ----------------------- |
| 0x00   | 0x04 | Stamp `CMNT`            |
| 0x04   | 0x04 | Section size            |
| 0x08   | -    | Unknown (4 bytes only?) |
