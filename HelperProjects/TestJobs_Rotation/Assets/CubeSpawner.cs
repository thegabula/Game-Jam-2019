using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour {
    public GameObject prefab;

    public float radius;
    public int objectCount;
    public bool useECS;
    public List<GameObject> items;
    // Use this for initialization
	void Start ()
    {
        int i;
        GameObject tempObj;
        for (i=0; i< objectCount; i++)
        {
            tempObj = (GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity);

            tempObj.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0f, 359f), 0));
            tempObj.transform.Translate(new Vector3(Random.Range(1f,radius), Random.Range(1f,radius), Random.Range(1f,radius)));
            items.Add(tempObj);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
     //   float dt = Time.deltaTime;
	    //if (!useECS)
     //   {
     //       int i;
     //       GameObject tempObj;
     //       for (i = 0; i < items.Count; i++)
     //       {
     //           tempObj = items[i];

     //           tempObj.transform.rotation = tempObj.transform.rotation * Quaternion.AngleAxis(dt * 50f, Vector3.up);
     //       }
     //   }
	}
}
