# Professor Layton - London Life

**London Life** is a mini-game for the _Nintendo DS_ (NDS). Developed by
_Level-5_, it was released only for UK as part of the release of _"Professor
Layton and The Last Specter"_. Right now, it is only available in English.

## Game files

The files for this mini-game are inside the folder `/ll`.

### Container and compressed files

- `ll/common/ll_common.darc` contains font, text and audio files
  [[Format DARC]](DARC.md)
- `ll/common/ll_common.darc/fileX.denc` [[Format DENC]](DENC.md)
- `ll/kihira/kihira.archive` contains images [[Format ARCHIVE]](ARCHIVE.md)
- `ll/kihira/kihira.archive/fileX` Nitro BIOS compressed format
- `ll/save/ll_save.darc` contains images [[Format DARC]](DARC.md)
- `ll/town/ll_town.darc` contains images [[Format DARC]](DARC.md)
- `ll/anazawa/anazawa.dat` contains images [[Format ANAG]](ANAG.md)
- `ll/menu/menudata.dat` contains images [[Format ANAG]](ANAG.md)
- `ll/shop/shopdata.dat` contains images [[Format ANAG]](ANAG.md)
- `ll/strage/stragedata.dat` contains images [[Format ANAG]](ANAG.md)

### Text files

- `ll/common/ll_common.darc/file2.denc/file2.msg` [[Format MSG]](MSG.md)

### Font files

Recommended tool to modify `NFTR` font files:
[NerdFontTerminator](https://github.com/pleonex/NerdFontTerminatoR).

- `ll/common/ll_common.darc/file3.denc/file3.nftr` small, size 8 [Format NFTR]
- `ll/common/ll_common.darc/file4.denc/file4.nftr` medium, size 10 [Format NFTR]
- `ll/common/ll_common.darc/file5.denc/file5.nftr` large, size 12 [Format NFTR]
- `ll/common/ll_common.darc/file6.denc/file6.nftr` medium bold, size 14 [Format
  NFTR]
- `ll/common/ll_common.darc/file7.denc/file7.nftr` large bold, size 16 [Format
  NFTR]

### Image files

The image encoding depends on their container.

- [ARCHIVE](ARCHIVE.md):
  - Files:
    - `ll/kihira/kihira.archive`: backgrounds, item and character sprites
  - Formats:
    - Palette: [NCCL]
    - Pixel: [NCCG]
    - Map: [NCSC]
    - Sprite: [NCOB]
- [DARC](DARC.md):
  - Files:
    - `ll/save/ll_save.darc`
    - `ll/town/ll_town.darc`
  - Formats:
    - Palette: [ACL](ACL.md)
    - Pixel: [ACB and ACG](ACB.md)
    - Unknown: `ASC`, `ACE`, `ANM` and `ACP`
- [ANAG](ANAG.md)
  - Files:
    - `ll/anazawa/anazawa.dat`
    - `ll/menu/menudata.dat`
    - `ll/shop/shopdata.dat`
    - `ll/strage/stragedata.dat`
  - Formats:
    - Unknown

### Audio files

Recommended tool to export and import `SAD` files:
[Sadler](https://github.com/pleonex/SADL-Audio-format).

- `ll/common/bgm_common.SWD` [Format SWDL]
- `ll/common/ll_common.darc` files from 8 to 31. Formats: SEDL, SWDL and SMDL.
- `ll/common/stream00_null.SAD` [Format SAD]
- `ll/common/stream01.SAD` [Format SAD]
- `ll/common/stream02.SAD` [Format SAD]

### Unknown files

- `ll/wifi/parts.dat`
- `ll/wifi/wifimenu.dat`
- `ll/wifi/wifimenuobj.dat`
