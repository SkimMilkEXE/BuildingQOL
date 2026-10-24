using System.ComponentModel;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.Config;

namespace BuildingQOL.Content.Selection
{
	// labels, tooltips and header text live in Localization/en-US_Mods.BuildingQOL.hjson
	public class BuildingQOLConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[Header("Selection")]
		[DefaultValue(typeof(Color), "0, 255, 255, 255")]
		public Color OutlineColor;

		[Range(1, 8)]
		[DefaultValue(2)]
		public int OutlineThickness;

		[Header("Paste")]
		[DefaultValue(true)]
		public bool AutoReframeOnPaste;

		[Header("TerrainWand")]
		[Range(1, 20)]
		[DefaultValue(4)]
		public int TerrainBrushRadius;

		[Range(1, 10)]
		[DefaultValue(3)]
		public int TerrainStepAmount;
	}
}
