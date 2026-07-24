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

			var targetTile = new Point16(Terraria.Player.tileTargetX, Terraria.Player.tileTargetY);

			if (SelectionSystem.SetCorner1Keybind.JustPressed)
			{
				SelectionSystem.Corner1 = targetTile;
				SelectionSystem.Corner2 = null;
			}

			if (SelectionSystem.SetCorner2Keybind.JustPressed)
				SelectionSystem.Corner2 = targetTile;

			if (SelectionSystem.ClearSelectionKeybind.JustPressed)
			{
				SelectionSystem.Corner1 = null;
				SelectionSystem.Corner2 = null;
				ClipboardSystem.Clear();
			}

			if (SelectionSystem.EraseSelectionKeybind.JustPressed)
				SelectionSystem.Erase();

			if (ClipboardSystem.CopyKeybind.JustPressed)
				ClipboardSystem.Copy();

			if (ClipboardSystem.PasteKeybind.JustPressed)
				ClipboardSystem.Paste(targetTile);

			if (ClipboardSystem.SaveKeybind.JustPressed)
				ClipboardSystem.SaveToFile();

			if (ClipboardSystem.LoadKeybind.JustPressed)
				ClipboardSystem.LoadFromFile();

			if (GridSystem.ToggleKeybind.JustPressed)
				GridSystem.Enabled = !GridSystem.Enabled;

			if (SelectionSystem.ToggleCursorHighlightKeybind.JustPressed)
				SelectionSystem.CursorHighlightEnabled = !SelectionSystem.CursorHighlightEnabled;

			if (UndoSystem.UndoKeybind.JustPressed)
				UndoSystem.Undo();

			if (UndoSystem.RedoKeybind.JustPressed)
				UndoSystem.Redo();
		}
	}
}
