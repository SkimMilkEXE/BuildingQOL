using BuildingQOL.Content.Selection;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Commands
{
	// /fill <block> - fills the entire current selection with one tile, wall, or liquid type.
	public class FillCommand : ModCommand
	{
		public override CommandType Type => CommandType.Chat;
		public override string Command => "fill";
		public override string Description => "Fills your current selection with the given tile/wall/liquid type.";
		public override string Usage => "/fill <block>  (e.g. /fill StoneBlock, or /fill lava)";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (args.Length != 1)
			{
				Chat.Error("Usage: " + Usage);
				return;
			}

			if (SelectionSystem.Corner1 is not Point16 c1 || SelectionSystem.Corner2 is not Point16 c2)
			{
				Chat.Error("No selection set. Use [ and ] to set one first.");
				return;
			}

			if (TryFill(c1, c2, args[0], (_, _, _) => true, out string kind))
				Chat.Success($"Filled selection with {kind} {args[0]}.");
			else
				Chat.Error(NoMatchMessage(args[0]));
		}

		// sets every matching cell to the named tile, wall, or liquid. shared with /outline
		// kind is "tile", "wall" or "liquid" for the reply message
		internal static bool TryFill(Point16 c1, Point16 c2, string name, System.Func<int, int, Terraria.Tile, bool> where, out string kind)
		{
			if (IdSearch.TryGetTileId(name, out int tileId))
			{
				RegionMutator.Apply(c1, c2, where, tile =>
				{
					tile.HasTile = true;
					tile.TileType = (ushort)tileId;
					tile.TileFrameX = 0;
					tile.TileFrameY = 0;
					tile.IsHalfBlock = false;
					tile.Slope = SlopeType.Solid;
				});
				kind = "tile";
				return true;
			}

			if (IdSearch.TryGetWallId(name, out int wallId))
			{
				RegionMutator.Apply(c1, c2, where, tile => tile.WallType = (ushort)wallId);
				kind = "wall";
				return true;
			}

			if (LiquidLookup.TryGetId(name, out int liquidId))
			{
				RegionMutator.Apply(c1, c2, where, tile =>
				{
					tile.LiquidType = (byte)liquidId;
					tile.LiquidAmount = 255;
				});
				kind = "liquid";
				return true;
			}

			kind = null;
			return false;
		}

		internal static string NoMatchMessage(string name) =>
			$"Couldn't match '{name}' as a tile, wall, or liquid name. Use exact internal IDs (e.g. WoodBlock, not Wood), or water/lava/honey for liquids.";
	}
}
