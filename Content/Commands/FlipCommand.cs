using BuildingQOL.Content.Selection;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Commands
{
	// /flip <h|v> - mirrors the clipboard before pasting
	public class FlipCommand : ModCommand
	{
		public override CommandType Type => CommandType.Chat;
		public override string Command => "flip";
		public override string Description => "Flips the clipboard horizontally (h) or vertically (v).";
		public override string Usage => "/flip <h|v>";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			string direction = args.Length == 1 ? args[0].ToLowerInvariant() : "";
			switch (direction)
			{
				case "h":
				case "horizontal":
					ClipboardSystem.Flip(horizontal: true);
					break;
				case "v":
				case "vertical":
					ClipboardSystem.Flip(horizontal: false);
					break;
				default:
					Chat.Error("Usage: " + Usage);
					break;
			}
		}
	}
}
