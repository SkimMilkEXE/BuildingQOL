using Terraria.ModLoader;

namespace BuildingQOL.Content.Commands
{
	// /qolhelp - lists just this mod's commands, since vanilla /help mixes in everything else too.
	// Keep the Commands list below in sync whenever a command is added/removed.
	public class HelpCommand : ModCommand
	{
		public override CommandType Type => CommandType.Chat;
		public override string Command => "qolhelp";
		public override string Description => "Lists all BuildingQOL commands and what they do.";
		public override string Usage => "/qolhelp";

		private static readonly (string Usage, string Description)[] Commands =
		{
			("/blockswap <from> <to>", "Replace one tile/wall/liquid type with another inside your selection."),
			("/fill <block>", "Fill your entire selection with one tile/wall/liquid type."),
			("/drain", "Remove all liquid in your selection, leaving tiles/walls untouched."),
			("/tilename", "Reports the tile/wall/liquid ID names under your cursor."),
		};

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			caller.Reply("BuildingQOL commands:");
			foreach ((string usage, string description) in Commands)
				caller.Reply($"{usage} - {description}");
		}
	}
}
