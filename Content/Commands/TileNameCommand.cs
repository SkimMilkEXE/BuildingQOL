using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Commands
{
	// /tilename - reports the tile/wall/liquid internal ID names under the cursor, for use with /blockswap.
	public class TileNameCommand : ModCommand
	{
		public override CommandType Type => CommandType.Chat;
		public override string Command => "tilename";
		public override string Description => "Reports the tile, wall, and liquid ID names under your cursor.";
		public override string Usage => "/tilename";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			int x = Player.tileTargetX;
			int y = Player.tileTargetY;
			if (!WorldGen.InWorld(x, y))
			{
				caller.Reply("No valid tile under cursor.");
				return;
			}

			Tile tile = Main.tile[x, y];

			string tileName = tile.HasTile && TileID.Search.TryGetName(tile.TileType, out string tName) ? tName : "none";
			string wallName = tile.WallType != 0 && WallID.Search.TryGetName(tile.WallType, out string wName) ? wName : "none";
			string liquidName = tile.LiquidAmount > 0 ? LiquidLookup.GetName(tile.LiquidType) ?? "unknown" : "none";

			caller.Reply($"Tile: {tileName}   Wall: {wallName}   Liquid: {liquidName}");
		}
	}
}
