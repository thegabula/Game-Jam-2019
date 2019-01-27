using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public Rigidbody barrel;
    public float steerFactor;
    public float riverDamp;
    public Vector3 steerVelocity;

    public Slider steering;
    public float value;

	// Use this for initialization
	void Start () {
		
	}
	
    public void RestSlider()
    {
        steering.value = 0;
    }
	// Update is called once per frame
	void Update () {
        float steerValue;
        float dt = Time.deltaTime*20;
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        steerValue = steering.value;

#if !UNITY_ANDROID
        if (Input.GetKey(KeyCode.LeftArrow))
        {            
            barrel.AddForceAtPosition(Vector3.left * steerFactor, transform.position);
          //  barrel.transform.Translate(Vector3.left);

        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            barrel.AddForceAtPosition(Vector3.left * -steerFactor, transform.position);
        }
#endif

        barrel.AddForceAtPosition(Vector3.left * -steerValue * dt, transform.position);
        value = barrel.velocity.magnitude;
        
        // Dampen overall velocity
        steerVelocity =  (barrel.velocity * riverDamp)*-1;
        barrel.AddForceAtPosition(steerVelocity, transform.position);
    }
}
