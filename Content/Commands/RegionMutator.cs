using System;
using BuildingQOL.Content.Selection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Commands
{
	// Shared "loop the selection, mutate matching tiles, reframe, record undo, sync" used by /blockswap and /fill.
	internal static class RegionMutator
	{
		public static void Apply(Point16 c1, Point16 c2, Predicate<Tile> matches, Action<Tile> apply)
		{
			int minX = Math.Min(c1.X, c2.X);
			int maxX = Math.Max(c1.X, c2.X);
			int minY = Math.Min(c1.Y, c2.Y);
			int maxY = Math.Max(c1.Y, c2.Y);
			int width = maxX - minX + 1;
			int height = maxY - minY + 1;
			var anchor = new Point16(minX, minY);

			RegionSnapshot before = RegionSnapshot.Capture(minX, minY, width, height);

			for (int x = minX; x <= maxX; x++)
			{
				for (int y = minY; y <= maxY; y++)
				{
					if (!WorldGen.InWorld(x, y))
						continue;

					Tile tile = Main.tile[x, y];
					if (matches(tile))
						apply(tile);
				}
			}

			if (ModContent.GetInstance<BuildingQOLConfig>().AutoReframeOnPaste)
				TileFraming.ReframeArea(minX, minY, width, height);

			UndoSystem.Record(anchor, before, RegionSnapshot.Capture(minX, minY, width, height));

			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendTileSquare(Main.myPlayer, minX, minY, width, height);
		}
	}
}
