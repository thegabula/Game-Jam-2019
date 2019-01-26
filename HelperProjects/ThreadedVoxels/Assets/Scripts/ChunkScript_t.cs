using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;


[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(MeshCollider))]
[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(ChunkManager))]

public class ChunkCalculator : ThreadedJob
{

	float[,,] _map;
	List<Vector3> verts = new List<Vector3>();
	List<Vector2> uvs = new List<Vector2>();
	List<int> tris	 = new List<int>();
	Vector3 myTransform;
	int width, height, depth;

	public  ChunkCalculator(ref float[,,] map, Vector3 pos)
	{
		width = map.GetUpperBound (0)+1;
		height = map.GetUpperBound (1)+1;
		depth = map.GetUpperBound (2)+1;

		_map = map;
		myTransform = pos;
	}

	protected override void ThreadFunction1()
	{
		int x, y, z;
		for ( x = 0; x < width; x++)
		{
			for ( y = 0; y < height; y++)
			{
				for ( z = 0; z < width; z++)
				{
					_map[x, y, z] = ChunkManager.GetTheoreticalMesh(new Vector3(x,y,z) + myTransform);//, offset0, offset1, offset2);
				}
			}
		}
	
		//if (myTransform.y < 0) 
		//{
		//	CreateMarchingMeshBelowGround ();
		//} 
		//else 
		{
			CreateMarchingMeshAboveGround();
		}

	}

	protected override void OnFinished()
	{
		Debug.Log ("Mesh done");
	}

	protected void SetChunkMap(float[,,] map)
	{
		_map = map;
	}

	public List<Vector3> GetVerts ()
	{
		return verts;
	}
	public List<Vector2> GetUVs ()
	{
		return uvs;
	}

	public List<int> GetTris ()
	{
		return tris;
	}

	public void SetPosition(Vector3 pos)
	{
		myTransform = pos;
	}

	public  void CreateMarchingMeshBelowGround()
	{
		verts.Clear();
		uvs.Clear();
		tris.Clear();
		MarchingInverterThreaded mc = new MarchingInverterThreaded();
		mc.Init(width,height,depth,myTransform,_map);
		mc.Processf(0.50f,ref verts,ref tris,ref uvs);
		Debug.Log (verts.Count);
	}

	public  void CreateMarchingMeshAboveGround()
	{
		verts.Clear();
		uvs.Clear();
		tris.Clear();
		MarchingConverterThreaded mc = new MarchingConverterThreaded();
		mc.Init(width,height,depth,myTransform,_map);
		mc.Processf(0.50f,ref verts,ref tris,ref uvs);
		Debug.Log (verts.Count);
	}

}

public class ChunkScript_t : MonoBehaviour
{
	public byte[,,]  map;
	public float[,,] meshMap;
	public Mesh visualMesh;
	protected MeshRenderer meshRenderer;
	protected MeshCollider meshCollider;
	protected MeshFilter meshFilter;
	
	public ChunkManager theManager;
	
	private Thread workerThread1;
	private Thread workerThread2;
	private Vector3 myTransform;
	private int theSeed;
	public bool CalculatingMesh;
	private static int loadIndex;
	private static int currentIndex;
	private int myIndex;
	public bool Displayed;

	//Holding variables
	List<Vector3> verts = new List<Vector3>();
	List<Vector2> uvs = new List<Vector2>();
	List<int> tris	 = new List<int>();
	bool chunkCalulated;
	bool chunkLoaded; 
	static bool RenderChunksQueued;

	ChunkCalculator CC1;
	
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
	// Use this for initialization
	void Start () {
		meshRenderer = GetComponent<MeshRenderer>();
		meshCollider = GetComponent<MeshCollider>();
		meshFilter  = GetComponent<MeshFilter>();
		
		myTransform = transform.position;
		CalculatingMesh = false;
		Debug.Log ("Mesh " + loadIndex + " added to Load List" + transform.position.ToString());
		loadIndex ++;

		workerThread1 = new Thread(CreateMarchingMeshAboveGround);
		//workerThread2 = new Thread(CreateMarchingMeshBelowGround);
	}
	
