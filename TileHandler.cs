using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Unity.Burst.Intrinsics.X86;

/// <summary>
/// Retrieves data from WorldGen and turns it into digestable chunks, and then into tiles.
/// </summary>
public class TileHandler : MonoBehaviour
{
    [SerializeField]
    private Grid tileGrid;
    [SerializeField]
    private TileBase[] foregroundTiles; 
                       //backgroundTiles;
    [SerializeField]
    private Tilemap foregroundTilemap;
                    //oreTilemap,
                    //backgroundTilemap;

    List<WorldChunk> activeChunks = new List<WorldChunk>();

    void Start()
    {
        int testChunksHeight = 6;
        int testChunksWidth = 10;

        InitialTileGeneration(testChunksWidth, testChunksHeight);
    }

    void InitialTileGeneration(int startingChunksWidth, int startingChunksHeight)
    {
        for (int chunkspaceX = -(startingChunksWidth); chunkspaceX < startingChunksWidth; chunkspaceX++)
        {
            for (int chunkspaceY = -(startingChunksHeight); chunkspaceY < startingChunksHeight; chunkspaceY++)
            {
                ChunkGen(chunkspaceX, chunkspaceY);
            }
        }
    }

    void Update() {}

    /// <summary>
    /// Creates the data for a chunk, and all its tiles, polling from WorldGen,
    /// then 'prints' actual tiles you can run and play on.
    /// </summary>
    /// <param name="chunkX">Chunks have their own grid: a chunk with position x.1 y.1 is actually at world x.16 y.16.</param>
    /// <param name="chunkY"></param>
    public void ChunkGen(int chunkX, int chunkY)
    {
        WorldChunk workingChunk = new WorldChunk(); //The chunk we're working on now.
        //WorldChunk backgroundChunk = new WorldChunk();

        workingChunk.chunkPosition = new Vector2Int(chunkX, chunkY);

        int worldX,
            worldY;

        int arrayLoc = 0; //Note that chunk tile positions are handled as a 1-D 256 array, not a 2-D 16x16 array ... unsure if this is the right approach.

        for (int workingX = 0; workingX < 16; workingX++) //Working X/Y is the tile position inside the chunk.
        {
            worldX = (chunkX * 16) + workingX;

            for (int workingY = 0; workingY < 16; workingY++)
            {
                worldY = (chunkY * 16) + workingY;
                workingChunk.chunkTilePositions[arrayLoc] = new Vector3Int(worldX, worldY, 0);

                if (WorldGen.GetMaterialAt(worldX, worldY)) //No sense in recording anything if it's an empty block, right?
                {
                    workingChunk.chunkTiles[arrayLoc] = foregroundTiles[0];
                }

                arrayLoc++;
            }
        }

        foregroundTilemap.SetTiles(workingChunk.chunkTilePositions, workingChunk.chunkTiles); 
        activeChunks.Add(workingChunk);
    }

    private void RemoveChunk(WorldChunk deletedChunk)
    {
        foreach (Vector3Int tilePos in deletedChunk.chunkTilePositions)
        {
            foregroundTilemap.SetTile(tilePos, null);
            //backgroundTilemap.SetTile(tilePos, null);
        }
    }
}
