using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour {

	public static PriorityQueue<ChunkScript_t> chunksLoadList = new PriorityQueue<ChunkScript_t> ();
	//public static PriorityQueue<ChunkScript_t> chunksNeedTrees = new PriorityQueue<ChunkScript_t>();
	//public static Queue<ChunkScript_t> chunksLoadList = new Queue<ChunkScript_t>();
	public static List<ChunkScript_t> chunksRenderList = new List<ChunkScript_t>();
	public static List<ChunkScript_t> chunksUnloadList = new List<ChunkScript_t>();
	public static Dictionary<string, ChunkScript_t> chunksVisibilityList = new Dictionary<string, ChunkScript_t>();
	public static List<ChunkScript_t> chunksSetupList = new List<ChunkScript_t>();
	public  ChunkScript_t chunkFab;
	public GameObject TreePrefab;

	private static Vector3 offset0;
	private static Vector3 offset1;
	private static Vector3 offset2;
	private static Vector2 offset3;
	
	private static int theSeed;
	private static float r1,r2,r3,r4,r5,r6,r7,r8,r9;
	public float vertViewRange;
	public float viewRange;
	public int chunkWidth;
	public int chunkHeight, chunkDepth;
	
	public bool loadingCube;
	public float ChunkCalcDelay= 0.5f;
	public float ChunkCurrentDelay =0;
	
	
	// Use this for initialization
	void Start () {
		theSeed = WorldThreaded.currentWorld.seed;
		Random.seed = theSeed;
		offset0 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
		offset1 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
	 	offset2 = new Vector3(Random.value * 10000, Random.value * 10000, Random.value * 10000);
		offset3 = new Vector2 (Random.value * 10000, Random.value * 10000);
	 	
	 	//loadingCube = false;
	}
	
	// Update is called once per frame
	public virtual IEnumerator TreePlanter(ChunkScript_t chunk)
	{
		Ray findGround = new Ray ();
		findGround.direction =  Vector3.up;
		findGround.origin = chunk.gameObject.transform.position + Vector3.up;
		RaycastHit hits;
		float treeScale;

		while (!chunk.Displayed) 
		{
			yield return 0;
		}

		////if (Physics.Raycast(findGround, out hits, chunkHeight))
		//if (Physics.SphereCast(findGround, chunkHeight,out hits))
		//{ // use it to update myNormal and isGrounded
		//	GameObject tempTree = (GameObject) Instantiate(TreePrefab, hits.point, Quaternion.identity);
		//	tempTree.transform.Rotate(new Vector3(0,Random.Range(0,359),0));

		//	//tempTree.transform..  localScale(new Vector3(treeScale,treeScale,treeScale));
		//}

		yield return 0;
	}

	void FixedUpdate () 
	{	
		int i;
		List<int> indexesToRemove = new List<int>();	
		if (chunksSetupList.Count > 0)
		{
			//for(i=chunksSetupList.Count-1; i>=0; i--)
			//for (i=0;i<chunksSetupList.Count;i++)
			//{
			//if ((!chunksSetupList[0].CalculatingMesh)&&(!chunksSetupList[0].Displayed))
			List<ChunkScript_t> tempList = new List<ChunkScript_t>();
			//for(i=chunksSetupList.Count-1;i>=0;i--)
			//Debug.Log ("Setup Count: " + chunksSetupList.Count);
			for(i=0;i<chunksSetupList.Count;i++)
			{
				if(chunksSetupList[i].CheckIfCalculated())
				{
					StartCoroutine(chunksSetupList[i].LoadVisualMeshForDisplay());
					//chunksNeedTrees.Enqueue((float)i,chunksSetupList[i]);
				//	chunksSetupList.RemoveAt(i);							
				}
				else
				{
					tempList.Add(chunksSetupList[i]);
					////Debug.Log("Not Ready to show mesh yet");
				}
			}
			//}
			chunksSetupList  = tempList;
			loadingCube = false;		
		}

		ChunkScript_t tempChunk;
		ChunkScript_t tempChunk2;
		//if (chunksSetupList.Count >0)
		//{

		//if (!loadingCube) 
		ChunkCurrentDelay += Time.deltaTime;
		if (ChunkCurrentDelay > ChunkCalcDelay)
		{
			ChunkCurrentDelay = 0;
			loadingCube = true;
						
			for(i=0;i<Mathf.Min(4,chunksLoadList.Count());i++)
			{
			//	if (chunksLoadList.Count > 0) 
			//	{				

				tempChunk = chunksLoadList.Deqeue();
				//chunksLoadList.RemoveAt(0);
				StartCoroutine (tempChunk.CalculateMapFromScratch ());
				chunksSetupList.Add (tempChunk);		
			}

		}
	}


	public void SetChunkDimentions(int width, int height, int depth)
	{
		chunkWidth = width;
		chunkHeight = height;
		chunkDepth = depth;
	}
	
	public void SetViewRanges(float hor, float vert)
	{
		viewRange = hor;
		vertViewRange = vert;
	}
	
	public void UpdatePlayerPos(Vector3 playerPos)
	{
		int x,y,z;
		Vector3 delta;
		Vector3 currentPosition;
		Vector3 tempPosition;
		Queue<Vector3> FloodQueue = new Queue<Vector3>();
		List<Vector3> ClosedList = new List<Vector3>();
		
		int breakCount =0;

        List<string> removeList = new List<string>();
        foreach(var c in chunksVisibilityList)
        {
            float tempDist = (c.Value.transform.position - playerPos).magnitude;
            if (tempDist > viewRange*1.5f)
            {
                removeList.Add(c.Key);
            }
        }

        ChunkScript_t tempChunk;
        for (x=0;x<removeList.Count;x++)
        {
            tempChunk = chunksVisibilityList[removeList[x]];
            chunksVisibilityList.Remove(removeList[x]);
            Destroy(tempChunk.gameObject);

        }

		//		for (a = 0;a < ChunkScript_t.chunksVisibilityList.Count; a++)
		//		{
		//			tempPos = ChunkScript_t.chunksVisibilityList[a].transform.position;
		//			delta = tempPos - transform.position;
		//			if (delta.magnitude < viewRange + (chunkWidth *3))
		//			{ 
		//				continue;
		//			}
		//			
		//			Destroy(ChunkScript_t.chunksVisibilityList[a].gameObject);
		//		}
		
		for (y = Mathf.Min((int) (playerPos.y - (vertViewRange)),0) ; y<(int) (playerPos.y + vertViewRange);y+= chunkHeight)
		{
			for (x = (int) (playerPos.x - viewRange); x <(int) (playerPos.x + viewRange); x+= chunkWidth)
			{
				for (z= (int) (playerPos.z - viewRange); z< (int) (playerPos.z + viewRange); z+= chunkDepth)
				{
					//					
					Vector3 pos = new Vector3(x,y,z);
					////					
					pos.x = Mathf.Floor(pos.x/((float)chunkWidth)) * (chunkWidth);
					pos.y = Mathf.Floor(pos.y/((float)chunkHeight))* (chunkHeight);
					pos.z = Mathf.Floor(pos.z/((float)chunkDepth)) * (chunkDepth);
					
					// Shave view cube
					delta = pos - transform.position;
					if (delta.magnitude > viewRange)
					{
						continue;
					}
					//					
					ChunkScript_t chunk = ChunkManager.FindChunk(pos);
					if (chunk != null) 
					continue;
					//if (chunk == null)						
					ChunkManager.Add(chunkFab, pos, Quaternion.identity, delta.magnitude);	
					//chunk = (ChunkScript_t) Instantiate(chunkFab, pos, Quaternion.identity);
					
				}
			}
		}						
	}
	public static void Add(ChunkScript_t chunkFab, Vector3 pos, Quaternion dir, float dist)
	{
		ChunkScript_t chunk = (ChunkScript_t) Instantiate(chunkFab, pos, Quaternion.identity);
		chunksLoadList.Enqueue(dist, chunk);
		string temp = pos.x.ToString() + pos.y.ToString() + pos.z.ToString();
		chunksVisibilityList.Add(temp, chunk);
	}
	
	public static ChunkScript_t FindChunk(Vector3 pos)
	{
		int a ;
//				for (a=0;a<chunksVisibilityList.Count;a++)
//				{
//					Vector3 cpos = chunksVisibilityList[a].transform.position;
//					
//					if (( pos.x < cpos.x)||(pos.z<cpos.z)||(pos.y<cpos.y)||(pos.x >=cpos.x+width)||(pos.y >=cpos.y+height)||(pos.z >=cpos.z+depth)) 
//					{
//						continue;
//					}
//					return chunksVisibilityList[a];
//				}
		string tempString = pos.x.ToString() + pos.y.ToString() + pos.z.ToString();
		if (chunksVisibilityList.ContainsKey(tempString))
		{
			return chunksVisibilityList[tempString];	
		}		

		return null;
		
	}
			
			
	public static float CalculateNoiseValue(Vector3 pos, Vector3 offset, float scale)
	{
		
		float noiseX = Mathf.Abs((pos.x + offset.x) * scale);
		float noiseY = Mathf.Abs((pos.y + offset.y) * scale);
		float noiseZ = Mathf.Abs((pos.z + offset.z) * scale);
		
		return Mathf.Max(0, Noise.Generate(noiseX, noiseY, noiseZ));
		
	}
	
	public static float GetTheoreticalMesh(Vector3 pos) {
	
		return GetTheoreticalMesh(pos, offset0, offset1, offset2);
	}
	
	public static float GetTheoreticalMesh(Vector3 pos, Vector3 offset0, Vector3 offset1, Vector3 offset2)
	{
		byte brickType;
		
		
		float noiseValue = CalculateNoiseValue (pos, offset0, 0.01117f)*14; //spread and height
		float mountainValue = CalculateNoiseValue (pos, offset2, 0.0014f) ;
		//mountainValue -= ((float)pos.y / 124);

		//noiseValue -= ((float)pos.y / 5);
		//noiseValue = Mathf.Max (noiseValue, CalculateNoiseValue (pos, offset1, 0.03f));
		//noiseValue -= ((float)pos.y / 2);
		mountainValue -= ((float)pos.y / 40);
		noiseValue += mountainValue * 60;
						
		//noiseValue += CalculateNoiseValue (pos, offset1, 0.01f)*5;
		
		return noiseValue;
	}
	
	
	
	public static byte GetTheoreticalByte(Vector3 pos) {

		return GetTheoreticalByte(pos, offset0, offset1, offset2);		
	}
	
	public static  byte GetTheoreticalByte(Vector3 pos, Vector3 offset0, Vector3 offset1, Vector3 offset2)
	{
		byte brickType;
		if (pos.y < 1) 
		{	
			return 1;
		}

		float noiseValue = CalculateNoiseValue (pos, offset0, 0.01917f);
		float mountainValue = CalculateNoiseValue (pos, offset2, 0.009f) * 5;
		mountainValue -= ((float)pos.y / 44);

		noiseValue -= ((float)pos.y / 4);
		noiseValue = Mathf.Max (noiseValue, CalculateNoiseValue (pos, offset1, 0.03f));
		noiseValue -= ((float)pos.y / 2);
		noiseValue += mountainValue * 2;

		if (noiseValue > 0.5f)
			return 1;
		return 0;

		//noiseValue =30 - Mathf.Sqrt((pos.x*pos.x)+(pos.y*pos.y)+(pos.z*pos.z));

		//		if (pos.y == 1)
		//		{
		//			byte temp = (byte) Random.Range(0,2);
		//			return temp;
		//		}
		//		
		//		if ((Mathf.Sqrt(((pos.x *pos.x)+(pos.z*pos.z))) < 300) && (pos.y < 1))
		//		{
		//			return 1;
		//		}
		//		else
		//		{
		//			return 0;
		//		}	
		
		//		if ((Mathf.Sqrt(((pos.x *pos.x)+(pos.y*pos.y)+(pos.z*pos.z))) < 60) )
		//		{
		//			return 1;
		//		}
		//		else
		//		{
		//			return 0;
		//		}	
	}
			
	public static int width {	
		get { return WorldThreaded.currentWorld.chunkWidth; }
	}
	public static int height {
		get { return WorldThreaded.currentWorld.chunkHeight; }
	}
	public static int depth {
		get { return WorldThreaded.currentWorld.chunkDepth; }
	}
	public static float brickHeight {
		get { return WorldThreaded.currentWorld.chunkHeight; }
	}
}