	// Update is called once per frame
	public bool CheckIfCalculated() 
	{
		if (CC1 != null) 
		{
			if(CC1.Update())
			{
				verts = CC1.GetVerts ();
				uvs = CC1.GetUVs ();
				tris = CC1.GetTris ();
				
				Debug.Log("sremoving " + currentIndex);
				currentIndex++;
				CC1 = null;
				CalculatingMesh = false;
				return true;
			}
		}
		return false;
	}

	public virtual void OnFinished() {}

	public virtual IEnumerator CalculateMapFromScratch()
	{
		map = new byte[width, height, depth];
		meshMap = new float[width,height,depth];

		int x,y,z;
		CalculatingMesh = true;
		CC1 = new ChunkCalculator (ref meshMap, transform.position);
		CC1.Start ();
//		while (!CC1.Update()) 
//		{
//			yield return 0;
//		}





//		for ( x = 0; x < width; x++)
//		{
//			for ( y = 0; y < height; y++)
//			{
//				for ( z = 0; z < width; z++)
//				{
//					meshMap[x, y, z] = ChunkManager.GetTheoreticalMesh(new Vector3(x,y,z) + transform.position);//, offset0, offset1, offset2);
//				}
//			}
//		}
//		chunkLoaded = true;
//		if (transform.position.y >= 10) {
//		//	workerThread1 = new Thread(CreateMarchingMeshAboveGround);
//			workerThread1.Start ();
//			CalculatingMesh = true;
//			yield return 0;
//			while(workerThread1.IsAlive)
//			{
//				yield return 0;
//			}
//		} 
//		else 
//		{
//			//workerThread2 = new Thread(CreateMarchingMeshBelowGround);
//			workerThread2.Start ();
//			CalculatingMesh = true;
//			yield return 0;
//			while(workerThread2.IsAlive)
//			{
//				yield return 0;
//			}
//		}

		//workerThread.Start();


		
		Debug.Log("removing " + currentIndex);
		currentIndex++;
		yield return 0;

	}

	public  void CreateMarchingMeshBelowGround()
	{
		verts.Clear();
		uvs.Clear();
		tris.Clear();
		MarchingInverterThreaded mc = new MarchingInverterThreaded();
		mc.Init(width,height,depth,myTransform,meshMap);
		mc.Processf(0.50f,ref verts,ref tris,ref uvs);
	}

	public  void CreateMarchingMeshAboveGround()
	{
		verts.Clear();
		uvs.Clear();
		tris.Clear();
		MarchingConverterThreaded mc = new MarchingConverterThreaded();
		mc.Init(width,height,depth,myTransform,meshMap);
		mc.Processf(0.50f,ref verts,ref tris,ref uvs);
	}
	public virtual IEnumerator CreateVisualMesh()
	{
		visualMesh = new Mesh();
		verts.Clear();
		uvs.Clear();
		tris.Clear();
		
		for(int x=0; x<WorldThreaded.currentWorld.chunkWidth; x++)
		{
			for(int y = 0;y<WorldThreaded.currentWorld.chunkHeight;y++)
			{
				for(int z = 0;z<WorldThreaded.currentWorld.chunkWidth;z++)
				{
					if (map[x,y,z] ==0) continue;
					
					byte brick = map[x,y,z];
					// left
					if( IsTransparent(x-1,y,z))
						BuildFace(brick, new Vector3(x,y,z), Vector3.up, Vector3.forward, false,verts,uvs,tris);
					// right
					if( IsTransparent(x+1,y,z))
						BuildFace(brick, new Vector3(x+1,y,z), Vector3.up, Vector3.forward, true,verts,uvs,tris);
					// top
					if( IsTransparent(x,y+1,z))
						BuildFace(brick, new Vector3(x,y+1,z), Vector3.forward, Vector3.right, true,verts,uvs,tris);
					// bottom
					if( IsTransparent(x,y-1,z))
						BuildFace(brick, new Vector3(x,y,z), Vector3.forward, Vector3.right, false,verts,uvs,tris);
					// front
					if( IsTransparent(x,y,z+1))
						BuildFace(brick, new Vector3(x,y,z+1), Vector3.up, Vector3.right, false,verts,uvs,tris);
					// back
					if( IsTransparent(x,y,z-1))
						BuildFace(brick, new Vector3(x,y,z), Vector3.up, Vector3.right, true,verts,uvs,tris);
				}
			}
		}
	
		visualMesh.vertices = verts.ToArray();
		visualMesh.uv = uvs.ToArray();
		visualMesh.triangles = tris.ToArray();
		visualMesh.RecalculateBounds();
		visualMesh.RecalculateNormals();
		
		meshFilter.mesh = visualMesh;
		meshCollider.sharedMesh = null;
		meshCollider.sharedMesh = visualMesh;
		yield return 0;
		
	}
	
