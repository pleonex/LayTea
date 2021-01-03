# Container format ANAG

Binary format that contains files without name or headers.

## Specification

| Offset | Size | Description                           |
| ------ | ---- | ------------------------------------- |
| 0x00   | 0x04 | Stamp `ANAG`                          |
| 0x04   | -    | 32-bits folder info absolute pointers |
| -      | -    | File data                             |

The folder information has the following recursive format:

| Offset | Size | Description                                   |
| ------ | ---- | --------------------------------------------- |
| 0x00   | 4    | Number of children                            |
| 0x04   | -    | 32-bits file or folder info absolute pointers |
| -      | 0x04 | [If last pointer for file] File end position  |

In this case, the pointer could be for a file or for another structure of folder
information. The table ends with a pointer to the file end if the last pointer
was for a file, so it can calculate the file length by subtracting the two.

The game requests files by providing the hierarchy depth and the indexes from
the root folder. The subroutine that gets a file pointer is in the overlay 53 at
`0x020ec210`.
