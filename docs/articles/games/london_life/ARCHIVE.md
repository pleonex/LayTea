# Format ARCHIVE

Binary format that contains files without name. Files are typically compressed
using Nintendo DS BIOS compression algorithm and format for _LZSS_.

## Specification

| Offset | Size | Description       |
| ------ | ---- | ----------------- |
| 0x00   | -    | File access table |
| -      | -    | File data         |

The file access table contains an entry per file. Each entry contains two values
of 32-bits (`int32`):

- Absolute offset to the file.
- File size.
