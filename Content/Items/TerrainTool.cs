using System;
using System.Collections.Generic;
using BuildingQOL.Content.Selection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace BuildingQOL.Content.Items
{
	public enum TerrainMode
	{
		Raise,
		Lower,
		Smooth,
		Roughen,
	}

	// Column-based terrain sculpting for the Terrain Wand: raise/lower/smooth/roughen a strip of ground
	// centered on the cursor. Unlike RegionMutator (which works on the rectangular [/] selection), this
	// works on a height-map around a click point instead - there's no selection involved.
	internal static class TerrainTool
	{
		private const int SearchWindow = 60;
		private static readonly Random Random = new();

		public static void Apply(TerrainMode mode, int centerX, int centerY, int radius, int stepAmount)
		{
			int minX = centerX - radius;
			int maxX = centerX + radius;

			var heights = new int[maxX - minX + 1];
			for (int x = minX; x <= maxX; x++)
				heights[x - minX] = FindSurfaceY(x, centerY);

			int minY = int.MaxValue;
			int maxY = int.MinValue;
			foreach (int h in heights)
			{
				if (h == -1)
					continue;

				minY = Math.Min(minY, h - stepAmount - 2);
				maxY = Math.Max(maxY, h + stepAmount + 2);
			}

			if (minY > maxY)
				return; // No solid ground found anywhere in range.

			int width = maxX - minX + 1;
			int height = maxY - minY + 1;
			var anchor = new Point16(minX, minY);

			RegionSnapshot before = RegionSnapshot.Capture(minX, minY, width, height);

			switch (mode)
			{
				case TerrainMode.Raise:
					RaiseColumns(minX, heights, stepAmount);
					break;
				case TerrainMode.Lower:
					LowerColumns(minX, heights, stepAmount);
					break;
				case TerrainMode.Smooth:
					SmoothColumns(minX, heights);
					break;
				case TerrainMode.Roughen:
					RoughenColumns(minX, heights);
					break;
			}

			TileFraming.ReframeArea(minX, minY, width, height);
			UndoSystem.Record(anchor, before, RegionSnapshot.Capture(minX, minY, width, height));

			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendTileSquare(Main.myPlayer, minX, minY, width, height);
		}

		// Topmost solid tile within a search window around searchCenterY, or -1 if none found.
		private static int FindSurfaceY(int x, int searchCenterY)
		{
			int top = searchCenterY - SearchWindow;
			int bottom = searchCenterY + SearchWindow;

			for (int y = top; y <= bottom; y++)
			{
				if (!WorldGen.InWorld(x, y))
					continue;

				Tile tile = Main.tile[x, y];
				if (tile.HasTile && Main.tileSolid[tile.TileType])
					return y;
			}

			return -1;
		}

		private static void RaiseColumns(int minX, int[] heights, int amount)
		{
			for (int i = 0; i < heights.Length; i++)
			{
				int surfaceY = heights[i];
				if (surfaceY == -1)
					continue;

				int x = minX + i;
				ushort fillType = Main.tile[x, surfaceY].TileType;
				for (int y = surfaceY - amount; y < surfaceY; y++)
					SetSolid(x, y, fillType);
			}
		}

		private static void LowerColumns(int minX, int[] heights, int amount)
		{
			for (int i = 0; i < heights.Length; i++)
			{
				int surfaceY = heights[i];
				if (surfaceY == -1)
					continue;

				int x = minX + i;
				for (int y = surfaceY; y < surfaceY + amount; y++)
				{
					if (WorldGen.InWorld(x, y))
						WorldGen.KillTile(x, y, noItem: true);
				}
			}
		}

		// Nudges every column one tile toward the average height of the whole brush.
		private static void SmoothColumns(int minX, int[] heights)
		{
			var known = new List<int>();
			foreach (int h in heights)
			{
				if (h != -1)
					known.Add(h);
			}

			if (known.Count == 0)
				return;

			double average = 0;
			foreach (int h in known)
				average += h;
			average /= known.Count;

			for (int i = 0; i < heights.Length; i++)
			{
				int surfaceY = heights[i];
				if (surfaceY == -1)
					continue;

				int x = minX + i;
				if (surfaceY > average)
				{
					ushort fillType = Main.tile[x, surfaceY].TileType;
					SetSolid(x, surfaceY - 1, fillType);
				}
				else if (surfaceY < average && WorldGen.InWorld(x, surfaceY))
				{
					WorldGen.KillTile(x, surfaceY, noItem: true);
				}
			}
		}

		// Nudges every column one tile up, down, or not at all, at random.
		private static void RoughenColumns(int minX, int[] heights)
		{
			for (int i = 0; i < heights.Length; i++)
			{
				int surfaceY = heights[i];
				if (surfaceY == -1)
					continue;

				int x = minX + i;
				int direction = Random.Next(-1, 2);
				if (direction > 0 && WorldGen.InWorld(x, surfaceY))
				{
					WorldGen.KillTile(x, surfaceY, noItem: true);
				}
				else if (direction < 0)
				{
					ushort fillType = Main.tile[x, surfaceY].TileType;
					SetSolid(x, surfaceY - 1, fillType);
				}
			}
		}

		private static void SetSolid(int x, int y, ushort tileType)
		{
			if (!WorldGen.InWorld(x, y))
				return;

			Tile tile = Main.tile[x, y];
			tile.HasTile = true;
			tile.TileType = tileType;
			tile.IsHalfBlock = false;
			tile.Slope = SlopeType.Solid;
		}
	}
}
