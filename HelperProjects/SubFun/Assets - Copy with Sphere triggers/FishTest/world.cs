using UnityEngine;
using System.Collections;

public class world : MonoBehaviour {

	public FishPropel fishPrefab;

	// Use this for initialization
	void Start () 
	{
		int i;

		for (i = 0;i<50;i++)
		{
	 	FishPropel temp = (FishPropel)	Instantiate(fishPrefab);
			temp.transform.position = new Vector3((Random.value * 500)-250,(Random.value * 500)-250, (Random.value * 500)-250);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
