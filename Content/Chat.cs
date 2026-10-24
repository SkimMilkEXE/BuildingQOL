using Microsoft.Xna.Framework;
using Terraria;

namespace BuildingQOL.Content
{
	// one place for chat messages so every command and keybind looks the same: prefix + green/red/blue
	internal static class Chat
	{
		private const string Prefix = "[BuildingQOL] ";
		private static readonly Color SuccessColor = new(120, 230, 120);
		private static readonly Color ErrorColor = new(255, 110, 90);
		private static readonly Color InfoColor = new(140, 200, 255);

		public static void Success(string message) => Main.NewText(Prefix + message, SuccessColor);
		public static void Error(string message) => Main.NewText(Prefix + message, ErrorColor);

		// withPrefix false is for list lines under a header, like /qolhelp
		public static void Info(string message, bool withPrefix = true) => Main.NewText((withPrefix ? Prefix : "") + message, InfoColor);
	}
}
