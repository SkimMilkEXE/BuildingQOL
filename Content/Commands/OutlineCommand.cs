using System;
using BuildingQOL.Content.Selection;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Commands
{
	// /outline <block> - draws a 1-tile border of a tile, wall, or liquid around the edge of the selection
	public class OutlineCommand : ModCommand
	{
		public override CommandType Type => CommandType.Chat;
		public override string Command => "outline";
		public override string Description => "Places a 1-tile border of the given tile/wall/liquid around the edge of your selection.";
		public override string Usage => "/outline <block>  (e.g. /outline GrayBrick)";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (args.Length != 1)
			{
				caller.Reply("Usage: " + Usage);
				return;
			}

			if (SelectionSystem.Corner1 is not Point16 c1 || SelectionSystem.Corner2 is not Point16 c2)
			{
				caller.Reply("No selection set. Use [ and ] to set one first.");
				return;
			}

			int lastX = Math.Abs(c1.X - c2.X);
			int lastY = Math.Abs(c1.Y - c2.Y);

			if (FillCommand.TryFill(c1, c2, args[0], (x, y, _) => x == 0 || y == 0 || x == lastX || y == lastY, out string kind))
				caller.Reply($"Outlined selection with {kind} {args[0]}.");
			else
				caller.Reply(FillCommand.NoMatchMessage(args[0]));
		}
	}
}
