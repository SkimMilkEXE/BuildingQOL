using BuildingQOL.Content.Selection;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Commands
{
	// /blockswap <from> <to> - replaces one tile, wall, or liquid type with another inside the current selection.
	public class BlockSwapCommand : ModCommand
	{
		public override CommandType Type => CommandType.Chat;
		public override string Command => "blockswap";
		public override string Description => "Replaces one tile/wall/liquid type with another inside your current selection.";
		public override string Usage => "/blockswap <from> <to>  (e.g. /blockswap WoodBlock RichMahogany, or /blockswap water lava)";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (args.Length != 2)
			{
				caller.Reply("Usage: " + Usage);
				return;
			}

			if (SelectionSystem.Corner1 is not Point16 c1 || SelectionSystem.Corner2 is not Point16 c2)
			{
				caller.Reply("No selection set. Use [ and ] to set one first.");
				return;
			}

			if (TryResolveTile(args[0], out int fromTile) && TryResolveTile(args[1], out int toTile))
			{
				RegionMutator.Apply(c1, c2, tile => tile.HasTile && tile.TileType == fromTile, tile => tile.TileType = (ushort)toTile);
				caller.Reply($"Swapped tile {args[0]} -> {args[1]} in selection.");
				return;
			}

			if (TryResolveWall(args[0], out int fromWall) && TryResolveWall(args[1], out int toWall))
			{
				RegionMutator.Apply(c1, c2, tile => tile.WallType == fromWall, tile => tile.WallType = (ushort)toWall);
				caller.Reply($"Swapped wall {args[0]} -> {args[1]} in selection.");
				return;
			}

			if (TryResolveLiquid(args[0], out int fromLiquid) && TryResolveLiquid(args[1], out int toLiquid))
			{
				RegionMutator.Apply(c1, c2, tile => tile.LiquidAmount > 0 && tile.LiquidType == fromLiquid, tile => tile.LiquidType = (byte)toLiquid);
				caller.Reply($"Swapped liquid {args[0]} -> {args[1]} in selection.");
				return;
			}

			caller.Reply($"Couldn't match '{args[0]}' and '{args[1]}' as a tile, wall, or liquid pair. Use exact internal IDs (e.g. WoodBlock, not Wood).");
		}

		private static bool TryResolveTile(string name, out int id) => TileID.Search.TryGetId(name, out id);
		private static bool TryResolveWall(string name, out int id) => WallID.Search.TryGetId(name, out id);
		private static bool TryResolveLiquid(string name, out int id) => LiquidLookup.TryGetId(name, out id);
	}
}
