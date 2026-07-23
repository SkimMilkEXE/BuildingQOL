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
			if (Corner1 is not Point16 c1 || Corner2 is not Point16 c2)
				return;

			int minX = Math.Min(c1.X, c2.X);
			int maxX = Math.Max(c1.X, c2.X);
			int minY = Math.Min(c1.Y, c2.Y);
			int maxY = Math.Max(c1.Y, c2.Y);

			Vector2 topLeft = new Vector2(minX * 16, minY * 16) - Main.screenPosition;
			int width = (maxX - minX + 1) * 16;
			int height = (maxY - minY + 1) * 16;

			Texture2D pixel = TextureAssets.MagicPixel.Value;
			Color color = Color.Cyan * 0.7f;
			const int thickness = 2;

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

			Main.spriteBatch.Draw(pixel, new Rectangle((int)topLeft.X, (int)topLeft.Y, width, thickness), color);
			Main.spriteBatch.Draw(pixel, new Rectangle((int)topLeft.X, (int)topLeft.Y + height - thickness, width, thickness), color);
			Main.spriteBatch.Draw(pixel, new Rectangle((int)topLeft.X, (int)topLeft.Y, thickness, height), color);
			Main.spriteBatch.Draw(pixel, new Rectangle((int)topLeft.X + width - thickness, (int)topLeft.Y, thickness, height), color);

			Main.spriteBatch.End();
		}
	}
}
