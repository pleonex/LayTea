# Container save

The file is inside the folder `ll/save` and contains images with formats:

- Palette: [ACL](ACL.md)
- Pixel: [ACB and ACG](ACB.md)
- Map: [ASC](ASC.md)
- Sprite: [ACP](ACP.md) and [ACE](ACE.md)
- Animation: [ANM]

## Files

The different parts of an image are split in different groups. These are the
known groups with their file index:

- Background 0
  - Palette: 1678
  - Pixels: 2
  - Map: 0
- Background 1
  - Palette: 1679
  - Pixels: 3
  - Map: 1
- Group 1
  - For `i` in `[0, ?]`
  - Palette: 1258 + i, also 1674, 1675 and 1676
  - Pixels: 842 + i
  - Sprite: 10
  - Animation: 426 + i
