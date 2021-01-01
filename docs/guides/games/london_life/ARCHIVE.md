# Container format ARCHIVE

Binary format that contains files without name. Files are typically compressed
using Nintendo DS BIOS compression algorithms and formats: _LZSS_, _Huffman_ and
_RLE_.

## Specification

| Offset | Size | Description       |
| ------ | ---- | ----------------- |
| 0x00   | -    | File access table |
| -      | -    | File data         |

The file access table contains an entry per file. Each entry contains two values
of 32-bits (`int32`):

- Absolute offset to the file.
- File size.
