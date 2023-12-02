# Format DARC (D? ARChive)

Binary format that contains files without name. Every file is compressed with
the format [`DENC`](DENC.md).

## Specification

| Offset | Size | Description                                       |
| ------ | ---- | ------------------------------------------------- |
| 0x00   | 4    | Stamp `DARC`                                      |
| 0x04   | 4    | Number of files                                   |
| -      | -    | 32-bits file pointers, relative to their position |
| -      | -    | File data                                         |

The first four bytes of the file data is a 32-bits value (`uint`) with the file
size. Then it follows the file data.

## Subroutines

The following subroutines get a file inside the `DARC` container, decompressing
the `DENC` data. The access is via index in `R1`. The main subroutine that gets
any file is at `0x020D527C`. Then, there is a wrapper per file type:

- `darc_getAsc`: 0x020D5088
- `darc_getAcg`: 0x020D50AC
- `darc_getAcl`: 0x020D50D0
- `darc_getAce`: 0x020D50F4
- `darc_getAnm`: 0x020D5118
- `darc_getAcp`: 0x020D513C
- `darc_getAcb`: 0x020D5160
- `darc_getNftr`: 0x020D5184
- `darc_getMsg`: 0x020D51A8
- `darc_getSedl`: 0x020D51CC
- `darc_getSmdl`: 0x020D51F8
- `darc_getSwdl`: 0x020D5224
- `darc_getUnk8`: 0x020D5250
  - This is the file 8 from `ll_common.darc` that has size 0.
