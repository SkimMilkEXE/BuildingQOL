using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Selection
{
	// Copy/paste within the same world: tiles, walls, chests, signs, and other tile entities.
	// Also save/load the clipboard to a file so a schematic can travel to a different world.
	public class ClipboardSystem : ModSystem
	{
		public static ModKeybind CopyKeybind;
		public static ModKeybind PasteKeybind;
		public static ModKeybind SaveKeybind;
		public static ModKeybind LoadKeybind;

		private static RegionSnapshot _clipboard;
		private static int _width;
		private static int _height;

		private static string SchematicPath => Path.Combine(Main.SavePath, "Schematics", "BuildingQOL", "clipboard.bqol");

		public override void Load()
		{
			CopyKeybind = KeybindLoader.RegisterKeybind(Mod, "Copy Selection", "C");
			PasteKeybind = KeybindLoader.RegisterKeybind(Mod, "Paste Selection", "V");
			SaveKeybind = KeybindLoader.RegisterKeybind(Mod, "Save Schematic To File", "S");
			LoadKeybind = KeybindLoader.RegisterKeybind(Mod, "Load Schematic From File", "L");
		}

		public override void Unload()
		{
			CopyKeybind = null;
			PasteKeybind = null;
			SaveKeybind = null;
			LoadKeybind = null;
			_clipboard = null;
		}

		public static void Clear()
		{
			_clipboard = null;
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
			_clipboard = RegionSnapshot.Capture(minX, minY, _width, _height, includeChestItems: false);
		}

		public static void Paste(Point16 anchor)
		{
			if (_clipboard == null)
				return;

			RegionSnapshot before = RegionSnapshot.Capture(anchor.X, anchor.Y, _width, _height);
			_clipboard.Apply(anchor);
			UndoSystem.Record(anchor, before, RegionSnapshot.Capture(anchor.X, anchor.Y, _width, _height));
		}

		// Single fixed save slot; rename/move the file outside the game to keep more than one schematic.
		public static void SaveToFile()
		{
			if (_clipboard == null)
				return;

			_clipboard.Save(SchematicPath);
		}

		public static void LoadFromFile()
		{
			if (!File.Exists(SchematicPath))
				return;

			_clipboard = RegionSnapshot.Load(SchematicPath);
			_width = _clipboard.Width;
			_height = _clipboard.Height;
		}
	}
}
