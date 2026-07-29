using Terraria.ModLoader;

namespace BuildingQOL.Content.Items
{
	public class TerrainWandSystem : ModSystem
	{
		public static TerrainMode Mode = TerrainMode.Raise;
		public static ModKeybind CycleModeKeybind;

		public override void Load()
		{
			CycleModeKeybind = KeybindLoader.RegisterKeybind(Mod, "Cycle Terrain Wand Mode", "M");
		}

		public override void Unload()
		{
			CycleModeKeybind = null;
		}
	}
}
