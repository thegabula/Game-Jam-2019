using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public GameObject target;    
    public float followDist = 45f;
    public float followHeight = 17;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 tempPos = target.transform.position;
        Vector3 myPos = transform.position;

        myPos.z = tempPos.z-followDist;
        myPos.x = tempPos.x;
        

        //myPos = tempPos - myPos;
        myPos.y = Mathf.Lerp(myPos.y, tempPos.y + followHeight, 0.1f);
        transform.transform.position =myPos;
       // transform.LookAt(target.transform.position);

	}
}
