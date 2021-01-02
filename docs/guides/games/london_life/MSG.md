# Text format MSG (MeSsaGe)

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
| 0x00   | 0x02 | Pre-selected option                         |
| 0x02   | 0x02 | Default option                              |
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

## Sections

The file contains _11,103_ entries. But internally the game access to the
content by sections. These are the known sections:

- `[0, 1526]`: default character dialogs, 6 entries per character (see
  [character dialogs](#character-dialogs)).
- `[1527,2072]`: requests, 2 entries per value: title and content.
- `[2073,2077]`: weird expressions.
- `[2078,2327]`: request info.
- `[2328,2758]`: script dialogs, hard-coded without speaker correlation.
- `[2759,4972]`: items, 3 entries per object: name, description, article.
- `[4973,4983]`: colors.
- `[4984,5010]`: numbers.
- `[5011,5019]`: style.
- `[5020,5043]`: house items, 2 entries per object: name and description.
- `[5044,5057]`: furniture style.
- `[5058,5601]`: map, 2 entries: title and subtitle.
- `[5602,7649]`: locations, 2 entries: location, first dialog
- `[7650,7658]`: reserved names.
- `[7659,8229]`: miscellaneous, hard-coded.
- `[8230,8301]`: second keyboard, first placeholders and then overwrite.
- `[8302,8601]`: keyboard, first placeholders and then overwrite.
- `[8602,8949]`: ban names regex
- `[8950,10473]`: quests, 6 entries per quest: start, accept, cancel, running,
  achieve, reserved.
- `[10474,10553]`: delivered messages, 2 entries: question and wrong reply.
- `[10554,10628]`: trait, 8 entries per value: trait, 3 special dialogs,
  birthday appeal, birthday blessing, special message, reserved.
- `[10629,10677]`: profession titles, 7 entries per value
- `[10678,10728]`: crossing and directions
- `[10729,10848]`: train messages, 4 entries: buy, thanks, failed, wait
- `[10849,11102]`: character names and descriptions. 2 entries per character:
  name and description.

### Character dialogs

The information to correlate the speaker name with their dialog is in the
`overlay48`. Each speaker has an ID that goes from 0 to 255. For each speaker,
there are 6 dialogs, divided en 2 groups according to the current event. At
`0x020E3D48` there is a table that gives the address to the information entry
for the speaker. Each entry of this main table is 12 bytes and has the following
format:

| Offset | Size | Description        |
| ------ | ---- | ------------------ |
| 0x00   | 1    | Speaker minimum ID |
| 0x01   | 1    | Speaker maximum ID |
| 0x02   | 2    | Reserved           |
| 0x04   | 4    | Base address       |
| 0x08   | 4    | Entry size         |

To calculate the address to the speaker information given its ID, iterate the
table finding an entry where the ID is in the range minimum and maximum ID.
Then, calculate the relative ID from the minimum value, multiply by the entry
size and add the base address. The size of the character info changes depending
the speaker category, but the second byte gives always the internal ID. This
second ID is also the index to the speaker dialog texts.

There is a second table at `0x020DC03C` with more information about the speaker.
To access, use the internal ID, each entry has a constant size of 12 bytes. This
information is divided in 2 groups of 3 values of 16-bits.

Internally, there aren't scripts. The game iterates over all characters,
deciding if they appear in the current map or not. This happens in the
subroutine `0x020EC6C4`. If they happen, then the game creates a new dialog
event assigning the speaker ID and the text index.