	public virtual void BuildFace(byte type,Vector3 corner, Vector3 up, Vector3 right, bool reversed, List<Vector3> verts, List<Vector2> uvs, List<int> tris)
	{
		int index = verts.Count;
		
		verts.Add (corner);
		verts.Add (corner + up);
		verts.Add (corner + up + right);
		verts.Add (corner + right);
		
		uvs.Add(new Vector2(0,0));
		uvs.Add(new Vector2(0,1));
		uvs.Add(new Vector2(1,1));
		uvs.Add(new Vector2(1,0));
		
		if (reversed)
		{
			tris.Add(index+0);
			tris.Add(index+1);
			tris.Add(index+2);
			tris.Add(index+2);
			tris.Add(index+3);
			tris.Add(index+0);
		}
		else
		{
			tris.Add(index+1);
			tris.Add(index+0);
			tris.Add(index+2);
			tris.Add(index+3);
			tris.Add(index+2);
			tris.Add(index+0);
		}
	}
	
	public virtual bool IsTransparent (int x, int y, int z)
	{
		//return true;
		byte brick = GetByte(x,y,z);
		switch (brick)
		{
		case 0: 
			return true;
		default:
			return false;
		}
	}

	public virtual byte GetByte(int x, int y, int z)
	{
		Vector3 realPos = new Vector3(x,y,z);
		Vector3 worldPos = realPos + transform.position;
		
		if (! chunkLoaded)
			return ChunkManager.GetTheoreticalByte(worldPos);
		
		if ((x<0)||(y<0)||(z<0)||(x>=width)||(y>=height)||(z>=depth))
		{			
			ChunkScript_t chunk = ChunkManager.FindChunk(worldPos);
			if (chunk == this) return 0;
			if (chunk == null) 
			{
				return ChunkManager.GetTheoreticalByte(worldPos);
			}
			
			return chunk.GetByte(worldPos);
		}
		
		return map[x,y,z];
	}
	public virtual byte GetByte(Vector3 worldPos)
	{
		worldPos -= transform.position;
		//int x = (int)Mathf.Floor(worldPos.x);
		//int y = (int)Mathf.Floor(worldPos.y);
		//int z = (int)Mathf.Floor(worldPos.z);
		int x = (int)Mathf.Floor(worldPos.x);
		int y = (int)Mathf.Floor(worldPos.y);
		int z = (int)Mathf.Floor(worldPos.z);
		
		return GetByte(x,y,z);
	}

	public virtual IEnumerator LoadVisualMeshForDisplay()
	{
		visualMesh = new Mesh();
		visualMesh.vertices = verts.ToArray();
		visualMesh.uv = uvs.ToArray();
		visualMesh.triangles = tris.ToArray();
		visualMesh.RecalculateBounds();
		visualMesh.RecalculateNormals();
		
		meshFilter.mesh = visualMesh;
		meshCollider.sharedMesh = visualMesh;
		chunkLoaded = true;
		Displayed = true;
		Debug.Log ("Mesh loaded for display" + transform.position.ToString());
	//	chunksRenderList.RemoveAt(0);
	//	Debug.Log ("Mesh Removed from render list" + transform.position.ToString());
		yield return 0;	
			
	}

}