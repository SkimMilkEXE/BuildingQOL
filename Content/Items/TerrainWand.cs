using BuildingQOL.Content.Selection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Items
{
	// Craftable from 10 Wood in case a starting player loses theirs. Cycle modes with the keybind
	// registered in TerrainWandSystem; the current mode is drawn from there too.
	public class TerrainWand : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.value = 0;
			Item.rare = ItemRarityID.White;
			Item.UseSound = SoundID.Dig;
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI != Main.myPlayer)
				return true;

			BuildingQOLConfig config = ModContent.GetInstance<BuildingQOLConfig>();
			TerrainTool.Apply(TerrainWandSystem.Mode, Player.tileTargetX, Player.tileTargetY, config.TerrainBrushRadius, config.TerrainStepAmount);
			return true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Wood, 10)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
