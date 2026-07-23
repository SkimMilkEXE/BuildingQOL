using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Selection
{
	public class GridSystem : ModSystem
	{
		public static bool Enabled;
		public static ModKeybind ToggleKeybind;

		public override void Load()
		{
			ToggleKeybind = KeybindLoader.RegisterKeybind(Mod, "Toggle Tile Grid", "G");
		}

		public override void Unload()
		{
			ToggleKeybind = null;
		}

		public override void PostDrawTiles()
		{
			if (!Enabled)
				return;

			Texture2D pixel = TextureAssets.MagicPixel.Value;
			Color color = Color.White * 0.2f;

			int firstTileX = (int)(Main.screenPosition.X / 16);
			int firstTileY = (int)(Main.screenPosition.Y / 16);
			int tilesWide = Main.screenWidth / 16 + 2;
			int tilesHigh = Main.screenHeight / 16 + 2;

			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

			for (int x = 0; x <= tilesWide; x++)
			{
				int screenX = (int)(((firstTileX + x) * 16) - Main.screenPosition.X);
				Main.spriteBatch.Draw(pixel, new Rectangle(screenX, 0, 1, Main.screenHeight), color);
			}

			for (int y = 0; y <= tilesHigh; y++)
			{
				int screenY = (int)(((firstTileY + y) * 16) - Main.screenPosition.Y);
				Main.spriteBatch.Draw(pixel, new Rectangle(0, screenY, Main.screenWidth, 1), color);
			}

			Main.spriteBatch.End();
		}
	}
}
