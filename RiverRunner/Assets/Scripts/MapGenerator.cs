using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapGenerator : MonoBehaviour {
    public static int width {
        get { return RiverChunkManager.currentManager.chunkWidth; }
    }

    public static int depth {
        get { return RiverChunkManager.currentManager.chunkDepth; }
    }
    public static int height
    {
        get { return RiverChunkManager.currentManager.chunkHeight; }
    }

    public float StaringZone;
    public string seed;
    public bool useRandomSeed;
    public int SmoothIterations;

    public List<GameObject> cubeList;
    public static List<MapGenerator> chunkList = new List<MapGenerator>();



    public GameObject CubePrefab;
    public VoxelChunk vc;

    [Range(0, 100)]
    public int randomFillPercent;
    byte[,,] map;

    void Start()
    {
        chunkList.Add(this);
        map = new byte[width, height, depth];

        cubeList = new List<GameObject>();
      //  vc = CubePrefab.GetComponent<VoxelChunk>();
        StartCoroutine("GenerateMap");
    }

    public static void ResetList()
    {
        for(int i=0;i<chunkList.Count; i++)
        {
            Destroy(chunkList[i]);
        }
        chunkList.Clear();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < cubeList.Count; i++)
        {
            Destroy(cubeList[i].gameObject);
        }
        chunkList.Remove(this);
    }

    void Update()
    {

        //if (Input.GetMouseButtonDown(0))
        //{
        //    GameObject temp;
        //    do {
        //        temp = cubeList[0];
        //        cubeList.RemoveAt(0);
        //        Destroy(temp);

        //    }
        //    while (cubeList.Count > 0);
        //    GenerateMap();
        //}
    }

    IEnumerator GenerateMap()
    {
        RandomFillMap();
        yield return null;
        for (int i = 0; i < SmoothIterations; i++)
        {
            SmoothMap();
        }

        ApplyHeight();
        GameObject temp = (GameObject)Instantiate(CubePrefab, transform.position, Quaternion.identity);
        vc = temp.GetComponent<VoxelChunk>();
        vc.SetMap(map);
        yield return null;
    }

	void RandomFillMap()
	{
		if (useRandomSeed) 
		{
			seed = Time.time.ToString ();
		}

		System.Random psuedoRandom = new System.Random (seed.GetHashCode ());
        if (transform.position.z > StaringZone)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < depth; y++)
                {
                    //if (x == 0 || x == width - 1) //|| y == 0 || y == depth - 1) 
                    //{
                    //	map [x, y] = 1;
                    //} 
                    if (y < 5 || y >= depth - 4)
                    {
                        map[x, 0, y] = 0;
                    }
                    else
                    {
                        map[x, 0, y] = (psuedoRandom.Next(0, 100) < randomFillPercent) ? (byte)1 : (byte)0;
                    }
                }
            }
        }
        for (int y = 0; y < depth; y++)
        {
            map[0, 0, y] = 1;
            map[width - 1, 0, y] = 1;
        }
	}

	void SmoothMap()
	{
		int neighbourWallTiles;
		for (int x = 0; x < width; x++) 
		{
			for (int y = 0; y < depth; y++) 
			{
				neighbourWallTiles = GetSurroundingWallCount (x,0, y);
				if (neighbourWallTiles > 4) {
					map [x, 0, y] = 1;                    
				} 
				else if (neighbourWallTiles < 4) 
				{
					map [x, 0, y] = 0;
				}

			}
		}
	}

    void ApplyHeight()
    {
        int neighbourWallTiles;
        for (int y = 1; y < 2; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    neighbourWallTiles = GetSurroundingWallCount(x, y-1, z);
                    //if (neighbourWallTiles >= 7)
                    //{
                    //    map[x, 2, z] = 1;
                    //}
                    if (neighbourWallTiles >= 7)
                    {
                        map[x, y, z] = 1;
                    }

                }
            }
        }
    }

	int GetSurroundingWallCount(int gridX, int gridY, int gridZ)
	{
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) 
		{
			for (int neighbourY = gridZ - 1; neighbourY <= gridZ + 1; neighbourY++) 
			{
				if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < depth) 
				{
					if (neighbourX != gridX || neighbourY != gridZ) 
					{
						wallCount += map [neighbourX, gridY, neighbourY];
					} 
				}
				else 
				{
					wallCount++;
				}
			}
		}
		return wallCount;
	}

    public static MapGenerator findChunk(Vector3 pos)
    {
        for (int a = 0; a < chunkList.Count; a++)
        {
            Vector3 cpos = chunkList[a].transform.position;
            if ((pos.x < cpos.x) || (pos.z < cpos.z) || (pos.x >= cpos.x + width) || (pos.z >= cpos.z + depth))
            {
                continue;
            }

            return chunkList[a];
        }
        return null;
    }
    
    public byte[,] GetPushLayer()
    {
        byte[,] result = new byte[width, depth];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                result[i, j] = map[i, 0, j];
            }
        }

        return result;
    }

}
