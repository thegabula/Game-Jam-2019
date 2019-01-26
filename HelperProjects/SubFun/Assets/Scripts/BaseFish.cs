using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(MeshFilter))]
[RequireComponent (typeof(CapsuleCollider))]
[RequireComponent (typeof(MeshRenderer))]
[RequireComponent (typeof(FishPropel))]
[RequireComponent (typeof(AudioSource))]

public class BaseFish : MonoBehaviour {
	public bool isAlive;
	public FishPropel fishPropel;
	
	protected Material skin;
	protected Mesh mMesh;
	protected Transform trans;

	protected AudioClip dieNoise;
	protected AudioSource dieSource;
	protected MeshRenderer mMeshRenderer;
	protected CapsuleCollider mCapCollider;
	protected MeshFilter mMeshFilter;

	public static List<BaseFish> fishList = new List<BaseFish>();

	// Use this for initialization
	void Start () 
	{
		Random r = new Random();
		fishList.Add(this);

		isAlive = true;
		dieSource = this.GetComponent<AudioSource>();
		
		mCapCollider = GetComponent<CapsuleCollider>();
		mMeshRenderer = GetComponent<MeshRenderer>();
		mMeshFilter = GetComponent<MeshFilter>();
		trans = transform;
		//theWorld   = World.currentWorld;

		skin = new Material(mMeshRenderer.material);
		skin.color = new Color( Random.value, Random.value,0);

		mMeshRenderer.material = skin;

		trans.position = new Vector3(Random.value*World.XBoundary,Random.value*World.YBoundary,Random.value*World.ZBoundary); 
		//trans.position += new Vector3(Random.value*100,Random.value*100,Random.value*100); 
		//trans.position = new Vector3(10,10,10);
	}

	public void OnTriggerEnter(Collider collision)
	{
        if (collision.tag == "Player")
        {
            skin = new Material(mMeshRenderer.material);
            skin.color = new Color(1, 1, 1);

            mMeshRenderer.material = skin;
            SkinnedMeshRenderer smr = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            smr.sharedMesh = null;
            dieSource.Play();

            isAlive = false;
        }
	}

	// Update is called once per frame
	void Update () 
	{

		if (!isAlive) 
		{
			if (!dieSource.isPlaying) 
			{	
				FishPropel tempfishPropel = GetComponent<FishPropel> ();
				FishPropel.FishList.Remove (tempfishPropel);				
				fishList.Remove (this);
				enabled = false;
				Destroy (this.gameObject);	
			}
		}

	}
}
