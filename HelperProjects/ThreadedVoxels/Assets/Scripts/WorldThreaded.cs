using UnityEngine;
using System.Collections;

public class WorldThreaded : MonoBehaviour {
	
	public static WorldThreaded currentWorld;
	public int chunkWidth = 20, chunkDepth = 20, chunkHeight = 20;
	public  int seed = 0;
	public float viewRange = 55;
	public float vertViewRange = 20;
	public double WaitToStart = 30000;
	
	//public ChunkScript_t chunkFab;
	public ChunkManager TheChunkManager;
	
	// Use this for initialization
	void Awake () {
		currentWorld = this;
		if (seed==0)
			seed = Random.Range (0,int.MaxValue);
		
		currentWorld = this;	
	}
	
	void Start()
	{
		//TheChunkManager = new ChunkManager();
		TheChunkManager.SetViewRanges(viewRange, vertViewRange);
		TheChunkManager.SetChunkDimentions(chunkWidth, chunkHeight, chunkDepth);
	}
	// Update is called once per frame
	void FixedUpdate () {
		float x,y,z;
		int a;
		Vector3 tempPos;
		Vector3 delta;
		
		
		TheChunkManager.UpdatePlayerPos(transform.position);
		
	}
}

