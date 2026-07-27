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
				caller.Reply("Usage: " + Usage);
				return;
			}

			if (SelectionSystem.Corner1 is not Point16 c1 || SelectionSystem.Corner2 is not Point16 c2)
			{
				caller.Reply("No selection set. Use [ and ] to set one first.");
				return;
			}

			string name = args[0];

			if (TileID.Search.TryGetId(name, out int tileId))
			{
				RegionMutator.Apply(c1, c2, _ => true, tile =>
				{
					tile.HasTile = true;
					tile.TileType = (ushort)tileId;
					tile.TileFrameX = 0;
					tile.TileFrameY = 0;
					tile.IsHalfBlock = false;
					tile.Slope = SlopeType.Solid;
				});
				caller.Reply($"Filled selection with tile {name}.");
				return;
			}

			if (WallID.Search.TryGetId(name, out int wallId))
			{
				RegionMutator.Apply(c1, c2, _ => true, tile => tile.WallType = (ushort)wallId);
				caller.Reply($"Filled selection with wall {name}.");
				return;
			}

			if (LiquidLookup.TryGetId(name, out int liquidId))
			{
				RegionMutator.Apply(c1, c2, _ => true, tile =>
				{
					tile.LiquidType = (byte)liquidId;
					tile.LiquidAmount = 255;
				});
				caller.Reply($"Filled selection with liquid {name}.");
				return;
			}

			caller.Reply($"Couldn't match '{name}' as a tile, wall, or liquid name. Use exact internal IDs (e.g. WoodBlock, not Wood), or water/lava/honey for liquids.");
		}
	}
}
