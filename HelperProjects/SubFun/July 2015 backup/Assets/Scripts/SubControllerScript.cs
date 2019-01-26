using UnityEngine;
using System.Collections;

public class SubControllerScript : MonoBehaviour 
{
	public myJoystick AccelArea;
	public myJoystick SteeringArea;
	//public GUITexture JoystickBack;



	public float maxSpeed = 40f;
	public float accel   = 0.05f;
	public float rotateSpeed = 0f;
	public float bouyanceySpeed = 300f;
	public float bouyancey = 0.0f;


	private float currentSpeed;
	private Vector3 currentHeading;
	private Quaternion startingRotation;
	private float pitch=0;
	private float UpBubble =0;
	// Use this for initialization
	void Start () 
	{
		currentSpeed = 20;
		currentHeading = new Vector3(0,0,1);
		startingRotation = transform.rotation; 
		bouyancey = 0f;
		
		//SteeringArea.guiTexture.pixelInset = new Rect( 0,40, SteeringArea.guiTexture.pixelInset.width, SteeringArea.guiTexture.pixelInset.height);
		//SteeringArea.guiTexture.
//		                           (float) Screen.height-SteeringArea.guiTexture.pixelInset.height);
//		
		
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
	    // Evaluate throttle
		if (AccelArea.isFingerDown) 
		{
		    currentSpeed = Mathf.Min(maxSpeed,currentSpeed+accel);
		    currentHeading.Normalize();
			GetComponent<Rigidbody>().velocity = currentHeading * currentSpeed;
	    }
	    else
	    {
	        currentSpeed = Mathf.Max(0, currentSpeed-accel);
	        currentHeading.Normalize();
	        GetComponent<Rigidbody>().velocity = currentHeading * currentSpeed;
	    }
	   
		
		// Evaluate steering
		if (SteeringArea.isFingerDown)
		{
		pitch = SteeringArea.position.x * rotateSpeed * Time.deltaTime;
		UpBubble =  SteeringArea.position.y *accel;
		
		pitch = Mathf.Clamp(pitch,-80,80);
		UpBubble = Mathf.Clamp(UpBubble,-50,10);
		
	
		
		//currentHeading += new Vector3(pitch,yaw,0.0f);
		
		 transform.Rotate( new Vector3(0, pitch,0));
		 bouyancey += UpBubble;
		 bouyancey = Mathf.Clamp(bouyancey,-10,10);
		 
		 transform.position = new Vector3(transform.position.x, transform.position.y + UpBubble, transform.position.z);
		// transform.Rotate (new Vector3(yaw,0,0));
	     currentHeading = transform.forward;
		// rigidbody.AddForce(new Vector3(bouyancey,0,0));
		//	currentSpeed = Mathf.Min(maxSpeed,currentSpeed+accel);
		//	currentHeading.Normalize();
		//	rigidbody.velocity = currentHeading * currentSpeed;
	    }
	}
	
	void LateUpdate ()
	{
//		//Vector3 temp = transform.up;
//	//	transform.rotation = Quaternion.Lerp(transform.rotation, startingRotation, Time.deltaTime ); 
//	//	temp = Vector3.Lerp(temp,Vector3.up,0.1f);
//	//	transform.Rotate (temp);
//		currentHeading = transform.forward;
        if (transform.position.y > 1000)
        {
        	transform.position.Set(transform.position.x,1000,transform.position.z);
        }
        
	}
}
