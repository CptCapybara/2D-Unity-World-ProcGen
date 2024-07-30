using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// The heart of world generation, where the data is made; and just the data. 
/// Other endpoint scripts pull from this for various uses.
/// </summary>
public static class WorldGen
{
    //General World Info
    //World Size
    //"Sea level"/base terrain height
    //Basic Height Map
    //scale var (plays with/interacts with perlin noise? height-map rendering function?)
    //Pole/Equator
    //point for pole
    //point for equator (1/2 way across map)
    //Faults / tectonic boundaries
    //No-Go zone around pole
    //pick number of points (how?) - more common closer to equator
    //each point has a 'boundary type' -- divergent, convergent, and transform
    //Temperature
    //Precipitation
    //Bodies of Water?
    //Finer details???
    //Ore, Caves, Plants?

    //ULTIMATE GOALS: Function that can be called that returns final world height/data given any X point
    //Octave as class/object
    //Reading order for data from point: octaves platemap/patch
    //World crafting order: Pole/Equator, Plates, Boundary Type (Plate Map? 'Patches'/Areas with heigh hap shapes?)
    //Entities: Core World/Map Generator, Tile Printer/Generator, Texture/Image Printer

    //General height maps: "Zoom" agnostic. Internal variables - height to width ratio, ratio between octaves/effects. Need consistent horizontal measurement for world size/effects/etc. 
    //We want a use-agnostic height map

    /* To-Dos
    void DetermineElevation()
    {
        //iterate through every chunk column in the world and figure out and log the elevation of each
        //Starting at chunk X (negative world width / 2) and going to (world width / 2), get and log the height of the middle-ish of each chunk on the main height map.
        //Technically getting the average height of all the blocks in the chunk would be more accurate, but...
    }

    void DesignatePoleAndEquator()
    {
        //Pick a random X coordinate to designate as the pole, and the 'opposite' to be the equator. Possibly this should be stored in some 'world' variable/container.
        //Should this be/also contain 'Designate Temperatures'?
        //The pole is the 'coldest' chunk, and the equator is the 'warmest' -- probably values between 0 and 1 for now. With, of course, chunks between transitioning based on proximity to either.
    }

    void DesignatePlateTectonicModifiers()
    {
        //Pick a random number of zones or points (probably via a noise map, again), to be areas of plate activity that apply certain modifiers to local elevation and features.
        //Plates can't be near the pole; they're more commonplace closer to the equator. Designate X number of points/zones. Pick (somehow? randomly?) the plate activity type. Colliding plates raise terrain in a point/triangle. One plate sliding beneath another raises terrain on one side and lowers it on the other. Etc.
    }

    void DesignatePrecipitation()
    {
        //Use a separate noise map to randomly designate what areas have what precipitation (Probably set a value to each chunk)
        //Or come up with some kind of fancy system for designating a rough simulation (elevation and windward and leeward winds?)
    }

    void DesignateBiomes()
    {
        //Use the set data to decide biomes!
        //Biomes will need minimum sizes
        //And some way of designating oceans
    }
    */

    //Temporary World Variables
    public static Biome worldBiome = new Biome(
        new Octave(0.01f, 0.4f, 0f), 
        new Octave(0.04f, 0.2f, 10f), 
        new Octave(0.1f, 0.05f, 20f)
    );

    static Octave worldElevationMap = new Octave(0.005f, 1f, 40f); //Elevation is the biggest octave, and exists above the biome level.
    static float worldElevationMagnitude = 100f;
    static float seaLevel = -75f;
    static float worldTerrainMagnitude = 100f;

    /// <summary>
    /// Takes an octave and an x position, and gives you perlin data (between 0 and 1, unaltered)
    /// </summary>
    /// <param name="xCoord"></param>
    /// <param name="octave"></param>
    /// <returns></returns>
    public static float GetPerlin1D(float xCoord, Octave octave)
    {
        return Mathf.PerlinNoise1D((octave.octaveSeed + (xCoord * octave.mapScale)));
    }

    public static float GetHeightAt(float x, Octave octave1, Octave octave2, Octave octave3)
    {
        float octaveSum = CompositeHeightMaps(x, octave1, octave2, octave3) * worldTerrainMagnitude;
        float elevation = GetPerlin1D(x, worldElevationMap) * worldElevationMagnitude + seaLevel;
        return octaveSum + elevation;
    }

    public static float GetHeightAt(float x, Biome biome)
    {
        float octaveSum = CompositeHeightMaps(x, biome.octave1, biome.octave2, biome.octave3) * worldTerrainMagnitude;
        float elevation = GetPerlin1D(x, worldElevationMap) * worldElevationMagnitude + seaLevel;
        return octaveSum + elevation;
    }
    static float CompositeHeightMaps(float x, Octave octave1, Octave octave2, Octave octave3) {
        return (
                    (GetPerlin1D(x, octave1) * octave1.ratio) +
                    (GetPerlin1D(x, octave2) * octave2.ratio) +
                    (GetPerlin1D(x, octave3) * octave3.ratio)
        );
    }

    /// <summary>
    /// Returns if there's any terrain at X, Y...eventually will return an actual value.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool GetMaterialAt(float x, float y)
    {
        //locationBiome = Get biome at X, Y
        if (GetHeightAt(x, worldBiome) > y) { return true; }
        else { return false; }
    }
}