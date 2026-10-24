using BuildingQOL.Content.Selection;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Commands
{
	// /schem save|load|delete <name>, /schem list - named schematic files instead of the single s/l slot
	public class SchematicCommand : ModCommand
	{
		public override CommandType Type => CommandType.Chat;
		public override string Command => "schem";
		public override string Description => "Saves, loads, lists, or deletes named schematic files.";
		public override string Usage => "/schem save <name>, /schem load <name>, /schem delete <name>, /schem list";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			string action = args.Length > 0 ? args[0].ToLowerInvariant() : "";

			if (action == "list" && args.Length == 1)
			{
				string[] names = ClipboardSystem.ListNames();
				caller.Reply(names.Length == 0
					? "No saved schematics yet. Copy a selection, then /schem save <name>."
					: $"Saved schematics ({names.Length}): {string.Join(", ", names)}");
				return;
			}

			if (args.Length != 2 || action is not ("save" or "load" or "delete"))
			{
				caller.Reply("Usage: " + Usage);
				return;
			}

			string name = args[1];
			if (!ClipboardSystem.IsValidName(name))
			{
				caller.Reply($"'{name}' isn't a valid schematic name. Use letters, numbers, - or _.");
				return;
			}

			switch (action)
			{
				case "save":
					ClipboardSystem.SaveToFile(name);
					break;
				case "load":
					ClipboardSystem.LoadFromFile(name);
					break;
				case "delete":
					caller.Reply(ClipboardSystem.Delete(name) ? $"Deleted schematic '{name}'." : $"No saved schematic named '{name}'.");
					break;
			}
		}
	}
}
