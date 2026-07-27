using System;
using BuildingQOL.Content.Selection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Commands
{
	// /floodfill <liquid> - fills only the open (non-solid) space in the selection with liquid, then lets it settle naturally.
	public class FloodFillCommand : ModCommand
	{
		public override CommandType Type => CommandType.Chat;
		public override string Command => "floodfill";
		public override string Description => "Fills the open space in your selection with liquid, then lets it settle naturally.";
		public override string Usage => "/floodfill <water|lava|honey|shimmer>";

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

			if (!LiquidLookup.TryGetId(args[0], out int liquidId))
			{
				caller.Reply($"Couldn't match '{args[0]}' as a liquid. Use water, lava, honey, or shimmer.");
				return;
			}

			RegionMutator.Apply(c1, c2, tile => !tile.HasTile, tile =>
			{
				tile.LiquidType = (byte)liquidId;
				tile.LiquidAmount = 255;
			});

			// Queue every open cell for the vanilla liquid settle simulation so it spreads/flows naturally.
			int minX = Math.Min(c1.X, c2.X);
			int maxX = Math.Max(c1.X, c2.X);
			int minY = Math.Min(c1.Y, c2.Y);
			int maxY = Math.Max(c1.Y, c2.Y);

			for (int x = minX; x <= maxX; x++)
			{
				for (int y = minY; y <= maxY; y++)
				{
					if (WorldGen.InWorld(x, y) && !Main.tile[x, y].HasTile)
						Liquid.AddWater(x, y);
				}
			}

			caller.Reply($"Flood-filled open space in selection with {args[0]}.");
		}
	}
}
