using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldChunk 
{ //The SetTiles function needs an array of Vector3Ints and Tiles to do its duty, so that's what chunks have!
	//Biome stuff may be stored here some day. Pulls from XML biome data to populate?

	public Vector2Int chunkPosition = new Vector2Int(); //Position of the chunk itself

	public Vector3Int[] chunkTilePositions = new Vector3Int[256]; //World positions for tiles in chunk

	public TileBase[] chunkTiles = new TileBase[256]; //Actual tiles of the chunk

}
