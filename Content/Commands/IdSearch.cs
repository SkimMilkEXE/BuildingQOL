using Terraria.ID;

namespace BuildingQOL.Content.Commands
{
	// TileID.Search/WallID.Search are case-sensitive and expect exact PascalCase internal names (e.g. "Dirt",
	// not "dirt"). This tries the raw input first, then falls back to capitalizing just the first letter, so
	// the common case of typing a name in lowercase still resolves.
	internal static class IdSearch
	{
		private delegate bool TryGetIdDelegate(string name, out int id);

		public static bool TryGetTileId(string name, out int id) => TryGet(TileID.Search.TryGetId, name, out id);
		public static bool TryGetWallId(string name, out int id) => TryGet(WallID.Search.TryGetId, name, out id);

		private static bool TryGet(TryGetIdDelegate lookup, string name, out int id)
		{
			if (lookup(name, out id))
				return true;

			if (name.Length > 0)
			{
				string capitalized = char.ToUpperInvariant(name[0]) + name.Substring(1);
				if (lookup(capitalized, out id))
					return true;
			}

			id = -1;
			return false;
		}
	}
}
