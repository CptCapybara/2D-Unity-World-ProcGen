# 2D-Unity-World-ProcGen
A simple set of C# scripts that generates a 2D world using perlin noise and Unity's built-in tileset systems.

Hey! Here's my world generation scripts; currently the results are very simple, but will hopefully lay the ground for something more comprehensive later on.

WorldGen.cs - A static script with all the math for making terrain data.

TileHandler.cs - Drop this on a game object and point it to the resources it needs, and it'll pull data from WorldGen to make tile-based terrain!

Biome.cs - Class definition for biomes -- which at this time are just a collection of octaves that define how the terrain generates.

Octave.cs - Class definition for octaves -- data that defines the details of one heightmap (of which our terrain map is comprised of four or more, overlayed to make interesting combinations).

WorldChunk.cs - Class definition for our chunks! Tiles, and their positions, etc. Currently formatted to work with Unity's tile system.

BiomeVisualizer.cs - An editor script that helps visualize terrain generation, right from the comfort of your Unity editor (Under >Window)! Freely edit the octave variables and click visualize to quickly and easily test 'em out!


To Get Started:

-Bring these scripts into Unity.

-Make a grid for tilemaps.

-On said grid, make a Foreground tilemap.

-Drag and drop your TileHandler script onto your grid (or any item in the scene, really). Designate in it your tilemap, tile grid, and one tile (a regular tile or a fancy rule tile -- your pick.)

-Press play and check our your tile-based terrain! Or just Hit > Window > Biome Visualizer.


I have a lot on my to-do list for this, so please keep an eye out for updates!
