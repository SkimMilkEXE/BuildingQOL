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

		public override void Load()
		{
			SetCorner1Keybind = KeybindLoader.RegisterKeybind(Mod, "Set Selection Corner 1", "OemOpenBrackets");
			SetCorner2Keybind = KeybindLoader.RegisterKeybind(Mod, "Set Selection Corner 2", "OemCloseBrackets");
			ClearSelectionKeybind = KeybindLoader.RegisterKeybind(Mod, "Clear Selection", "Back");
		}

		public override void Unload()
		{
			SetCorner1Keybind = null;
			SetCorner2Keybind = null;
			ClearSelectionKeybind = null;
			Corner1 = null;
			Corner2 = null;
		}

		public override void PostDrawTiles()
		{
			Texture2D pixel = TextureAssets.MagicPixel.Value;

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

			DrawCursorHighlight(pixel);

			if (Corner1 is Point16 c1 && Corner2 is Point16 c2)
				DrawSelectionOutline(pixel, c1, c2, ModContent.GetInstance<BuildingQOLConfig>());

			Main.spriteBatch.End();
		}

		// Highlights the tile under the cursor so corner placement is precise even without the grid on.
		private static void DrawCursorHighlight(Texture2D pixel)
		{
			int tileX = (int)(Main.MouseWorld.X / 16);
			int tileY = (int)(Main.MouseWorld.Y / 16);
			Vector2 pos = new Vector2(tileX * 16, tileY * 16) - Main.screenPosition;

			Main.spriteBatch.Draw(pixel, new Rectangle((int)pos.X, (int)pos.Y, 16, 16), Color.White * 0.3f);
		}

		private static void DrawSelectionOutline(Texture2D pixel, Point16 c1, Point16 c2, BuildingQOLConfig config)
		{
			int minX = Math.Min(c1.X, c2.X);
			int maxX = Math.Max(c1.X, c2.X);
			int minY = Math.Min(c1.Y, c2.Y);
			int maxY = Math.Max(c1.Y, c2.Y);

			Vector2 topLeft = new Vector2(minX * 16, minY * 16) - Main.screenPosition;
			int width = (maxX - minX + 1) * 16;
			int height = (maxY - minY + 1) * 16;

			Color color = config.OutlineColor;
			int thickness = config.OutlineThickness;

			Main.spriteBatch.Draw(pixel, new Rectangle((int)topLeft.X, (int)topLeft.Y, width, thickness), color);
			Main.spriteBatch.Draw(pixel, new Rectangle((int)topLeft.X, (int)topLeft.Y + height - thickness, width, thickness), color);
			Main.spriteBatch.Draw(pixel, new Rectangle((int)topLeft.X, (int)topLeft.Y, thickness, height), color);
			Main.spriteBatch.Draw(pixel, new Rectangle((int)topLeft.X + width - thickness, (int)topLeft.Y, thickness, height), color);
		}
	}
}
