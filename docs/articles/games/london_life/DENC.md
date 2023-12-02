# Format DENC (D? ENCrypt)

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

- `\0`: not implemented in this game but it is detected as a valid case.
- [`NULL`](#null-compression): data is not compressed.
- [`RLE `](#rle-compression): data is compressed with RLE variant.
- [`LZSS`](#lzss-compression): data is compressed with a LZSS variant.
- [`BPE `](#bpe-compression): Not implemented in this game.
- [`ZSKP`](#zskp-compression): data is compressed with ZSKP.
- [`LSKP`](#lskp-compression): data is compressed with LSKP.
- [`RNGE`](#rnge-compression): data is compressed with RNGE.
- [`SOA `](#soa-compression): Not implemented in this game.

### Null compression

Data is not compressed. The processing subroutine in overlay 48 at `0x020d314c`.

### RLE compression

TODO: Analyze code. The decompression subroutine is in overlay 48 at
`0x020d320c`.

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
| 12-15 | Number of bytes to copy starting with 2 (2-18 bytes)  |

The decompression subroutine is in overlay 48 at `0x020d3330`.

### BPE compression

The game _Layton London Life_ does not implement this compression.

### ZSKP compression

TODO: Analyze code. The decompression subroutine is in overlay 48 at
`0x020d3580`.

### LSKP compression

TODO: Analyze code. The decompression subroutine is in overlay 48 at
`0x020d366c`.

### RNGE compression

TODO: Analyze code. The decompression subroutine is in overlay 48 at
`0x020d3744`.

### SOA compression

The game _Layton London Life_ does not implement this compression.
