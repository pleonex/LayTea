# Text format MSG (MeSsaGe)

Binary format that encode the game text.

## Specification

| Offset | Size | Description                                         |
| ------ | ---- | --------------------------------------------------- |
| 0x00   | 0x04 | ID: `MSG `                                          |
| 0x04   | 0x04 | Number of scripts                                   |
| 0x08   | -    | 32-bits script pointers, relative to their position |
| -      | -    | Scripts                                             |

### Script encoding

- The text is mixed with formatting and script functions (control codes).
- The encoding is `ISO/IEC 8859-1` (`Latin-1`).
- Script ends with null-terminator character (`\0`).
- Padding to multiple of 4 with byte `0x00`.

The script consists of blocks of 4 bytes. A block contains text _or_ control
codes. Control code blocks start with a byte in the range `[0xF0, 0xFE]`. The
byte `0xFF` serves as block padding.

#### Text blocks

Text code-points are `Latin-1`, so they are always 1 byte. If the first
code-point of a block is in the range `[0xF0, 0xFE]`, it must be escaped with
the byte `0xFF`, so it is not detected as a control code block.

#### Control code blocks

Control code blocks have two 16-bits values:

- ID which identifies the function or formatting code.
- Length of the control code, including the two bytes of the length value.

There are three type of arguments:

- No arguments (length is _2_).
- One signed 16-bits (length is _4_).
- A list of strings (length is bigger than _4_).

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

| ID       | Arguments  | Mnemonic    | Description                         |
| -------- | ---------- | ----------- | ----------------------------------- |
| `0x00F0` | None       | `\n`        | New line                            |
| `0x00F1` | None       | split entry | End of dialog, press button `A`     |
| `0x00F2` | `short`    | `color`     | Change text color                   |
| `0x00F3` | `short`    | `wait`      | Wait a specific time                |
| `0x00F4` | `short`    | `name`      | Print a name                        |
| `0x00F5` | `string[]` | `question`  | Ask a question                      |
| `0x00F9` | `short`    | `variable`  | Print a variable                    |
| `0x00FA` | `short`    | `unk_FA`    | Unknown                             |
| `0x00FB` | None       | `bg_work`   | Wait for a task (saving or loading) |
| `0x00FC` | None       | `unk_FC`    | Unknown                             |
