using BuildingQOL.Content.Selection;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Commands
{
	// /clear - erases tiles, walls, and liquid in the current selection (Erase + Drain combined).
	public class ClearCommand : ModCommand
	{
		public override CommandType Type => CommandType.Chat;
		public override string Command => "clear";
		public override string Description => "Erases tiles, walls, and liquid in your current selection.";
		public override string Usage => "/clear";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (SelectionSystem.Corner1 is not Point16 c1 || SelectionSystem.Corner2 is not Point16 c2)
			{
				caller.Reply("No selection set. Use [ and ] to set one first.");
				return;
			}

			SelectionSystem.Erase();
			RegionMutator.Apply(c1, c2, tile => tile.LiquidAmount > 0, tile => tile.LiquidAmount = 0);
			caller.Reply("Cleared tiles, walls, and liquid in selection.");
		}
	}
}
