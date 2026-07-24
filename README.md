# BuildingQOL

🚧 **Under development** — not yet released, features and keybinds may change.

A tModLoader mod that adds building quality of life features — currently a WorldEdit-style selection and copy/paste tool.

## Features

- Select a rectangular region with two corner keybinds, with a live preview that follows the cursor
- Copy/paste tiles and walls (type, frame, slope, half-block, paint) within the same world, with a paste ghost preview
- Copy/paste chest contents, sign text, and other tile entities (item frames, weapon racks, display dolls, hat racks, food platters, logic sensors, training dummies, pylons) along with their tiles
- Erase all tiles/walls inside a selection
- Undo/redo for paste and erase, up to 50 actions back
- Save/load the clipboard to a file, so a schematic can be carried into a different world
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
| `Z` | Undo |
| `Y` | Redo |
| `S` | Save schematic to file |
| `L` | Load schematic from file |
| `H` | Toggle cursor tile highlight |

## Roadmap

1. ~~Selection tool + visual outline~~
2. ~~Copy/paste within the same world, tiles only~~
3. ~~Tile entity support (chests, signs)~~
4. ~~Undo/Redo~~
5. ~~Save/load to file for cross-world portability~~
6. Multiplayer sync
