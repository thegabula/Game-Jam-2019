using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour {

	public static PriorityQueue<ChunkScript_t> chunksLoadList = new PriorityQueue<ChunkScript_t> ();
	//public static Queue<ChunkScript_t> chunksLoadList = new Queue<ChunkScript_t>();
	public static List<ChunkScript_t> chunksRenderList = new List<ChunkScript_t>();
	public static List<ChunkScript_t> chunksUnloadList = new List<ChunkScript_t>();
	public static Dictionary<string, ChunkScript_t> chunksVisibilityList = new Dictionary<string, ChunkScript_t>();
	public static List<ChunkScript_t> chunksSetupList = new List<ChunkScript_t>();
	public  ChunkScript_t chunkFab;
	
	private static Vector3 offset0;
	private static Vector3 offset1;
	private static Vector3 offset2;
	
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
	 	
	 	//loadingCube = false;
	}
	
	// Update is called once per frame
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
			Debug.Log ("Setup Count: " + chunksSetupList.Count);
			for(i=0;i<chunksSetupList.Count;i++)
			{
				if(chunksSetupList[i].CheckIfCalculated())
				{
					StartCoroutine(chunksSetupList[i].LoadVisualMeshForDisplay());
				//	chunksSetupList.RemoveAt(i);							
				}
				else
				{
					tempList.Add(chunksSetupList[i]);
					//Debug.Log("Not Ready to show mesh yet");
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
						
			for(i=0;i<Mathf.Min(2,chunksLoadList.Count());i++)
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
		
//		FloodQueue.Enqueue(playerPos);
//		while (FloodQueue.Count >0 && breakCount < 100)
//		{
//		breakCount++;
//			currentPosition = FloodQueue.Dequeue();
//			currentPosition.x = Mathf.Floor(currentPosition.x/((float)chunkWidth)) * (chunkWidth);
//			currentPosition.y = Mathf.Floor(currentPosition.y/((float)chunkHeight))* (chunkHeight);
//			currentPosition.z = Mathf.Floor(currentPosition.z/((float)chunkDepth)) * (chunkDepth);
//			
//			delta = currentPosition - transform.position;
//			if (delta.magnitude > viewRange)
//			{
//				ClosedList.Add(currentPosition);
//				continue;
//			}
//			
//			ChunkScript_t chunk = ChunkManager.FindChunk(currentPosition);
////			if (chunk != null) 
////			{
////				FloodQueue.Enqueue(tempPosition);
////				continue;
////			}
//			if (chunk == null)						
//			{
//				ChunkManager.Add(chunkFab, currentPosition, Quaternion.identity);	
//			}
//			
//			
//			ClosedList.Add(currentPosition);
//			
//			tempPosition = new Vector3(currentPosition.x + chunkWidth,currentPosition.y, currentPosition.z);
//			if (!ClosedList.Contains(tempPosition))
//			{
//				FloodQueue.Enqueue(tempPosition);	
//			}
//			tempPosition = new Vector3(currentPosition.x,currentPosition.y + chunkHeight, currentPosition.z);
//			if (!ClosedList.Contains(tempPosition))
//			{
//				FloodQueue.Enqueue(tempPosition);	
//			}
//			tempPosition = new Vector3(currentPosition.x,currentPosition.y, currentPosition.z + chunkDepth);
//			if (!ClosedList.Contains(tempPosition))
//			{
//				FloodQueue.Enqueue(tempPosition);	
//			}
////			
//			tempPosition = new Vector3(currentPosition.x - chunkWidth,currentPosition.y, currentPosition.z);
//			if (!ClosedList.Contains(tempPosition))
//			{
//				FloodQueue.Enqueue(tempPosition);	
//			}
//			tempPosition = new Vector3(currentPosition.x,currentPosition.y - chunkHeight, currentPosition.z);
//			if (!ClosedList.Contains(tempPosition))
//			{
//				FloodQueue.Enqueue(tempPosition);	
//			}
//			tempPosition = new Vector3(currentPosition.x,currentPosition.y, currentPosition.z - chunkDepth);
//			if (!ClosedList.Contains(tempPosition))
//			{
//				FloodQueue.Enqueue(tempPosition);	
//			}			
			// Make sure within view distance
			
		//}
		
		for (y = (int) (playerPos.y - (vertViewRange)) ; y<(int) (playerPos.y + vertViewRange);y+= chunkHeight)
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

		// Main hill hole
		if ((pos.x * pos.x) + (pos.z * pos.z ) <23.53f)
			return 0;

		if ((pos.y < 0) && (pos.y > -10))
			return 1;

		float noiseValue = CalculateNoiseValue (pos, offset0, 0.04917f);

		if (pos.y > -0.5f) {
			float mountainValue = CalculateNoiseValue (pos, offset2, 0.009f) * 5;
			mountainValue -= ((float)pos.y / 44);
			
			noiseValue -= ((float)pos.y / 4);
			noiseValue = Mathf.Max (noiseValue, CalculateNoiseValue (pos, offset1, 0.03f));
			noiseValue -= ((float)pos.y / 2);
			noiseValue += mountainValue * 2;

		} 
		else 
		{
			noiseValue = 1-noiseValue;
		}
				
		//noiseValue =30 - Mathf.Sqrt((pos.x*pos.x)+(pos.y*pos.y)+(pos.z*pos.z));
		
		return noiseValue;
	}
	
	
	
	public static byte GetTheoreticalByte(Vector3 pos) {

		return GetTheoreticalByte(pos, offset0, offset1, offset2);		
	}
	
	public static  byte GetTheoreticalByte(Vector3 pos, Vector3 offset0, Vector3 offset1, Vector3 offset2)
	{
		byte brickType;
		
		float noiseValue = CalculateNoiseValue (pos, offset0, 0.01917f);
		if (pos.y > -10) {
			float mountainValue = CalculateNoiseValue (pos, offset2, 0.009f) * 5;
			mountainValue -= ((float)pos.y / 44);

			noiseValue -= ((float)pos.y / 4);
			noiseValue = Mathf.Max (noiseValue, CalculateNoiseValue (pos, offset1, 0.03f));
			noiseValue -= ((float)pos.y / 2);
			noiseValue += mountainValue * 2;

		//	if (noiseValue < 0.5f)
		//		return 1;
		//	return 0;
		}
		//else 
		{
			if (noiseValue > 0.5f)
				return 1;
			return 0;
		}
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
