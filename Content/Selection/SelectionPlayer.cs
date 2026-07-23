using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Selection
{
	public class SelectionPlayer : ModPlayer
	{
		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (Player.whoAmI != Main.myPlayer)
				return;

			var targetTile = new Point16((int)(Main.MouseWorld.X / 16), (int)(Main.MouseWorld.Y / 16));

			if (SelectionSystem.SetCorner1Keybind.JustPressed)
				SelectionSystem.Corner1 = targetTile;

			if (SelectionSystem.SetCorner2Keybind.JustPressed)
				SelectionSystem.Corner2 = targetTile;

			if (SelectionSystem.ClearSelectionKeybind.JustPressed)
			{
				SelectionSystem.Corner1 = null;
				SelectionSystem.Corner2 = null;
			}

			if (SelectionSystem.EraseSelectionKeybind.JustPressed)
				SelectionSystem.Erase();

			if (ClipboardSystem.CopyKeybind.JustPressed)
				ClipboardSystem.Copy();

			if (ClipboardSystem.PasteKeybind.JustPressed)
				ClipboardSystem.Paste(targetTile);

			if (GridSystem.ToggleKeybind.JustPressed)
				GridSystem.Enabled = !GridSystem.Enabled;
		}
	}
}
