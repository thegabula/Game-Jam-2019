using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour {
	public static World currentWorld;
	public static float XBoundary;
	public static float YBoundary;
	public static float ZBoundary;

	public BaseFish bFish;



	// Use this for initialization
	void Awake () 
	{
		XBoundary = 1000;
		YBoundary = 1000;
		ZBoundary = 1000;
		currentWorld = this;
	
		int i,count;
		BaseFish tempFish;
		count = (int)((Random.value*100)+100);
		//for (i=count;i<2000;i++) 
		for (i=0;i<1000;i++)
		{

		tempFish =  (BaseFish) Instantiate(bFish,Vector3.zero,Quaternion.identity);
			
		}
	
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	

		
	}
}
