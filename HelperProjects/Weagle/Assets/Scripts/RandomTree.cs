using UnityEngine;
using System.Collections;

public class RandomTree : MonoBehaviour {
	public GameObject TreeFab;
	public Random rand;
	public GameObject[] forest;
	public int MaxTrees = 100;
	public int index =0;
	// Use this for initialization
	void Start () {
		forest = new GameObject[MaxTrees];

//		for (int i=0; i<MaxTrees; i++) 
//		{
//			GameObject tempTree =  (GameObject) Instantiate(TreeFab, new Vector3(Random.Range(0,100),0,Random.Range(0,100)), Quaternion.identity);
//			tempTree.transform.Rotate(new Vector3(0,Random.Range(0,359),0));
//		}
	}
	
	// Update is called once per frame
	void Update () {
		index ++;
		if (index == MaxTrees) 
		{
			index = 0;
		}

		if (forest [index] != null) 
		{
			Destroy (forest [index].gameObject);
		}
		forest[index] =  (GameObject) Instantiate(TreeFab, new Vector3(Random.Range(0,100),0,Random.Range(0,100)), Quaternion.identity);
		forest[index].transform.Rotate(new Vector3(0,Random.Range(0,359),0));
	
	}
}
