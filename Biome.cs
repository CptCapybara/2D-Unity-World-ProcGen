using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data type: Biomes
/// </summary>
public class Biome
{
    //Tiles
    //WorldGen Rules

    //Octave Ratios
    [SerializeField]
    public Octave octave1 { get; set; }
    [SerializeField]
    public Octave octave2 { get; set; }
    [SerializeField]
    public Octave octave3 { get; set; }

    public Biome(Octave newOctave1, Octave newOctave2, Octave newOctave3)
    {
        octave1 = newOctave1;
        octave2 = newOctave2;
        octave3 = newOctave3;   
    }
}
