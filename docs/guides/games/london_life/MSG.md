# Format MSG (MeSsaGe)

Binary format that encode the game text.

## Specification

| Offset | Size | Description                                         |
| ------ | ---- | --------------------------------------------------- |
| 0x00   | 0x04 | Stamp `MSG `                                        |
| 0x04   | 0x04 | Number of scripts                                   |
| 0x08   | -    | 32-bits script pointers, relative to their position |
| -      | -    | Scripts                                             |

### Script encoding

- The text is mixed with formatting and script functions (control codes).
- The encoding is `ISO/IEC 8859-1` (`Latin-1`).
- Script ends with null-terminator character (`\0`).
  - The null-terminator may appear in any place a text character can appear.
    This includes text blocks and remaining bytes from a function argument
    block.
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
- Question options (length is bigger than _4_).

The format of the latter is:

| Offset | Size | Description                                 |
| ------ | ---- | ------------------------------------------- |
| 0x00   | 0x02 | Pre-selected option                         |
| 0x02   | 0x02 | Default option                              |
| 0x04   | 0x04 | Number of strings                           |
| 0x08   | x    | 16-bits string pointers from their position |
| ...    | ...  | For each string...                          |
| 0x00   | 0x04 | String ID (not sequential always)           |
| 0x04   | 0x08 | String length                               |
| 0x08   | x    | String no null-terminated                   |

In the case of one signed 16-bits arguments, after the argument (2 bytes of a
new 4-bytes block) text characters can be stored, including the script
null-terminator.

The known control codes are:

| ID       | Arguments  | Mnemonic    | Description                         |
| -------- | ---------- | ----------- | ----------------------------------- |
| `0x00F0` | None       | `\n`        | New line                            |
| `0x00F1` | None       | `next_box`  | End of dialog, press button `A`     |
| `0x00F2` | `short`    | `color`     | Change text color                   |
| `0x00F3` | `short`    | `wait`      | Wait a specific time                |
| `0x00F4` | `short`    | `name`      | Print a name                        |
| `0x00F5` | `string[]` | `question`  | Select options. Always at the end.  |
| `0x00F9` | `short`    | `variable`  | Print a variable                    |
| `0x00FA` | `short`    | `unk_FA`    | Unknown                             |
| `0x00FB` | None       | `save_load` | Wait for a task (saving or loading) |
| `0x00FC` | None       | `unk_FC`    | Unknown                             |

## Sections

The file contains _11,103_ entries. But internally the game access to the
content by sections. Each section contains a set of entries which may have
different fields. These are the known sections:

| Index | Fields | Description                                               |
| ----- | ------ | --------------------------------------------------------- |
| 0     | 6      | static dialogs ([character dialogs](#character-dialogs))  |
| 714   | 6      | special dialogs                                           |
| 1404  | 6      | special dialogs 2                                         |
| 1482  | 1      | roommate dialogs                                          |
| 1527  | 2      | requests title and description                            |
| 2073  | 1      | weird expressions                                         |
| 2078  | 1      | requests info                                             |
| 2328  | 1      | script dialogs, hard-coded without speaker correlation    |
| 2759  | 3      | items name, description and article                       |
| 4973  | 1      | colors                                                    |
| 4984  | 1      | numbers                                                   |
| 5011  | 1      | style                                                     |
| 5020  | 2      | house items name and description                          |
| 5044  | 1      | furniture style                                           |
| 5058  | 2      | maps title and subtitle                                   |
| 5602  | 2      | places and default dialog                                 |
| 7650  | 1      | reserved names                                            |
| 7659  | 1      | miscellaneous messages, hard-coded                        |
| 8230  | 1      | Keys from keyboard 1                                      |
| 8302  | 1      | Keys from keyboard 2                                      |
| 8602  | 1      | forbidden name regex                                      |
| 8950  | 6      | quest start, accept, cancel, running, 2x achieve messages |
| 10474 | 2      | delivered messages question and dialog on wrong reply     |
| 10554 | 8      | traits and 7 special dialogs                              |
| 10629 | 7      | profession titles                                         |
| 10678 | 1      | crossing and directions                                   |
| 10729 | 4      | train dialogs: buy, thanks and 2x failed messages         |
| 10865 | 2      | characters name and description                           |

### Character dialogs

There are 6 dialogs per character. The order of the character's dialogs match
with the order of the character's names.

It is not possible to get the speaker of script dialogs and other dialogs since
the speaker ID and text index are hard-coded in code for each case.
