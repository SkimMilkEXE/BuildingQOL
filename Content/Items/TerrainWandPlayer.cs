using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Items
{
	public class TerrainWandPlayer : ModPlayer
	{
		public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
		{
			if (mediumCoreDeath)
				return Array.Empty<Item>();

			return new List<Item> { new Item(ModContent.ItemType<TerrainWand>()) };
		}

		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (Player.whoAmI != Main.myPlayer)
				return;

			if (TerrainWandSystem.CycleModeKeybind.JustPressed)
			{
				int modeCount = Enum.GetValues(typeof(TerrainMode)).Length;
				TerrainWandSystem.Mode = (TerrainMode)(((int)TerrainWandSystem.Mode + 1) % modeCount);
				Main.NewText($"Terrain Wand mode: {TerrainWandSystem.Mode}", Color.Cyan);
			}
		}
	}
}
