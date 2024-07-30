using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Stores data about a 16x16 tile "chunk" of the world.
/// </summary>
public class WorldChunk 
{ //The SetTiles function needs a 1-D array of Vector3Ints and Tiles, so that's what chunks have!
	public Vector2Int chunkPosition = new Vector2Int(); //Note that 'chunk space' is kind of its own grid; Chunk 1, 3 is at x.16 y.48 in world space.

	public Vector3Int[] chunkTilePositions = new Vector3Int[256]; //World positions for tiles in chunk

	public TileBase[] chunkTiles = new TileBase[256]; //Actual tiles of the chunk
}
