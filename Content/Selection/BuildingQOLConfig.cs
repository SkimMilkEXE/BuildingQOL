using System.ComponentModel;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Config;

namespace BuildingQOL.Content.Selection
{
	public class BuildingQOLConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[DefaultValue(typeof(Color), "0, 255, 255, 255")]
		public Color OutlineColor;

		[Range(1, 8)]
		[DefaultValue(2)]
		public int OutlineThickness;

		[DefaultValue(true)]
		[Label("Auto-reframe on paste")]
		[Tooltip("Recalculates tile/wall visuals in and around the pasted area so it blends with neighbors")]
		public bool AutoReframeOnPaste;

		[Range(1, 20)]
		[DefaultValue(4)]
		[Label("Terrain Wand brush radius")]
		[Tooltip("How many columns on each side of your cursor the Terrain Wand affects per use")]
		public int TerrainBrushRadius;

		[Range(1, 10)]
		[DefaultValue(3)]
		[Label("Terrain Wand step amount")]
		[Tooltip("How many tiles Raise/Lower shifts terrain by per use")]
		public int TerrainStepAmount;
	}
}
