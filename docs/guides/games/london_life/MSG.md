# MSG format

## Specification

| Offset | Size | Description                                        |
| ------ | ---- | -------------------------------------------------- |
| 0x00   | 0x04 | ID: `MSG `                                         |
| 0x04   | 0x04 | Number of entries                                  |
| 0x08   | x    | 32-bits script pointers relative to their position |
| x      | x    | Scripts                                            |

### Script encoding

- The text is mixed with formatting and script functions (control codes).
- The encoding is `ISO/IEC 8859-1` (`Latin-1`).
- Script ends with null-terminator character (`\0`).
- Padding to multiple of 4 with byte `0x00`.

The script consists in blocks of 4 bytes. A block may contain text _or_ control
codes. When necessary, it uses padding with the byte `0xFF` to fill until the
next block. If the first byte of the block is in the range `[0xF0, 0xFE]`, then
the following bytes are for a control code (it could take more than a block).
Otherwise, it is a text block.

#### Text blocks

Text code-points are `Latin-1`, so they are always 1-byte. If the first
code-point is in the range `[0xF0, 0xFE]`, escape it with a first byte `0xFF` so
it is not detected as a control code block.

#### Control code blocks

Control code blocks have two 16-bits values:

- ID which identifies the function or formatting code.
- Length of the control code, including the two bytes of the length value.

There are three type of arguments:

- No arguments.
- One signed 16-bits.
- A list of strings.

The format of the latter is:

| Offset | Size | Description                                 |
| ------ | ---- | ------------------------------------------- |
| 0x00   | 0x02 | Unknown                                     |
| 0x02   | 0x02 | Unknown                                     |
| 0x04   | 0x04 | Number of strings                           |
| 0x08   | x    | 16-bits string pointers from their position |
| ...    | ...  | For each string...                          |
| 0x00   | 0x04 | String ID                                   |
| 0x04   | 0x08 | String length                               |
| 0x08   | x    | String no null-terminated                   |

The known control codes are:

| ID       | Argument type | Mnemonic    | Description                                    |
| -------- | ------------- | ----------- | ---------------------------------------------- |
| `0x00F0` | None          | `\n`        | New line                                       |
| `0x00F1` | None          | split entry | End of dialog, user must press a button        |
| `0x00F2` | 16-bits       | `color`     | Change text color                              |
| `0x00F3` | 16-bits       | `wait`      | Wait a specific time                           |
| `0x00F4` | 16-bits       | `name`      | Print a name                                   |
| `0x00F5` | Strings list  | `question`  | Ask a question                                 |
| `0x00F9` | 16-bits       | `variable`  | Print a variable                               |
| `0x00FA` | 16-bits       | `unk_FA`    | Unknown                                        |
| `0x00FB` | None          | `bg_work`   | Wait for a background task (saving or loading) |
| `0x00FC` | None          | `unk_FC`    | Unknown                                        |
