<p align="center">
  <img src="icon.png" alt="BuildingQOL icon" width="96">
</p>

<h1 align="center">Building Quality of Life</h1>

<p align="center">
  WorldEdit-style building tools for Terraria: select, copy, flip, paste, save schematics and reshape builds with chat commands.
</p>

<p align="center">
  <img src="docs/hero.png" alt="A house being pasted with the see-through paste preview lined up next to the original">
</p>

## Features

- **Selection:** mark two corners with `[` and `]`. A live outline, corner markers and a size label (like `12 x 8`) follow your cursor while you pick the second corner.
- **Copy and paste:** copies tiles, walls, slopes, half blocks and paint, plus chest names, sign text, item frames, weapon racks, mannequins and other tile entities. Chest items are left out so nothing can be duplicated.
- **Paste preview:** a see-through copy of the actual blocks, furniture and walls shows exactly where a paste will land before you press **V**.
- **Flip:** mirror the clipboard horizontally (**X**) or vertically (`/flip v`). Beds, tables and other furniture stay in one piece.
- **Named schematics:** `/schem save castle` stores a build in a file you can load into any world.
- **Undo and redo:** **Z** and **Y**, up to 50 actions back, for pastes, erases and every command.
- **Shape and fill commands:** `/fill`, `/outline`, `/hollow`, `/blockswap`, `/floodfill`, `/drain` and `/clear` work on the whole selection at once.
- **Terrain Wand:** sculpt hills and valleys around your cursor with Raise, Lower, Smooth and Roughen modes.
- **Grid and cursor highlight:** optional tile grid overlay and cursor highlight for precise placement.
- **Multiplayer:** changes sync to the other players in the world.

<p align="center">
  <img src="docs/selection.png" alt="A selection outline with corner markers and a 12 x 8 size label above it" width="49%">
  <img src="docs/outline-hollow.png" alt="A brick room frame made with /outline, then hollowed out with /hollow" width="49%">
</p>

<p align="center">
  <img src="docs/terrain-wand.png" alt="The Terrain Wand smoothing a bumpy hill">
</p>

## Install

1. Open tModLoader and go to **Workshop → Download Mods**.
2. Search for **Building Quality of Life Improvements** and click **Download**.
3. Enable it in **Workshop → Manage Mods**, then click **Reload Mods**.

You need Terraria and tModLoader on Steam (tModLoader is free if you own Terraria). Nothing else needs installing.

Settings are in **Settings → Mod Configuration → Building QOL** and saved to `Documents\My Games\Terraria\tModLoader\ModConfigs\BuildingQOL_BuildingQOLConfig.json`. Schematics are saved to `Documents\My Games\Terraria\tModLoader\Schematics\BuildingQOL`.

## Keybinds

All of these can be rebound in **Settings → Controls**.

| Key | Action |
|-----|--------|
| `[` / `]` | Set selection corner 1 / corner 2 |
| Backspace | Clear selection |
| Delete | Erase tiles and walls in selection |
| `C` | Copy selection |
| `V` | Paste at cursor |
| `X` | Flip clipboard horizontally |
| `Z` / `Y` | Undo / Redo |
| `S` / `L` | Save / load the default schematic slot |
| `G` | Toggle grid overlay |
| `H` | Toggle cursor highlight |
| `M` | Cycle Terrain Wand mode (Raise → Lower → Smooth → Roughen) |

## Commands

Type these in chat. `/qolhelp` lists them in-game.

| Command | What it does |
|---------|--------------|
| `/fill <block>` | Fill the selection with one tile, wall or liquid, e.g. `/fill StoneBlock` or `/fill lava`. |
| `/outline <block>` | Place a 1-tile border around the edge of the selection, e.g. `/outline GrayBrick` for a room frame. |
| `/hollow` | Remove the blocks inside the selection, keeping a 1-tile shell. Furniture, torches and walls are kept. |
| `/blockswap <from> <to>` | Replace one tile, wall or liquid with another, e.g. `/blockswap WoodBlock RichMahogany`. |
| `/floodfill <water\|lava\|honey\|shimmer>` | Fill only the open space in the selection with liquid, then let it settle naturally. |
| `/drain` | Remove all liquid in the selection, leaving tiles and walls alone. |
| `/clear` | Erase tiles, walls and liquid in the selection. |
| `/flip <h\|v>` | Flip the clipboard horizontally or vertically. |
| `/schem save\|load\|delete <name>` | Save the clipboard as a named schematic, load one back, or delete one. |
| `/schem list` | List your saved schematics. |
| `/tilename` | Show the internal names of the tile, wall and liquid under your cursor. |
| `/qolhelp` | List all commands. |

## How do I use the Terrain Wand?

New characters spawn with one, and you can craft another from 10 Wood at a Work Bench. Left-click (hold to repeat) to shape a strip of ground centered on your cursor. **M** switches between modes:

- **Raise** builds the ground up using each column's own surface block, so it blends in.
- **Lower** digs the ground down.
- **Smooth** nudges every column one tile toward the average height.
- **Roughen** nudges every column randomly up, down or not at all.

Brush radius and step size are in the mod config under **Terrain Wand**.

## Does it work in multiplayer?

Yes. Pastes, erases, commands and Terrain Wand edits sync to the other players. Everyone in the world needs the mod, which tModLoader handles for you when joining. Multiplayer is harder to test than single player, so please open an issue if you see blocks that don't match between players.

## Notes and limits

- **Block names:** commands use Terraria's internal names (`WoodBlock`, not `Wood`). Point at a block and use `/tilename` to find its name.
- **Chest items aren't copied:** chests paste with their name but empty, so copy/paste can't be used to duplicate items. Undo still restores items exactly.
- **Flipped furniture faces the same way:** a chair facing right still faces right after a flip. Rotation isn't supported, since furniture can't turn sideways in Terraria.
- **Paste preview:** walls show their plain middle texture and slopes show as full squares until you paste. Paint isn't shown in the preview, but it is pasted.
- **Modded blocks in schematics:** they load correctly when the same mods are enabled. Moving a schematic to a different mod list can swap or drop modded blocks.
- **`/clear` undoes in two steps:** one for the tiles and walls, one for the liquid.
- **Key conflicts:** some defaults (`S`, `H`, `M`) are also Terraria keys. Rebind them in **Settings → Controls** if they clash with your setup.

## Build from source

You need [tModLoader](https://store.steampowered.com/app/1281930/tModLoader/) and the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0). Clone the repo into `Documents\My Games\Terraria\tModLoader\ModSources\BuildingQOL` (or symlink it there), because the project imports `..\tModLoader.targets` from that folder.

```sh
# build the mod (close tModLoader first, or use Workshop → Develop Mods → Build + Reload in-game)
dotnet build

# the built mod is placed in Documents\My Games\Terraria\tModLoader\Mods\BuildingQOL.tmod
```

### Tech

- C# on [tModLoader](https://github.com/tModLoader/tModLoader) (.NET 8)
- A single region snapshot class handles copy/paste, undo/redo and schematic files, so all three save and restore the same data the same way
- Schematics are saved with tModLoader's `TagIO` format, and chest items go through `ItemIO` so modded items survive
- Flip uses each tile's `TileObjectData` to move multi-tile furniture as one piece
- The paste preview only draws the cells on screen, so large schematics don't hurt the framerate

```
Content/
  Commands/      chat commands and the shared region mutator they use
  Items/         Terrain Wand item, its keybind and sculpting logic
  Selection/     selection, clipboard, undo, schematics, grid overlay and config
  Chat.cs        shared chat message styling
Localization/    in-game text for keybinds, config and items
```
