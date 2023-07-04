using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGen : MonoBehaviour {

	[SerializeField]
    private Grid grid; //Main tile grid
	[SerializeField]
    private TileBase[] fgTiles, //Foreground tiles
						bgTiles; //Background tiles
	[SerializeField]
	private Tilemap oreMap, //Ore/Overlay tilemap
                    fgMap, //Foreground tilemap
                    bgMap; //Background tilemap
	//public GameObject plant; //An example game object 'plant' to test placing.

	[Header("Octave 1")] //Octave 1 is the most "zoomed in" one, and therefor the smoothest; defines the overall shape of the landscape.
	[SerializeField]
	private float seed; //Seed is the random number the noise starts from.
	[SerializeField]
	private float octave1Scale, //The scale, or how 'zoomed in' you are on the Perlin Noise, affects how smooth or bumpy the terrain is.
					bgOctave1Scale, //Scale but for the background layer.
					o1Ratio; //The ratio is the percentage of how much this octave affects the final result. All the ratios together have to add up to 1!
	private float o1Result;
    private float bgo1Result;

	[Header("Octave 2")] //Octave 2 is more "zoomed out", so perlin features are smaller and more hill/bump-like. Medium size for bumps and hills. Lower ratio than 1 so it's more an 'accent' or suggestion of features.
	[SerializeField]
	private float o2Seed;
    [SerializeField]
    private float octave2Scale, 
                    bgOctave2Scale, 
                    o2Ratio; 
    private float o2Result;
    private float bgo2Result;

	[Header("Octave 3")] //Octave 3: like 2 but even moreso. More lumpy, spiky, smaller features, etc.
	[SerializeField]
	private float o3Seed;
    [SerializeField]
    private float octave3Scale,
                    bgOctave3Scale,
                    o3Ratio;
    private float o3Result;
    private float bgo3Result;

    [Header("Height Adjuster")] //Kind of an octave but instead of altering the main height map, it affects the general height of the whole result at tile generation time.
	[SerializeField]
	private float heightASeed,
					heightAScale,
					heightMultiplier; //What we multiply the heightAResult by. Height Adjustment is the...elevation of the terrain, and not so much its general smaller features.
    private int heightAResult;
	
	[Header("Octave Crossfader")] //This is for adjusting the ratio of the existing octaves with MORE PERLIN NOISE 
	[SerializeField]
	private float crossfadeSeed,
				crossfadeScale,
				crossfadeRange; //How far the crossfade can push and pull the ratios of the octaves.
    private float crossfadeResult;

	[Header("Testing Environment")] //Stuff just for testing!
	[SerializeField]
	private int worldWidth; //Size of initial area to generate.
    [SerializeField]
    private int worldHeight;

    List<WorldChunk> activeChunks = new List<WorldChunk>(); //For keeping track of live/visible chunks

    private int surfaceY, //The working surface height of the location we're calculating.
				stoneY, //The height at which the stone layer begins.
				bgsurfaceY,
				bgstoneY;


    void Start () 
	{
		worldWidth = 10; //Orig Values were 2 and 5 for these
		worldHeight = 10;

		seed = Random.Range(1000f, 2000f);
		o2Seed = seed + 500f; //For reproduceability, the original seed should be the only random element; everything else should derive from it somehow
		o3Seed = seed + 100f;
		heightASeed = seed + 750f;
		crossfadeSeed = seed + 250f;

		octave1Scale = .005f;
		octave2Scale = .02f;
		octave3Scale = .1f;
		heightAScale = .002f;
		crossfadeScale = .003f;

        bgOctave1Scale = .005f;
        bgOctave2Scale = .02f;
        bgOctave3Scale = .1f;

        crossfadeRange = 2f;

		heightMultiplier = 100f;

		o1Ratio = 0.5f; //Again, these three ratios have to add up to 1.
		o2Ratio = 0.4f;
		o3Ratio = 0.1f;

		InitialWorldGeneration();
	}

	void InitialWorldGeneration() //For when you're first genning the world
	{
		for (int i = -(worldWidth); i < worldWidth; i++) //i here is the current chunkspace X coord being calculated. j is chunkspace Y.
		{
			for (int j = -(worldHeight); j < worldHeight; j++)
			{
				ChunkGen(i,j);
			}
		}
	}

	//TO DO: Actually explain how all of this malarky works in detail!
	//Caves, ores, biomes, plant/entity generation.
	//Try Async on chunkgen, possibly BurstCompile as well.

    //Feed ChunkGen the X, Y of the chunk and it feeds back the tiles to make in that chunk! //NOTE TO SELF: Review BurstCompile for some of these things
    public void ChunkGen(int chunkX, int chunkY) 
	{
		WorldChunk workingChunk = new WorldChunk(); //The chunk we're working on now.
        WorldChunk bgChunk = new WorldChunk();

		workingChunk.chunkPosition = new Vector2Int(chunkX, chunkY);

		int worldX; //For holding X value.
		int worldY;

		int arrayLoc = 0; //Where we are in our arrays of coords and tiles we'll be building.
        int bgarrayLoc = 0;

		for (int workingX = 0; workingX < 16; workingX++) //Working X/Y is the tile position inside the chunk.
		{
			worldX = (chunkX * 16) + workingX;

			for (int workingY = 0; workingY < 16; workingY++)
			{
				worldY = (chunkY * 16) + workingY;
                
                //All this is using the magical perlin noise to figure out where the surface level is.

                //Experimenting with subtracting 0.5 from the noise result so it can be either a negative or positive adjustment.
                crossfadeResult = (Mathf.PerlinNoise(((worldX * crossfadeScale) + crossfadeSeed), 0.1f) - 0.5f) * crossfadeRange;

                //Here's where we consult the magical Perlin Noise~. Then we enforce our ratios (modified by the crossfader) on the results.
                o1Result = (Mathf.PerlinNoise(((worldX * octave1Scale) + seed), 0.1f)) * (o1Ratio - crossfadeResult);
                //Crossfading: To make sure the numbers add up, the major and medium octaves get modified by the same number in opposite directions. MATH!
                o2Result = (Mathf.PerlinNoise(((worldX * octave2Scale) + o2Seed), 0.1f)) * (o2Ratio + crossfadeResult); 
				o3Result = (Mathf.PerlinNoise(((worldX * octave3Scale) + o3Seed), 0.1f)) * o3Ratio;
				heightAResult = (int)((Mathf.PerlinNoise(((worldX * heightAScale) + heightASeed), 0.1f)) * heightMultiplier);

                bgo1Result = (Mathf.PerlinNoise(((worldX * bgOctave1Scale) + seed), 0.1f)) * (o1Ratio - crossfadeResult);
                bgo2Result = (Mathf.PerlinNoise(((worldX * bgOctave2Scale) + seed), 0.1f)) * (o1Ratio - crossfadeResult);
                bgo3Result = (Mathf.PerlinNoise(((worldX * bgOctave3Scale) + seed), 0.1f)) * (o1Ratio - crossfadeResult);

                //Add the ratio'd results together, times 50 (because the results are just values between 0 and 1) and then add the heightadjust and drop the block height down some just 'cuz.
                surfaceY = ((int)((o1Result + o2Result + o3Result) * 50f)) + heightAResult - 100; 
                bgsurfaceY = ((int)((bgo1Result + bgo2Result + bgo3Result) * 50f)) + heightAResult - 100;

                if (worldY <= surfaceY) //No sense in recording anything if it's an empty block, right?
				{
                    //Record current X,Y position in chunk.
                    workingChunk.chunkTilePositions[arrayLoc] = new Vector3Int(worldX,(chunkY * 16) + workingY,0); 

					stoneY = surfaceY - 5; //Temporary solution!

                    //You're above stone but below surface, so it's dirt.
                    if (worldY > stoneY) 
					{
						workingChunk.chunkTiles[arrayLoc] = fgTiles[0];
					}
                    //Below stone line, so it's stone.
                    else if (worldY <= stoneY) 
					{
						workingChunk.chunkTiles[arrayLoc] = fgTiles[1];
					}
				}

                arrayLoc++;

                if (worldY <= surfaceY) //Same as above but for the background layer.
                {
                    bgChunk.chunkTilePositions[bgarrayLoc] = new Vector3Int(worldX, (chunkY * 16) + workingY, 0);

                    stoneY = surfaceY - 5;
                    if (worldY > stoneY)
                    {
                        bgChunk.chunkTiles[bgarrayLoc] = bgTiles[0];
                    }
                    else if (worldY <= stoneY)
                    {
                        bgChunk.chunkTiles[bgarrayLoc] = bgTiles[1];
                    }
                }

                bgarrayLoc++;
			}
		}

        fgMap.SetTiles(workingChunk.chunkTilePositions, workingChunk.chunkTiles);
        bgMap.SetTiles(bgChunk.chunkTilePositions, bgChunk.chunkTiles);

        activeChunks.Add(workingChunk);
    }

	//Comment goes here
    public void RemoveChunk(WorldChunk deletedChunk){
		foreach(Vector3Int tilePos in deletedChunk.chunkTilePositions) {
			fgMap.SetTile(tilePos, null);
            bgMap.SetTile(tilePos, null);
        }
    }
	
	//Comment goes here
	public void GenerateChunksFrom(int chunkX, int chunkY, int viewDistance)
	{
        List<Vector2Int> chunksToGen = new List<Vector2Int>();

        for (int cX = (chunkX - viewDistance); cX <= (chunkX + viewDistance); cX++)
        {
            for (int cY = (chunkY - viewDistance); cY <= (chunkY + viewDistance); cY++)
            {
				chunksToGen.Add(new Vector2Int(cX, cY));

				activeChunks.ForEach(delegate (WorldChunk testChunk)
				{
					if (testChunk.chunkPosition.x == cX && testChunk.chunkPosition.y == cY)
					{
						chunksToGen.Remove(new Vector2Int(cX, cY));
                    }
				});
			}
        }
        chunksToGen.ForEach(delegate (Vector2Int newChunk)
        {
			ChunkGen(newChunk.x, newChunk.y);
        });
		chunksToGen = null;
    }

	//Comment goes here
	public void CleanupChunks(int chunkX, int chunkY, int viewDistance){

        List<WorldChunk> chunksToKill = new List<WorldChunk>();

        activeChunks.ForEach(delegate (WorldChunk testChunk){
			if (testChunk.chunkPosition.x > (chunkX + viewDistance) || testChunk.chunkPosition.x < (chunkX - viewDistance) || testChunk.chunkPosition.y > (chunkY + viewDistance) || testChunk.chunkPosition.y < (chunkY - viewDistance)){
				RemoveChunk(testChunk);
				chunksToKill.Add(testChunk);
			}
		});

		chunksToKill.ForEach(delegate (WorldChunk testChunk)
		{
            activeChunks.Remove(testChunk);
        });

		chunksToKill = null;
    }

	// Using update here for testing!
	void Update () {
		
		if (Input.GetKeyDown("g")) //Press G to generate a new set of terrain.
		{
			InitialWorldGeneration();
		}

		if (Input.GetKeyDown("r")) //Press R to remove existing terrain.
		{
			fgMap.ClearAllTiles();
            bgMap.ClearAllTiles();
		}
		
	}
}