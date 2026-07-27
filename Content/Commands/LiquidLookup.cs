using System;
using Terraria.ID;

namespace BuildingQOL.Content.Commands
{
	// LiquidID has no Search dictionary like TileID/WallID do, so this is a small hand-rolled equivalent.
	internal static class LiquidLookup
	{
		private static readonly (string Name, int Id)[] Entries =
		{
			("water", LiquidID.Water),
			("lava", LiquidID.Lava),
			("honey", LiquidID.Honey),
			("shimmer", LiquidID.Shimmer),
		};

		public static bool TryGetId(string name, out int id)
		{
			foreach ((string Name, int Id) entry in Entries)
			{
				if (string.Equals(entry.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					id = entry.Id;
					return true;
				}
			}

			id = -1;
			return false;
		}

		public static string GetName(int id)
		{
			foreach ((string Name, int Id) entry in Entries)
			{
				if (entry.Id == id)
					return entry.Name;
			}

			return null;
		}
	}
}
