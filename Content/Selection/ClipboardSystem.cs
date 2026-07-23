using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Selection
{
	// Step 2: raw tile copy/paste within the same world. No tile entities (chests/signs) yet — that's step 3.
	public class ClipboardSystem : ModSystem
	{
		private struct TileData
		{
			public bool HasTile;
			public ushort TileType;
			public short FrameX;
			public short FrameY;
			public bool HalfBrick;
			public SlopeType Slope;
			public byte TileColor;
			public ushort WallType;
			public byte WallColor;
		}

		public static ModKeybind CopyKeybind;
		public static ModKeybind PasteKeybind;

		private static TileData[,] _clipboard;
		private static int _width;
		private static int _height;

		public override void Load()
		{
			CopyKeybind = KeybindLoader.RegisterKeybind(Mod, "Copy Selection", "C");
			PasteKeybind = KeybindLoader.RegisterKeybind(Mod, "Paste Selection", "V");
		}

		public override void Unload()
		{
			CopyKeybind = null;
			PasteKeybind = null;
			_clipboard = null;
		}

		public static void Copy()
		{
			if (SelectionSystem.Corner1 is not Point16 c1 || SelectionSystem.Corner2 is not Point16 c2)
				return;

			int minX = System.Math.Min(c1.X, c2.X);
			int minY = System.Math.Min(c1.Y, c2.Y);
			_width = System.Math.Abs(c1.X - c2.X) + 1;
			_height = System.Math.Abs(c1.Y - c2.Y) + 1;
			_clipboard = new TileData[_width, _height];

			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					Tile tile = Main.tile[minX + x, minY + y];
					_clipboard[x, y] = new TileData
					{
						HasTile = tile.HasTile,
						TileType = tile.TileType,
						FrameX = tile.TileFrameX,
						FrameY = tile.TileFrameY,
						HalfBrick = tile.IsHalfBlock,
						Slope = tile.Slope,
						TileColor = tile.TileColor,
						WallType = tile.WallType,
						WallColor = tile.WallColor,
					};
				}
			}
		}

		public static void Paste(Point16 anchor)
		{
			if (_clipboard == null)
				return;

			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					int worldX = anchor.X + x;
					int worldY = anchor.Y + y;
					if (!WorldGen.InWorld(worldX, worldY))
						continue;

					TileData data = _clipboard[x, y];
					Tile tile = Main.tile[worldX, worldY];
					tile.HasTile = data.HasTile;
					tile.TileType = data.TileType;
					tile.TileFrameX = data.FrameX;
					tile.TileFrameY = data.FrameY;
					tile.IsHalfBlock = data.HalfBrick;
					tile.Slope = data.Slope;
					tile.TileColor = data.TileColor;
					tile.WallType = data.WallType;
					tile.WallColor = data.WallColor;
				}
			}

			// Re-frame the pasted area plus a 1-tile border so it blends with existing neighbors.
			for (int x = -1; x <= _width; x++)
			{
				for (int y = -1; y <= _height; y++)
				{
					int worldX = anchor.X + x;
					int worldY = anchor.Y + y;
					if (!WorldGen.InWorld(worldX, worldY))
						continue;

					WorldGen.TileFrame(worldX, worldY);
					WorldGen.SquareWallFrame(worldX, worldY);
				}
			}
		}
	}
}
