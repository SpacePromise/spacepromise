using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{

	public int depth = 20;
	public int scale = 10;
	
	public int width = 256;
	public int height = 256;
	private Terrain terrain;

	// Use this for initialization
	void Start ()
	{
		this.terrain = this.GetComponent<Terrain>();

		terrain.terrainData = this.GenerateTerrain(this.terrain.terrainData);
	}

	private TerrainData GenerateTerrain(TerrainData terrainData)
	{
		terrainData.heightmapResolution = width + 1;
		
		terrainData.size = new Vector3(width, depth, height);
		terrainData.SetHeights(0, 0, this.GenerateHeights());

		return terrainData;
	}

	private float[,] GenerateHeights()
	{
		float[,] heights = new float[width,height];
		
		for (int yIndex = 0; yIndex < height; yIndex++)
		{
			for (int index = 0; index < width; index++)
			{
				heights[index, yIndex] = this.CalculateHeight(index, yIndex);
			}
		}

		return heights;
	}

	private float CalculateHeight(int x, int y)
	{
		var xcord = (float)x / height * scale;
		var ycord = (float)y / height * scale;

		return Mathf.PerlinNoise(xcord, ycord);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
