using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RiverChunkManager : MonoBehaviour {

	public static RiverChunkManager currentManager;

	public float FarPlaneDistance;
	public float BehindPlaneDistance;

	public int chunkWidth = 20, chunkDepth = 20, chunkHeight = 2, seed = 0;
	public MapGenerator chunkFab;
    public GameObject RiverBank;

	// Use this for initialization
	void Awake () 
	{
		currentManager = this;
		//if (seed == 0)
		//{
			seed = Random.Range (0, int.MaxValue);
		//}
	}

    public void ReStart()
    {
        transform.position = new Vector3(55, 0, 500);
        FindObjectOfType<FloatForce>().ResetPosition();
        FindObjectOfType<PlayerController>().RestSlider();
        seed = Random.Range(0, int.MaxValue);
       // MapGenerator.ResetList();
    }

    // Update is called once per frame
    void Update () 
	{
  
		for (float z = transform.position.z - chunkDepth; z < transform.position.z + FarPlaneDistance; z += chunkDepth) 
		{
			//Debug.Log ("Z:" + z);
			Vector3 pos = new Vector3 (0, 0, z);
			pos.z = Mathf.Floor (pos.z / (float)chunkDepth) * chunkDepth;
			//Debug.Log ("z:" + pos.z);
			MapGenerator chunk = MapGenerator.findChunk (pos);
			if (chunk != null) 
			{
			//	Debug.Log ("cZ:" + chunk.transform.position.z);
				continue;
			}

			chunk = (MapGenerator)Instantiate (chunkFab, pos, Quaternion.identity);
            GameObject rb = (GameObject)Instantiate(RiverBank, pos, Quaternion.identity);
		}
	
	}
}
