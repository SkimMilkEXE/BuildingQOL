using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace BuildingQOL.Content.Selection
{
	// Captures a rectangular region of the world (tiles, walls, chests, signs, tile entities) and can stamp it back.
	// Shared by the clipboard (copy/paste) and the undo/redo history, which both just need "snapshot region, restore region".
	public class RegionSnapshot
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
			public Item[] Items; // null when captured without item contents (see Capture's includeChestItems)
		}

		public readonly int Width;
		public readonly int Height;

		private readonly TileData[,] _tiles;
		private readonly Dictionary<Point16, ChestData> _chests = new();
		private readonly Dictionary<Point16, string> _signs = new();
		private readonly Dictionary<Point16, (Type Type, byte[] Data)> _tileEntities = new();

		private RegionSnapshot(int width, int height)
		{
			Width = width;
			Height = height;
			_tiles = new TileData[width, height];
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

		private static Type TileEntityTypeFromName(string name) => name switch
		{
			nameof(TEItemFrame) => typeof(TEItemFrame),
			nameof(TEWeaponsRack) => typeof(TEWeaponsRack),
			nameof(TEFoodPlatter) => typeof(TEFoodPlatter),
			nameof(TEHatRack) => typeof(TEHatRack),
			nameof(TEDisplayDoll) => typeof(TEDisplayDoll),
			nameof(TETrainingDummy) => typeof(TETrainingDummy),
			nameof(TELogicSensor) => typeof(TELogicSensor),
			nameof(TETeleportationPylon) => typeof(TETeleportationPylon),
			_ => null,
		};

		// includeChestItems is false for the clipboard so copy/paste can't duplicate items; undo/redo always needs
		// full fidelity since it's restoring exactly what a paste/erase changed, not cloning anything new.
		public static RegionSnapshot Capture(int minX, int minY, int width, int height, bool includeChestItems = true)
		{
			var snapshot = new RegionSnapshot(width, height);

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int worldX = minX + x;
					int worldY = minY + y;
					Tile tile = Main.tile[worldX, worldY];
					snapshot._tiles[x, y] = new TileData
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
						Item[] items = null;
						if (includeChestItems)
						{
							items = new Item[Chest.maxItems];
							for (int slot = 0; slot < Chest.maxItems; slot++)
								items[slot] = sourceChest.item[slot].Clone();
						}

						snapshot._chests[new Point16(x, y)] = new ChestData { Name = sourceChest.name, Items = items };
					}

					int signIndex = FindSign(worldX, worldY);
					if (signIndex != -1)
						snapshot._signs[new Point16(x, y)] = Main.sign[signIndex].text;

					if (TileEntity.ByPosition.TryGetValue(new Point16(worldX, worldY), out TileEntity sourceEntity))
					{
						using var stream = new MemoryStream();
						using var writer = new BinaryWriter(stream);
						sourceEntity.WriteExtraData(writer, true);
						snapshot._tileEntities[new Point16(x, y)] = (sourceEntity.GetType(), stream.ToArray());
					}
				}
			}

			return snapshot;
		}

		public void Apply(Point16 anchor)
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					int worldX = anchor.X + x;
					int worldY = anchor.Y + y;
					if (!WorldGen.InWorld(worldX, worldY))
						continue;

					// Clear whatever entity currently occupies this cell so stale chests/signs/entities don't linger.
					int oldChestIndex = Chest.FindChest(worldX, worldY);
					if (oldChestIndex != -1)
						Main.chest[oldChestIndex] = null;

					int oldSignIndex = FindSign(worldX, worldY);
					if (oldSignIndex != -1)
						Main.sign[oldSignIndex] = null;

					if (TileEntity.ByPosition.TryGetValue(new Point16(worldX, worldY), out TileEntity existingEntity))
					{
						TileEntity.ByPosition.Remove(new Point16(worldX, worldY));
						TileEntity.ByID.Remove(existingEntity.ID);
					}

					TileData data = _tiles[x, y];
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
				TileFraming.ReframeArea(anchor.X, anchor.Y, Width, Height);

			foreach (KeyValuePair<Point16, ChestData> entry in _chests)
			{
				int worldX = anchor.X + entry.Key.X;
				int worldY = anchor.Y + entry.Key.Y;
				if (!WorldGen.InWorld(worldX, worldY))
					continue;

				int chestIndex = Array.IndexOf(Main.chest, null);
				if (chestIndex == -1)
					continue;

				Chest chest = new Chest(true) { x = worldX, y = worldY, name = entry.Value.Name };
				if (entry.Value.Items != null)
				{
					for (int slot = 0; slot < Chest.maxItems; slot++)
						chest.item[slot] = entry.Value.Items[slot].Clone();
				}

				Main.chest[chestIndex] = chest;
			}

			foreach (KeyValuePair<Point16, string> entry in _signs)
			{
				int worldX = anchor.X + entry.Key.X;
				int worldY = anchor.Y + entry.Key.Y;
				if (!WorldGen.InWorld(worldX, worldY))
					continue;

				int signIndex = Array.IndexOf(Main.sign, null);
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

		// Vanilla tile/wall IDs are stable across worlds, so raw netIDs are fine there. Chest items go through
		// ItemIO/TagCompound instead so modded items survive being loaded into a different mod list.
		// ponytail: tile/wall netIDs for modded content aren't remapped by name, only reliable within the same mod list.
		private TagCompound ToTag()
		{
			var tileBytes = new byte[Width * Height * 15];
			using (var stream = new MemoryStream(tileBytes))
			using (var writer = new BinaryWriter(stream))
			{
				for (int x = 0; x < Width; x++)
				{
					for (int y = 0; y < Height; y++)
					{
						TileData data = _tiles[x, y];
						writer.Write(data.HasTile);
						writer.Write(data.TileType);
						writer.Write(data.FrameX);
						writer.Write(data.FrameY);
						writer.Write(data.HalfBrick);
						writer.Write((byte)data.Slope);
						writer.Write(data.TileColor);
						writer.Write(data.WallType);
						writer.Write(data.WallColor);
					}
				}
			}

			var chests = _chests.Select(entry => new TagCompound
			{
				["X"] = (int)entry.Key.X,
				["Y"] = (int)entry.Key.Y,
				["Name"] = entry.Value.Name ?? "",
				["HasItems"] = entry.Value.Items != null,
				["Items"] = entry.Value.Items?.Select(item => ItemIO.Save(item)).ToList() ?? new List<TagCompound>(),
			}).ToList();

			var signs = _signs.Select(entry => new TagCompound
			{
				["X"] = (int)entry.Key.X,
				["Y"] = (int)entry.Key.Y,
				["Text"] = entry.Value,
			}).ToList();

			var entities = _tileEntities.Select(entry => new TagCompound
			{
				["X"] = (int)entry.Key.X,
				["Y"] = (int)entry.Key.Y,
				["TypeName"] = entry.Value.Type.Name,
				["Data"] = entry.Value.Data,
			}).ToList();

			return new TagCompound
			{
				["Width"] = Width,
				["Height"] = Height,
				["Tiles"] = tileBytes,
				["Chests"] = chests,
				["Signs"] = signs,
				["Entities"] = entities,
			};
		}

		private static RegionSnapshot FromTag(TagCompound tag)
		{
			int width = tag.Get<int>("Width");
			int height = tag.Get<int>("Height");
			var snapshot = new RegionSnapshot(width, height);

			byte[] tileBytes = tag.Get<byte[]>("Tiles");
			using (var stream = new MemoryStream(tileBytes))
			using (var reader = new BinaryReader(stream))
			{
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						snapshot._tiles[x, y] = new TileData
						{
							HasTile = reader.ReadBoolean(),
							TileType = reader.ReadUInt16(),
							FrameX = reader.ReadInt16(),
							FrameY = reader.ReadInt16(),
							HalfBrick = reader.ReadBoolean(),
							Slope = (SlopeType)reader.ReadByte(),
							TileColor = reader.ReadByte(),
							WallType = reader.ReadUInt16(),
							WallColor = reader.ReadByte(),
						};
					}
				}
			}

			foreach (TagCompound chestTag in tag.Get<List<TagCompound>>("Chests"))
			{
				var pos = new Point16((short)chestTag.Get<int>("X"), (short)chestTag.Get<int>("Y"));
				Item[] items = chestTag.Get<bool>("HasItems")
					? chestTag.Get<List<TagCompound>>("Items").Select(ItemIO.Load).ToArray()
					: null;

				snapshot._chests[pos] = new ChestData { Name = chestTag.Get<string>("Name"), Items = items };
			}

			foreach (TagCompound signTag in tag.Get<List<TagCompound>>("Signs"))
			{
				var pos = new Point16((short)signTag.Get<int>("X"), (short)signTag.Get<int>("Y"));
				snapshot._signs[pos] = signTag.Get<string>("Text");
			}

			foreach (TagCompound entityTag in tag.Get<List<TagCompound>>("Entities"))
			{
				Type type = TileEntityTypeFromName(entityTag.Get<string>("TypeName"));
				if (type == null)
					continue;

				var pos = new Point16((short)entityTag.Get<int>("X"), (short)entityTag.Get<int>("Y"));
				snapshot._tileEntities[pos] = (type, entityTag.Get<byte[]>("Data"));
			}

			return snapshot;
		}

		public void Save(string path)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			using FileStream stream = File.Create(path);
			TagIO.ToStream(ToTag(), stream);
		}

		public static RegionSnapshot Load(string path)
		{
			using FileStream stream = File.OpenRead(path);
			return FromTag(TagIO.FromStream(stream));
		}
	}
}
