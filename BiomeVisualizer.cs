using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

/// <summary>
/// Biome Visualizer: For testing different biome octave values in the comfort of the editor!
/// </summary>
public class BiomeVisualizer : EditorWindow
{
    //Main To-Do here: The ability to conveniently export values as a prefab or data set, to be used in world generation.
    //Also: Some way to indicate scale in visualizer; probably a 'human-sized' rectangle that scales with zoom.
    Texture2D mapImage;

    float zoomLevel = 1f;
    int worldX = 0;
    int heightOffset = 256;

    bool animate = false;
    int scrollSpeed = 1;
    float animationInterval = 0.25f;
    float lastAnimTime;

    Octave octave1 = new Octave(0.01f, 0.4f, 0f);
    Octave octave2 = new Octave(0.04f, 0.2f, 10f);
    Octave octave3 = new Octave(0.1f, 0.05f, 20f);

    Octave elevation = new Octave(0.005f, 1f, 40f);

    [MenuItem("Window/Biome Visualizer")]
    static void Init()
    {
        var window = GetWindow<BiomeVisualizer>("Biome Visualizer");
        window.position = new Rect(100, 100, 1044, 700);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Biome Visualizer", EditorStyles.boldLabel);

        if (GUILayout.Button("Visualize"))
        {
            DrawHeightMap();
            lastAnimTime = Time.realtimeSinceStartup;
        }

        if (mapImage)
        {
            EditorGUI.DrawPreviewTexture(new Rect(10, 60, mapImage.width, mapImage.height), mapImage);
        }

        animate = EditorGUILayout.Toggle("Animate", animate);
        if (animate) //This isn't working well -- To Do: Replace with buttons that scroll screen to left/right.
        {
            if (Time.realtimeSinceStartup >= lastAnimTime + animationInterval)
            {
                worldX += scrollSpeed;
                DrawHeightMap();
                lastAnimTime = Time.realtimeSinceStartup;
            }
        }

        //Visualizer Octave Control Variables
        octave1.mapScale = EditorGUI.FloatField(new Rect(10, 582, 200, 20), "Octave1 Zoom", octave1.mapScale);
        octave2.mapScale = EditorGUI.FloatField(new Rect(10, 612, 200, 20), "Octave2 Zoom", octave2.mapScale);
        octave3.mapScale = EditorGUI.FloatField(new Rect(10, 642, 200, 20), "Octave3 Zoom", octave3.mapScale);
        elevation.mapScale = EditorGUI.FloatField(new Rect(10, 672, 200, 20), "Elevation Zoom", elevation.mapScale);

        octave1.ratio = EditorGUI.FloatField(new Rect(220, 582, 200, 20), "Octave1 Ratio", octave1.ratio);
        octave2.ratio = EditorGUI.FloatField(new Rect(220, 612, 200, 20), "Octave2 Ratio", octave2.ratio);
        octave3.ratio = EditorGUI.FloatField(new Rect(220, 642, 200, 20), "Octave3 Ratio", octave3.ratio);
        elevation.ratio = EditorGUI.FloatField(new Rect(220, 672, 200, 20), "Elevation Ratio", elevation.ratio);

        zoomLevel = EditorGUI.FloatField(new Rect(430, 672, 200, 20), "Zoom Level", zoomLevel);
    }

    void DrawHeightMap()
    {
        this.mapImage = new Texture2D(1024, 512, TextureFormat.RGBA32, false);
        for (int drawX = 0; drawX < 1024; drawX++)
        {
            float heightResult = WorldGen.GetHeightAt(worldX + (drawX / zoomLevel), octave1, octave2, octave3);
            heightResult *= zoomLevel;
            heightResult += heightOffset;

            this.mapImage.SetPixel(drawX, (int)heightResult, Color.black);
            for (int belowGroundPixels = (int)heightResult - 1; belowGroundPixels >= 0; belowGroundPixels--)
            {
                this.mapImage.SetPixel(drawX, belowGroundPixels, Color.black);
            }
        }
        this.mapImage.Apply();
    }
}
