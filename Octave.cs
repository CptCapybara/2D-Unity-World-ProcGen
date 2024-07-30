using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The data necessary for a single heightmap, with variables to get different shapes of terrain. 
/// Called an 'Octave' because it represents one level of multiples that overlap to create an interesting heightmap.
/// A finer/zoomed in map will produce large smooth curves, like a low C note. A 'zoomed out' map will show all the bumps in the noise, like a high C note.
/// </summary>
public struct Octave
{
    public float mapScale; //The 'zoom' or 'amplitude' of this heightmap
    public float ratio; //The ratio of this octave, as compared to the other octaves; normally all add up to 1.
    public float octaveSeed; //Offset from the world seed
    public Octave(float newMapScale, float newRatio, float newOctaveSeed)
    {
        mapScale = newMapScale;
        ratio = newRatio;
        octaveSeed = newOctaveSeed;
    }
}
