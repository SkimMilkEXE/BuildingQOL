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
	}
}
