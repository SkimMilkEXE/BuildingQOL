using System;
using BuildingQOL.Content.Selection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Commands
{
	// /hollow - removes the blocks inside the selection, keeping a 1-tile shell around the edge.
	// furniture and walls are left alone so hollowing a room doesn't wreck what's in it
	public class HollowCommand : ModCommand
	{
		public override CommandType Type => CommandType.Chat;
		public override string Command => "hollow";
		public override string Description => "Removes the blocks inside your selection, keeping a 1-tile shell around the edge.";
		public override string Usage => "/hollow";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (SelectionSystem.Corner1 is not Point16 c1 || SelectionSystem.Corner2 is not Point16 c2)
			{
				caller.Reply("No selection set. Use [ and ] to set one first.");
				return;
			}

			int lastX = Math.Abs(c1.X - c2.X);
			int lastY = Math.Abs(c1.Y - c2.Y);
			if (lastX < 2 || lastY < 2)
			{
				caller.Reply("Selection is too small to hollow - it needs to be at least 3x3.");
				return;
			}

			int count = RegionMutator.Apply(c1, c2,
				(x, y, tile) => x > 0 && y > 0 && x < lastX && y < lastY
					&& tile.HasTile && !Main.tileFrameImportant[tile.TileType], // skips furniture, torches, etc.
				tile =>
				{
					tile.HasTile = false;
					tile.IsHalfBlock = false;
					tile.Slope = SlopeType.Solid;
				});
			caller.Reply($"Hollowed out {count} block(s) in selection.");
		}
	}
}
