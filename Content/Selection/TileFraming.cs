using Terraria;

namespace BuildingQOL.Content.Selection
{
	// Shared re-framing pass used after both paste and erase so edited tiles blend with neighbors.
	internal static class TileFraming
	{
		public static void ReframeArea(int minX, int minY, int width, int height)
		{
			for (int x = -1; x <= width; x++)
			{
				for (int y = -1; y <= height; y++)
				{
					int worldX = minX + x;
					int worldY = minY + y;
					if (!WorldGen.InWorld(worldX, worldY))
						continue;

					WorldGen.TileFrame(worldX, worldY);
					WorldGen.SquareWallFrame(worldX, worldY);
				}
			}
		}
	}
}
