using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Selection
{
	// Step 1: selection state + visual outline only. Copy/paste logic comes later.
	public class SelectionSystem : ModSystem
	{
		public static Point16? Corner1;
		public static Point16? Corner2;

		public static ModKeybind SetCorner1Keybind;
		public static ModKeybind SetCorner2Keybind;
		public static ModKeybind ClearSelectionKeybind;
		public static ModKeybind EraseSelectionKeybind;

		public override void Load()
		{
			SetCorner1Keybind = KeybindLoader.RegisterKeybind(Mod, "Set Selection Corner 1", "OemOpenBrackets");
			SetCorner2Keybind = KeybindLoader.RegisterKeybind(Mod, "Set Selection Corner 2", "OemCloseBrackets");
			ClearSelectionKeybind = KeybindLoader.RegisterKeybind(Mod, "Clear Selection", "Back");
			EraseSelectionKeybind = KeybindLoader.RegisterKeybind(Mod, "Erase Selection Tiles", "Delete");
		}

		public override void Unload()
		{
			SetCorner1Keybind = null;
			SetCorner2Keybind = null;
			ClearSelectionKeybind = null;
			EraseSelectionKeybind = null;
			Corner1 = null;
			Corner2 = null;
		}

		// Clears tiles/walls in the selected area using the vanilla removal path (handles multi-tile objects and refresh correctly).
		public static void Erase()
		{
			if (Corner1 is not Point16 c1 || Corner2 is not Point16 c2)
				return;

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

					WorldGen.KillTile(x, y, noItem: true);
					WorldGen.KillWall(x, y);
				}
			}

			if (ModContent.GetInstance<BuildingQOLConfig>().AutoReframeOnPaste)
				TileFraming.ReframeArea(minX, minY, width, height);

			UndoSystem.Record(anchor, before, RegionSnapshot.Capture(minX, minY, width, height));
		}

		public override void PostDrawTiles()
		{
			Texture2D pixel = TextureAssets.MagicPixel.Value;

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

			DrawCursorHighlight(pixel);

			// While only corner 1 is set, the outline follows the mouse as a live preview to help line up corner 2.
			if (Corner1 is Point16 c1)
			{
				Point16 c2 = Corner2 ?? GetMouseTile();
				BuildingQOLConfig config = ModContent.GetInstance<BuildingQOLConfig>();
				Color color = Corner2 is null ? config.OutlineColor * 0.5f : config.OutlineColor;
				DrawSelectionOutline(pixel, c1, c2, color, config.OutlineThickness);
			}

			Main.spriteBatch.End();
		}

		private static Point16 GetMouseTile()
		{
			return new Point16(Terraria.Player.tileTargetX, Terraria.Player.tileTargetY);
		}

		// Highlights the tile under the cursor so corner placement is precise even without the grid on.
		private static void DrawCursorHighlight(Texture2D pixel)
		{
			Point16 mouseTile = GetMouseTile();
			Vector2 pos = new Vector2(mouseTile.X * 16, mouseTile.Y * 16) - Main.screenPosition;

			Main.spriteBatch.Draw(pixel, new Rectangle((int)pos.X, (int)pos.Y, 16, 16), Color.White * 0.3f);
		}

		private static void DrawSelectionOutline(Texture2D pixel, Point16 c1, Point16 c2, Color color, int thickness)
		{
			int minX = Math.Min(c1.X, c2.X);
			int maxX = Math.Max(c1.X, c2.X);
			int minY = Math.Min(c1.Y, c2.Y);
			int maxY = Math.Max(c1.Y, c2.Y);

			Vector2 topLeft = new Vector2(minX * 16, minY * 16) - Main.screenPosition;
			int width = (maxX - minX + 1) * 16;
			int height = (maxY - minY + 1) * 16;

			Main.spriteBatch.Draw(pixel, new Rectangle((int)topLeft.X, (int)topLeft.Y, width, thickness), color);
			Main.spriteBatch.Draw(pixel, new Rectangle((int)topLeft.X, (int)topLeft.Y + height - thickness, width, thickness), color);
			Main.spriteBatch.Draw(pixel, new Rectangle((int)topLeft.X, (int)topLeft.Y, thickness, height), color);
			Main.spriteBatch.Draw(pixel, new Rectangle((int)topLeft.X + width - thickness, (int)topLeft.Y, thickness, height), color);
		}
	}
}
