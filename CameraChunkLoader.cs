using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChunkLoader : MonoBehaviour
{
	Camera mainCamera; 
    private Vector2Int chunkCheck; //Current chunk the camera is in
    private Vector2Int cameraChunk; //Last recorded chunk the camera was in; when these two don't match, update what chunks are visible.

    [SerializeField]
    private int viewDistance; //How many chunks away from the main camera's chunk we can see. If '1', then we can see 1 chunk in any direction (which would be 9 chunks visible total, including the center one the camera is in.)

    private float tileSize; //The size of tiles relative to unity's native measurements, for calculating tiles to chunks, etc.

	[SerializeField]
	private GameObject worldGenObj; //The object holding our world gen stuff, so we can access...
    private WorldGen worldGenScript; //Our world gen script, so we can access the tasty functions inside it.

    // Start is called before the first frame update
    void Start()
    {
		viewDistance = 2;
		tileSize = (float)2.56;
        mainCamera = Camera.main;
		cameraChunk = Vector2Int.FloorToInt(new Vector2 ((mainCamera.transform.position.x / tileSize), (mainCamera.transform.position.y / tileSize))); //Divide position by tilesize to get your 'tile space' position
		worldGenScript = worldGenObj.GetComponent<WorldGen>();
    }

    // Update is called once per frame
    void Update()
    { //
		chunkCheck = Vector2Int.FloorToInt(new Vector2 ((mainCamera.transform.position.x / tileSize), (mainCamera.transform.position.y / tileSize)));
		if(cameraChunk != chunkCheck){ //Again, if the camera isn't in the same chunk it was previously, update what chunks are visible.
			cameraChunk = chunkCheck;
			worldGenScript.CleanupChunks(cameraChunk.x, cameraChunk.y, viewDistance);
            worldGenScript.GenerateChunksFrom(cameraChunk.x, cameraChunk.y, viewDistance);
        }
    }
} //TO DO: Make the chunk check only happen if the camera has moved, to save a little power.