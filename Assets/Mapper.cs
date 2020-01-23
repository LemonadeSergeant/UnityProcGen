using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapper : MonoBehaviour
{

    public float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float scale,int seed)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];

        for (int zIndex = 0; zIndex < mapDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < mapWidth; xIndex++)
            {
                // calculate sample indices based on the coordinates and the scale
                float sampleX = xIndex / scale;
                float sampleZ = zIndex / scale;

                // generate noise value using PerlinNoise
                float noise = Mathf.PerlinNoise(seed + sampleX, seed + sampleZ);

                noiseMap[zIndex, xIndex] = noise;
            }
        }

        return noiseMap;
    }

    public float[,] GenerateNoiseMap(int mapDepth, int mapWidth, float scale, float[] weights, float[] scaleMult,int seed)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];
        for (int i = 0; i < weights.Length; i++)
        {
            float[,] tempNoise = GenerateNoiseMap(mapDepth, mapWidth, scale * scaleMult[i],seed);
            for (int zIndex = 0; zIndex < mapDepth; zIndex++)
            {
                for (int xIndex = 0; xIndex < mapWidth; xIndex++)
                {
                    noiseMap[zIndex, xIndex] = noiseMap[zIndex, xIndex] + tempNoise[zIndex, xIndex] * weights[i];
                }
            }
        }

        return noiseMap;
    }
}
