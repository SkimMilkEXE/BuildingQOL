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
		public static ModKeybind FlipKeybind;

		private static RegionSnapshot _clipboard;
		private static int _width;
		private static int _height;

		// the s/l keybinds use this slot so older saves keep working
		public const string DefaultSchematicName = "clipboard";
		private const string SchematicExtension = ".bqol";

		public static string SchematicFolder => Path.Combine(Main.SavePath, "Schematics", "BuildingQOL");

		private static string PathFor(string name) => Path.Combine(SchematicFolder, name + SchematicExtension);

		// blocks names that would escape the schematics folder or aren't valid file names
		public static bool IsValidName(string name)
		{
			return !string.IsNullOrWhiteSpace(name)
				&& name != "." && name != ".."
				&& name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0
				&& name.IndexOfAny(new[] { '/', '\\' }) < 0; // linux allows '\' in file names, so check it here too
		}

		public override void Load()
		{
			CopyKeybind = KeybindLoader.RegisterKeybind(Mod, "Copy Selection", "C");
			PasteKeybind = KeybindLoader.RegisterKeybind(Mod, "Paste Selection", "V");
			SaveKeybind = KeybindLoader.RegisterKeybind(Mod, "Save Schematic To File", "S");
			LoadKeybind = KeybindLoader.RegisterKeybind(Mod, "Load Schematic From File", "L");
			FlipKeybind = KeybindLoader.RegisterKeybind(Mod, "Flip Clipboard Horizontally", "X");
		}

		public override void Unload()
		{
			CopyKeybind = null;
			PasteKeybind = null;
			SaveKeybind = null;
			LoadKeybind = null;
			FlipKeybind = null;
			_clipboard = null;
		}

		// The in-memory clipboard came from a specific world; drop it on world switch (file save/load is unaffected).
		public override void OnWorldLoad() => Clear();

		public override void OnWorldUnload() => Clear();

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

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
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

		public static void Flip(bool horizontal)
		{
			if (_clipboard == null)
			{
				Main.NewText("Nothing copied yet - press C to copy a selection first.", Color.OrangeRed);
				return;
			}

			_clipboard = _clipboard.Flipped(horizontal);
			Main.NewText($"Flipped clipboard {(horizontal ? "horizontally" : "vertically")}.", Color.LimeGreen);
		}

		public static void SaveToFile(string name = DefaultSchematicName)
		{
			if (_clipboard == null)
			{
				Main.NewText("Nothing copied yet - press C to copy a selection first.", Color.OrangeRed);
				return;
			}

			try
			{
				_clipboard.Save(PathFor(name));
				Main.NewText($"Saved {_width}x{_height} schematic '{name}'.", Color.LimeGreen);
			}
			catch (System.Exception e) when (e is IOException or System.UnauthorizedAccessException)
			{
				Main.NewText($"Couldn't save schematic '{name}': {e.Message}", Color.OrangeRed);
			}
		}

		public static void LoadFromFile(string name = DefaultSchematicName)
		{
			string path = PathFor(name);
			if (!File.Exists(path))
			{
				Main.NewText($"No saved schematic named '{name}' - use /schem list to see saved ones.", Color.OrangeRed);
				return;
			}

			// a damaged or hand-edited file shouldn't crash the game
			try
			{
				_clipboard = RegionSnapshot.Load(path);
			}
			catch (System.Exception e)
			{
				Main.NewText($"Couldn't load schematic '{name}': {e.Message}", Color.OrangeRed);
				return;
			}

			_width = _clipboard.Width;
			_height = _clipboard.Height;
			Main.NewText($"Loaded {_width}x{_height} schematic '{name}' - press V to paste.", Color.LimeGreen);
		}

		public static string[] ListNames()
		{
			if (!Directory.Exists(SchematicFolder))
				return System.Array.Empty<string>();

			string[] names = Directory.GetFiles(SchematicFolder, "*" + SchematicExtension);
			for (int i = 0; i < names.Length; i++)
				names[i] = Path.GetFileNameWithoutExtension(names[i]);
			System.Array.Sort(names, System.StringComparer.OrdinalIgnoreCase);
			return names;
		}

		public static bool Delete(string name)
		{
			string path = PathFor(name);
			if (!File.Exists(path))
				return false;

			File.Delete(path);
			return true;
		}
	}
}
