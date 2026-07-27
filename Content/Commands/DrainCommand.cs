using BuildingQOL.Content.Selection;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Commands
{
	// /drain - removes all liquid in the current selection, leaving tiles/walls untouched.
	public class DrainCommand : ModCommand
	{
		public override CommandType Type => CommandType.Chat;
		public override string Command => "drain";
		public override string Description => "Removes all liquid in your current selection, leaving tiles/walls untouched.";
		public override string Usage => "/drain";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (SelectionSystem.Corner1 is not Point16 c1 || SelectionSystem.Corner2 is not Point16 c2)
			{
				caller.Reply("No selection set. Use [ and ] to set one first.");
				return;
			}

			RegionMutator.Apply(c1, c2, tile => tile.LiquidAmount > 0, tile => tile.LiquidAmount = 0);
			caller.Reply("Drained liquid in selection.");
		}
	}
}
