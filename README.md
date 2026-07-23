# BuildingQOL

🚧 **Under development** — not yet released, features and keybinds may change.

A tModLoader mod that adds building quality of life features — currently a WorldEdit-style selection and copy/paste tool.

## Features

- Select a rectangular region with two corner keybinds, with a live preview that follows the cursor
- Copy/paste tiles and walls (type, frame, slope, half-block, paint) within the same world, with a paste ghost preview
- Erase all tiles/walls inside a selection
- Optional tile grid overlay for precise alignment
- Mod config for outline color/thickness and auto-reframe on paste

## Default keybinds

Rebindable in-game via Settings > Controls.

| Key | Action |
|-----|--------|
| `[` | Set selection corner 1 |
| `]` | Set selection corner 2 |
| Backspace | Clear selection |
| Delete | Erase tiles/walls in selection |
| `C` | Copy selection |
| `V` | Paste at cursor |
| `G` | Toggle grid overlay |

## Roadmap

1. ~~Selection tool + visual outline~~
2. ~~Copy/paste within the same world, tiles only~~
3. Tile entity support (chests, signs)
4. Undo
5. Save/load to file for cross-world portability
6. Multiplayer sync
