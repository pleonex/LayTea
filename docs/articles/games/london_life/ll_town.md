# Container town

The file is inside the folder `ll/town` and contains images with formats:

- Palette: [ACL](ACL.md)
- Pixel: [ACB and ACG](ACB.md)
- Map: [ASC](ASC.md)
- Sprite: [ACP](ACP.md) and [ACE](ACE.md)
- Animation: [ANM]

## Files

The different parts of an image are split in different groups. These are the
known groups with their file index:

- Image 1
  - Palette: 4524
  - Pixels: 466
  - Sprite: 462
  - Animation: 464
- Image 2
  - Palette: ?
  - Pixels: 2382
  - Sprite: 468
  - Animation: 1425
- Maps
  - For `i` in `[0,52]`
  - Palette: 3339 + (i \* 2) + K, where `K=0` at day and `K=1` at night.
    - Also 3445 + K + Z, for unknown Z and unknown usage.
  - Pixels: 398 + i
  - Maps: (i \* 2) + K, where `K` can be 0 or 1 for the two map layers.
    - Also 106 + i, but what it seems to be the collision map but unknown tiles
      and palette.
- Background menu
  - Palette: 4523
  - Pixels: 451
  - Map: 159
- Minimaps
  - For `i` in `[0,9]`
  - Palette: 4525 + i
  - Pixels: 452 + i
  - Maps: 160 + i
- Room decoration
  - For `i` in `[0,113]`
  - Palette: 4256 + i
  - Pixels: 284 + i
    - It looks like some width values are incorrect here.
  - Maps: 170 + i
- Group 1
  - For `i` in `[0,803]`
  - Palettes: 3450 + i
  - Pixels: 2383 + i
  - Sprites: 469 + i
  - Animations: 1426 + i
- Group 2
  - For `i` in `[0,152]`
  - Palettes: 4370 + i
  - Pixels: 3186 + i
  - Sprites: 1272 + i
  - Animations: 2229 + i
- Unknown palettes: 4254, 4255, 4535
