using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(MeshCollider))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(FishPropel))]

public class BaseFish : MonoBehaviour {
	public bool isAlive;
	public FishPropel fishPropel;
	
	protected Material skin;
	protected Mesh mMesh;
	protected Transform trans;
	
	protected MeshRenderer mMeshRenderer;
	protected MeshCollider mMeshCollider;
	protected MeshFilter mMeshFilter;

	public static List<BaseFish> fishList = new List<BaseFish>();

	// Use this for initialization
	void Start () 
	{
		Random r = new Random();
		fishList.Add(this);

		isAlive = true;
		
		mMeshCollider = GetComponent<MeshCollider>();
		mMeshRenderer = GetComponent<MeshRenderer>();
		mMeshFilter = GetComponent<MeshFilter>();
		trans = transform;
		//theWorld   = World.currentWorld;

		skin = new Material(mMeshRenderer.material);
		skin.color = new Color( Random.value, Random.value,0);

		mMeshRenderer.material = skin;

		trans.position += new Vector3(Random.value*World.XBoundary,Random.value*World.YBoundary,Random.value*World.ZBoundary); 
		//trans.position += new Vector3(Random.value*100,Random.value*100,Random.value*100); 
		//trans.position = new Vector3(10,10,10);
	}

	public void OnCollisionEnter(Collision collision)
	{
		skin = new Material(mMeshRenderer.material);
		skin.color = new Color( 1, 1,1);

		mMeshRenderer.material = skin;
		this.audio.Play();
		FishPropel tempfishPropel = GetComponent<FishPropel>();
		FishPropel.FishList.Remove(tempfishPropel);
		fishList.Remove(this);
	 	isAlive = false;
		enabled = false;
		Destroy(this.gameObject);	

	}

	// Update is called once per frame
	void Update () 
	{

	}
}
