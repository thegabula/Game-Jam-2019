using UnityEngine;
using System.Collections;

public class MoveForward : MonoBehaviour {

    public bool Moving = false;
    public float Speed = 0.05f;
    
	// Use this for initialization
	void Start ()
    {
       
	}
	
	// Update is called once per frame
	void Update () 
	{     
        if (Moving)
        {
            transform.Translate(Vector3.forward * Speed*Time.deltaTime);
            WaveMaker.Flowing = true;
        }
        else
        {
            WaveMaker.Flowing = false;
        }
	}    
}
