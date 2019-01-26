using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OctTreeTest : MonoBehaviour {
	Octree<GameObject> myJob1;
	Octree<int> myJob2;
	Octree<int> myJob5;
	Octree<int> myJob3;
	Octree<int> myJob4;

	public GameObject dummy;

	int count;
	List<Vector3> EntityPos;
	List<GameObject> Entities;



	// Use this for initialization
	void Start () {
		myJob1 = new Octree<GameObject> (new Vector3(100,10,10), 30);
		myJob1.InData = new Vector3[10];
		myJob1.Start ();
		count = 0;
		EntityPos = new List<Vector3>();
		Entities = new List<GameObject>();

//		myJob2 = new Octree<int> ();
//		myJob2.InData = new Vector3[40];
//		myJob2.Start ();
//
//		myJob3 = new Octree<int> ();
//		myJob3.InData = new Vector3[60];
//		myJob3.Start ();
		Vector3 tempPos;

		for (int i = 0; i<300; i++) 
		{
			count++;
			tempPos = new Vector3(Random.Range(-200,1528), Random.Range(-200,1233), Random.Range(-200,544));
			GameObject tempObj = (GameObject) Instantiate(dummy, tempPos,Quaternion.identity);
			myJob1.Add(tempPos, tempObj);
			EntityPos.Add(tempPos);
			Entities.Add(tempObj);
		}
//
//		myJob4 = new Octree ();
//		myJob4.InData = new Vector3[90];
//		myJob4.Start ();
//
//		myJob5 = new Octree ();
//		myJob5.InData = new Vector3[40];
//		myJob5.Start ();

	}
	
	// Update is called once per frame
	void Update () {
		if (myJob1 != null) 
		{
			if(myJob1.Update())
			{
//				myJob1 = null;
				if (EntityPos.Count>0)
				{
					//myJob1.Add(new Vector3(Random.Range(-10-count,10+count), Random.Range(-10-count,10+count), Random.Range(-10-count,10+count)), 2);
					myJob1.Remove(EntityPos[0], Entities[0]);
					Destroy(Entities[0]);
					EntityPos.RemoveAt(0);
					Entities.RemoveAt(0);
				}
				myJob1.Start();
			}
		}
		Debug.DrawRay(new Vector3(1,1,1),Vector3.left,Color.red);
		myJob1.DebugShow ();
		//count++;
		Debug.Log ("Point count " + EntityPos.Count);


//		if (myJob2 != null) 
//		{
//			if(myJob2.Update())
//			{
//				myJob2 = null;
//			}
//		}
//
//
//		if (myJob3 != null) 
//		{
//			if(myJob3.Update())
//			{
//				myJob3 = null;
//			}
//		}
//
//		if (myJob4 != null) 
//		{
//			if(myJob4.Update())
//			{
//				myJob4= null;
//			}
//		}
//
//		if (myJob5 != null) 
//		{
//			if(myJob5.Update())
//			{
//				myJob5 = null;
//			}
//		}
	
	}
}
