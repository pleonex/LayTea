# Compression format DENC (D ENCrypt)

Binary format that contains data that can be compressed.

## Specification

| Offset | Size | Description           |
| ------ | ---- | --------------------- |
| 0x00   | 4    | Stamp `DENC`          |
| 0x04   | 4    | Decompressed size     |
| 0x08   | 4    | Compression algorithm |
| 0x0C   | 4    | Compressed size       |
| 0x10   | -    | Data                  |

The possible values for the _compression algorithm_ field are:

- `NULL`: data is **not** compressed.
- `LZSS`: data is compressed with a LZSS variant.

### LZSS compression

The compression algorithm has two mode of operation. The compressed data starts
with a token that contains the operation mode and information. The first bit of
the token decides the operation:

- If the first bit of the token is `0`, copy a sequence of input bytes to the
  output. The token is 8-bits with the following format:

| Bits | Explanation                                  |
| ---- | -------------------------------------------- |
| 0    | `0` for copy operation                       |
| 1-7  | Number of bytes to copy from input to output |

- If the first bit of the token is `1`, copy a sequence of bytes from a previous
  position of the output to the current position. The token is 16-bits (read a
  second byte and combine). The format is:

| Bits  | Explanation                                           |
| ----- | ----------------------------------------------------- |
| 0     | `1` for repeat decompressed data                      |
| 1-11  | Number of bytes to go back in output (max 2048 bytes) |
| 12-15 | Number of bytes to copy (max 16 bytes)                |
