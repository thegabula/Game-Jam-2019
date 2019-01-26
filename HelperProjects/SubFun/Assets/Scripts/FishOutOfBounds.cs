using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishOutOfBounds : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BaseFish")
        {
            other.gameObject.GetComponent<FishPropel>().turning = false;
            Debug.Log("Fish inbounds-" + other.gameObject.name);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "BaseFish")
        {
            other.gameObject.GetComponent<FishPropel>().turning = true;
            Debug.Log("Fish out of bounds-" + other.gameObject.name);
        }
    }
}
