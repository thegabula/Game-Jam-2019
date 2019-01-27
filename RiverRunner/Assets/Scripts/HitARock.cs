using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitARock : MonoBehaviour {
    public float hungUpTime;
    public float hungUpMax;
    public float bounceFactor;
    public TimeManager tm;
	// Use this for initialization
	void Start () {
        hungUpTime = 0;
        tm = FindObjectOfType<TimeManager>();
	}
	
	// Update is called once per frame
	void Update () {        
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collidee = collision.gameObject;
        if (collidee.tag == "Barrel")
        {
            collidee.GetComponentInParent<MoveForward>().Moving = false;
            WaveMaker.Flowing = false;
           // Debug.Log("Struck a rock");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        GameObject collidee = collision.gameObject;
        if (collidee.tag == "Barrel")
        {
            collidee.GetComponentInParent<MoveForward>().Moving = true;
            WaveMaker.Flowing = true;
            hungUpTime = 0;
            //Debug.Log("Off the rock");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        hungUpTime += Time.deltaTime;
       // collision.rigidbody.AddForceAtPosition(Vector3.up * bounceFactor, collision.transform.position);
        if (hungUpTime > hungUpMax)
        {
            GameObject barrel = collision.gameObject;            
          //  barrel.transform.position = new Vector3(0, 0, 0);
            RiverChunkManager.currentManager.ReStart();
            barrel.transform.Translate(Vector3.up * -35); // little bump to make it look like restarting
            Debug.Log("Hung UP");
            tm.ResetTime();
            hungUpTime = 0;
        }
    }

    }
