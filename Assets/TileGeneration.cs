using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGeneration : MonoBehaviour
{

    [SerializeField]
    float[] weights;
    [SerializeField]
    float[] scaleMult;

    [SerializeField]
    Mapper noiseMapGeneration;

    [SerializeField]
    private MeshRenderer tileRenderer;

    [SerializeField]
    private float mapScale;

    public float waterHeight = 0.1f, sandHeight = 0.3f, stoneHeight = 0.4f, grassHeight = 0.7f, snowHeight =0.9f, forest;
    public int seed = 0,xOffset=0;
    public Texture2D mask;
    void Start()
    {
        GenerateTile();
        time = Time.time;
    }
    float time;
    void FixedUpdate()
    {
        if (time + 0.5f < Time.time)
        {
            GenerateTile();
            time = Time.time;
        }

    }
    public int tileDepth, tileWidth;
    void GenerateTile()
    {


        // calculate the offsets based on the tile position
        float[,] heightMap = this.noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, this.mapScale,weights, scaleMult,seed);
        float[,] MoisturetMap = this.noiseMapGeneration.GenerateNoiseMap(tileDepth, tileWidth, this.mapScale, weights, scaleMult, seed+1);

        // generate a heightMap using noise
        Texture2D tileTexture = BuildTexture(heightMap, MoisturetMap);
        this.tileRenderer.material.mainTexture = tileTexture;
    }

    private Texture2D BuildTexture(float[,] heightMap, float[,] MoisturetMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Color[] colorMap = new Color[tileDepth * tileWidth];
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                // transform the 2D map index is an Array index
                int colorIndex = zIndex * tileWidth + xIndex;

                float height = heightMap[zIndex, xIndex] * mask.GetPixel(xIndex * mask.width / tileWidth, zIndex * mask.height / tileDepth).a;
                float moisture = MoisturetMap[zIndex, xIndex] * mask.GetPixel(xIndex * mask.width / tileWidth, zIndex * mask.height / tileDepth).a;
                // assign as color a shade of grey proportional to the height value
                if (height < waterHeight) colorMap[colorIndex] = Color.Lerp(new Color(0,0,0.8f),new Color(0,0,1f),height);
                else if (height < sandHeight) colorMap[colorIndex] = Color.Lerp(new Color(0.8f, 06f, 0.016f), new Color(1, 0.92f, 0.016f), height);
                else if (height < grassHeight) 
                {
                    if(moisture > forest)
                    colorMap[colorIndex] = Color.Lerp(new Color(0,1f,0), new Color(0, 0.7f, 0f), height);
                    else
                    colorMap[colorIndex] = Color.Lerp(new Color(0, 0.5f, 0), new Color(0, 0.3f, 0f), moisture);
                }
                else if (height < snowHeight) colorMap[colorIndex] = Color.Lerp(new Color(0.6f, 0.6f, 0.6f), new Color(1, 1, 1f), height);

                else
                    colorMap[colorIndex] = Color.cyan;
            }
        }

        // create a new texture and set its pixel colors
        Texture2D tileTexture = new Texture2D(tileWidth, tileDepth);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();

        return tileTexture;
    }
}
