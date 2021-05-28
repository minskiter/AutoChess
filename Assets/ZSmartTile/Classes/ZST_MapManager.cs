using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZST_MapManager : object {

	public static ZST_MapManager SharedInstance = new ZST_MapManager();

	static ZST_MapManager() {}
	private ZST_MapManager() {}

	private bool isMidUpdate;

	private HashSet<string> updateTags = new HashSet<string>();
	private HashSet<ZST_SmartTile> tiles = new HashSet<ZST_SmartTile>();

	public void AddUpdateTag(string updateTag) {
		updateTags.Add(updateTag);
	}

	public void AddTile(ZST_SmartTile tile) {
		tiles.Add(tile);
	}

	public void RemoveTile(ZST_SmartTile tile) {
		tiles.Remove(tile);
	}
		
	public void HardReloadTiles() {
		tiles = new HashSet<ZST_SmartTile>(GameObject.FindObjectsOfType<ZST_SmartTile>());
		foreach (ZST_SmartTile tile in tiles) {
			AddUpdateTag(tile.tileTag);
		}
	}

	public void Update() {

		// we're gonna block updating while in play mode
		if (Application.isPlaying) {return;}

		foreach (string updateTag in updateTags) {

			List<ZST_SmartTile> adjustableTiles = new List<ZST_SmartTile>();
			HashSet<ZST_SmartTile.Coord> occupiedCoords = new HashSet<ZST_SmartTile.Coord>();

			foreach (ZST_SmartTile tile in tiles) {

				if (tile.tileTag != null && !tile.tileTag.Equals(updateTag)) {continue;}

				tile.MarkAsMidUpdate();

				ZST_SmartTile.Coord coord = tile.GetCoord();
				occupiedCoords.Add(coord);

				adjustableTiles.Add(tile);
			}

			foreach (ZST_SmartTile tile in adjustableTiles) {

				ZST_SmartTile.Coord coord = tile.GetCoord();

				int row = coord.row;
				int col = coord.col;

				System.Array cardinalDirs = System.Enum.GetValues(typeof(ZST_SmartTile.CardinalDirection));
				foreach (ZST_SmartTile.CardinalDirection dir in cardinalDirs) {

					ZST_SmartTile.Coord offset = ZST_SmartTile.dirsToCoordOffsets[dir];

					tile.hasNeighbor[dir] = HasNeighbor(row + offset.row, col + offset.col, occupiedCoords);
				}

				tile.UpdateSprite();
			}
		}

		updateTags.Clear();

	}

	private bool HasNeighbor(int row, int col, HashSet<ZST_SmartTile.Coord> occupiedCoords) {
		ZST_SmartTile.Coord coord = new ZST_SmartTile.Coord(row,col);
		return occupiedCoords.Contains(coord);
	}

}
