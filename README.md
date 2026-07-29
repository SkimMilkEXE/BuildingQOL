# BuildingQOL

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
- `/blockswap <from> <to>` and `/fill <block>` chat commands to replace or fill tile/wall/liquid types inside the selection
- Terrain Wand item (craft from 10 Wood at a Work Bench, or spawn with one automatically) for sculpting terrain around the cursor: Raise, Lower, Smooth, Roughen, cycled with a keybind

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
| `M` | Cycle Terrain Wand mode (Raise → Lower → Smooth → Roughen) |

## Commands

| Command | Action |
|---------|--------|
| `/fill <block>` | Fill the entire current selection with one tile, wall, or liquid type. |
| `/floodfill <water\|lava\|honey\|shimmer>` | Fill only the open space in the selection with liquid, then let it settle naturally instead of staying a static block. |
| `/blockswap <from> <to>` | Replace one tile, wall, or liquid type with another inside the current selection. Uses exact internal IDs (e.g. `WoodBlock`, not `Wood`), or `water`/`lava`/`honey`/`shimmer` for liquids. |
| `/drain` | Remove all liquid in the current selection, leaving tiles/walls untouched. |
| `/clear` | Erase tiles, walls, and liquid in the current selection (Erase + Drain combined). Undoes in two steps, not one. |
| `/tilename` | Reports the tile/wall/liquid internal ID names under your cursor, for use with `/blockswap` and `/fill`. |
| `/qolhelp` | Lists all BuildingQOL commands and what they do. |

## Terrain Wand

A tool item, not tied to the `[`/`]` selection — new characters spawn with one, and it can be crafted from 10 Wood at a Work Bench if lost. Left-click (hold to auto-repeat) applies the current mode to a strip of columns centered on your cursor, using the topmost solid tile of each column as the surface:

- **Raise** — extends the surface upward, using each column's own surface tile so it blends in.
- **Lower** — removes the top of the surface (via the same safe tile-removal path as Erase).
- **Smooth** — nudges every column one tile toward the brush's average height.
- **Roughen** — nudges every column randomly up, down, or unchanged.

`M` cycles through the four modes; brush radius and step size are adjustable in the mod config (`TerrainBrushRadius`, `TerrainStepAmount`). Undo/redo and multiplayer sync work the same as everything else.
