using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Selection
{
	// Step 2/3: raw tile copy/paste within the same world, plus chest contents and sign text.
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

		private struct ChestData
		{
			public string Name;
			public Item[] Items;
		}

		public static ModKeybind CopyKeybind;
		public static ModKeybind PasteKeybind;

		private static TileData[,] _clipboard;
		private static int _width;
		private static int _height;
		private static readonly Dictionary<Point16, ChestData> _chests = new();
		private static readonly Dictionary<Point16, string> _signs = new();
		private static readonly Dictionary<Point16, (Type Type, byte[] Data)> _tileEntities = new();

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
			_chests.Clear();
			_signs.Clear();
			_tileEntities.Clear();
		}

		public static void Clear()
		{
			_clipboard = null;
			_chests.Clear();
			_signs.Clear();
			_tileEntities.Clear();
		}

		private static int FindSign(int x, int y)
		{
			for (int i = 0; i < Main.sign.Length; i++)
			{
				Sign sign = Main.sign[i];
				if (sign != null && sign.x == x && sign.y == y)
					return i;
			}

			return -1;
		}

		// Vanilla TileEntity subtypes each have their own static Place(x, y) factory; no common interface for it.
		private static int PlaceTileEntity(Type type, int x, int y)
		{
			if (type == typeof(TEItemFrame)) return TEItemFrame.Place(x, y);
			if (type == typeof(TEWeaponsRack)) return TEWeaponsRack.Place(x, y);
			if (type == typeof(TEFoodPlatter)) return TEFoodPlatter.Place(x, y);
			if (type == typeof(TEHatRack)) return TEHatRack.Place(x, y);
			if (type == typeof(TEDisplayDoll)) return TEDisplayDoll.Place(x, y);
			if (type == typeof(TETrainingDummy)) return TETrainingDummy.Place(x, y);
			if (type == typeof(TELogicSensor)) return TELogicSensor.Place(x, y);
			if (type == typeof(TETeleportationPylon)) return TETeleportationPylon.Place(x, y);
			return -1;
		}

		// Ghost preview of where the clipboard will land, so paste isn't a blind guess.
		public override void PostDrawTiles()
		{
			if (_clipboard == null)
				return;

			Vector2 topLeft = new Vector2(Terraria.Player.tileTargetX * 16, Terraria.Player.tileTargetY * 16) - Main.screenPosition;

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
			Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)topLeft.X, (int)topLeft.Y, _width * 16, _height * 16), Color.LimeGreen * 0.35f);
			Main.spriteBatch.End();
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
			_chests.Clear();
			_signs.Clear();
			_tileEntities.Clear();

			for (int x = 0; x < _width; x++)
			{
				for (int y = 0; y < _height; y++)
				{
					int worldX = minX + x;
					int worldY = minY + y;
					Tile tile = Main.tile[worldX, worldY];
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

					int chestIndex = Chest.FindChest(worldX, worldY);
					if (chestIndex != -1)
					{
						Chest sourceChest = Main.chest[chestIndex];
						var items = new Item[Chest.maxItems];
						for (int slot = 0; slot < Chest.maxItems; slot++)
							items[slot] = sourceChest.item[slot].Clone();

						_chests[new Point16(x, y)] = new ChestData { Name = sourceChest.name, Items = items };
					}

					int signIndex = FindSign(worldX, worldY);
					if (signIndex != -1)
						_signs[new Point16(x, y)] = Main.sign[signIndex].text;

					if (TileEntity.ByPosition.TryGetValue(new Point16(worldX, worldY), out TileEntity sourceEntity))
					{
						using var stream = new MemoryStream();
						using var writer = new BinaryWriter(stream);
						sourceEntity.WriteExtraData(writer, true);
						_tileEntities[new Point16(x, y)] = (sourceEntity.GetType(), stream.ToArray());
					}
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

			if (ModContent.GetInstance<BuildingQOLConfig>().AutoReframeOnPaste)
				TileFraming.ReframeArea(anchor.X, anchor.Y, _width, _height);

			foreach (KeyValuePair<Point16, ChestData> entry in _chests)
			{
				int worldX = anchor.X + entry.Key.X;
				int worldY = anchor.Y + entry.Key.Y;
				if (!WorldGen.InWorld(worldX, worldY))
					continue;

				int chestIndex = Chest.FindEmptyChest();
				if (chestIndex == -1)
					continue;

				Chest chest = new Chest(false) { x = worldX, y = worldY, name = entry.Value.Name };
				for (int slot = 0; slot < Chest.maxItems; slot++)
					chest.item[slot] = entry.Value.Items[slot].Clone();

				Main.chest[chestIndex] = chest;
			}

			foreach (KeyValuePair<Point16, string> entry in _signs)
			{
				int worldX = anchor.X + entry.Key.X;
				int worldY = anchor.Y + entry.Key.Y;
				if (!WorldGen.InWorld(worldX, worldY))
					continue;

				int signIndex = System.Array.IndexOf(Main.sign, null);
				if (signIndex == -1)
					continue;

				Main.sign[signIndex] = new Sign { x = worldX, y = worldY, text = entry.Value };
			}

			foreach (KeyValuePair<Point16, (Type Type, byte[] Data)> entry in _tileEntities)
			{
				int worldX = anchor.X + entry.Key.X;
				int worldY = anchor.Y + entry.Key.Y;
				if (!WorldGen.InWorld(worldX, worldY))
					continue;

				int newId = PlaceTileEntity(entry.Value.Type, worldX, worldY);
				if (newId == -1 || !TileEntity.ByID.TryGetValue(newId, out TileEntity newEntity))
					continue;

				using var stream = new MemoryStream(entry.Value.Data);
				using var reader = new BinaryReader(stream);
				newEntity.ReadExtraData(reader, true);
			}
		}
	}
}
