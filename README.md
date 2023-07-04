# 2D-Unity-World-ProcGen
A simple set of C# scripts that generates a 2D world using perlin noise and Unity's built-in tileset systems.

Hey! Here's my world generation scripts; currently the results are very simple, but will hopefully lay the ground for something more comprehensive later on.

WorldGen.cs - The script that does all the work of generating 2D tile based terrain; right now it uses multiple sets of perlin noise at different scales to generate a basic 2D height map. Worlds are divided into 16x16 tile chunks, which this script has the functions for generating, cleaning up, etc.

WorldChunk.cs - Class definition for our chunks!

CameraChunkLoader - Simple script to drop on your main Camera (or wherever). Keeps track of where the camera is and handles loading/unloading chunks around it as it moves.


To Get Started:

-Bring these scripts into Unity.

-Make a grid for tilemaps.

-On said grid, make tilemaps for: Foreground tiles, Background tiles, and Ore 'tiles'

-Make an empty WorldGen object in your world, drag and drop the Worldgen script into it,

-Drag and drop your grid and tilemaps into the matching fields at the top of the WorldGen component.

-Similarly give your FG & BG tile fields one tile for the top layer and a second for your stone layer.

-Now drag and drop the CameraChunkLoader onto an existant gameObject; I did so on the main camera itself.

-In the CameraChunkLoader script component, drag and drop your WorldGen object.
	
 -Note: You may need to manually set your tilesize in the camera chunk list; it needs to know how big your tiles are in Unity-measurements to measure distance to create new blocks.

-Press play and test! You should be able to drag the main camera around (or attach it to a player character!) and watch the world generate and be destroyed as you do so!
