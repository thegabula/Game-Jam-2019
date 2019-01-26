using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUITexture))]

class Boundary 
{
	Vector2 min = Vector2.zero;
	Vector2 max = Vector2.zero;
}
public class Propel : MonoBehaviour 
{
	protected Vector3 heading;
	protected Transform trans;

	private static Propel[] joysticks;
	private static bool enumeratedJoysticks = false;
	private static float tapTimeDelta = 0.3f;

	bool leftTouchpad;
	Rect leftTouchZone;
	Vector2 deadZone = Vector2.zero;
	bool normalize = false;
	Vector2 position;
	int tapCount;

	int lastFingerID = -1;
	float timeTapWindow;
	Vector2 fingerDownPos;
	float fingerDownTime;
	float firstDeltaTime = 0.5f;

	GUITexture accelButtonGUI;
	Rect defaultRect;
	Boundary accelButtonBoundary = new Boundary();
	Vector2 accelButtonOffset;
	Vector2 accelCenter;

	public float velocity;
	// Use this for initialization
	void Start () 
	{
	   // accelButtonGUI = GetComponent<GUITexture();
		defaultRect = accelButtonGUI.pixelInset;

		defaultRect.x += transform.position.x * Screen.width;// + gui.pixelInset.x; // -  Screen.width * 0.5;
		defaultRect.y += transform.position.y * Screen.height;// - Screen.height * 0.5;

		transform.position = Vector2.zero;

		if ( accelButtonGUI.texture )
		    leftTouchZone = defaultRect;
	}

	void Disable()
	{
		gameObject.SetActive(false);
		enumeratedJoysticks = false;
	}

	void ResetJoystick()
	{
		// Release the finger control and set the joystick back to the default position
		accelButtonGUI.pixelInset = defaultRect;
		lastFingerID = -1;
		position = Vector2.zero;
		fingerDownPos = Vector2.zero;
		
		if ( leftTouchpad )
			accelButtonGUI.color = new Color(1,1,1,0.025f);	
	}

	bool IsFingerDown()
	{
		return (lastFingerID != -1);
	}

	void LatchedFinger(int fingerId )
	{
		// If another joystick has latched this finger, then we must release it
		if ( lastFingerID == fingerId )
			ResetJoystick();
	}

	// Update is called once per frame
	void Update () 
	{
/*		if ( !enumeratedJoysticks )
		{
			// Collect all joysticks in the game, so we can relay finger latching messages
		//	joysticks = FindObjectsOfType(joysticks) as Propel[];
			enumeratedJoysticks = true;
		}	

		int count = Input.touchCount;

		// Adjust the tap time window while it still available
		if ( timeTapWindow > 0 )
		{
			timeTapWindow -= Time.deltaTime;
		}
		else
		{
			tapCount = 0;
		}

		if ( count == 0 )
			ResetJoystick();
		else
		{
			int i;
			for(i = 0;i < count; i++)
			{
				Touch touch = Input.GetTouch(i);			
				Vector2 accelTouchPos = touch.position - accelTouchOffset;
				
				bool shouldLatchFinger = false;
				if ( touchPad )
				{					
					if ( touchZone.Contains( touch.position ) )
						shouldLatchFinger = true;
				}
				else if (accelButtonGUI.HitTest( touch.position ) )
				{
					shouldLatchFinger = true;
				}		
				
				// Latch the finger if this is a new touch
				if ( shouldLatchFinger && ( lastFingerId == -1 || lastFingerId != touch.fingerId ) )
				{
					
					if ( touchPad )
					{
						accelButtonGUI.color.a = 0.15;
						
						lastFingerId = touch.fingerId;
						fingerDownPos = touch.position;
						fingerDownTime = Time.time;
					}
					
					lastFingerId = touch.fingerId;
					
					// Accumulate taps if it is within the time window
					if ( tapTimeWindow > 0 )
						tapCount++;
					else
					{
						tapCount = 1;
						tapTimeWindow = tapTimeDelta;
					}
					
					// Tell other joysticks we've latched this finger
					foreach (Joystick j in joysticks )
					{
						if ( j != this )
							j.LatchedFinger( touch.fingerId );
					}						
				}				
				
				if ( lastFingerId == touch.fingerId )
				{	
					// Override the tap count with what the iPhone SDK reports if it is greater
					// This is a workaround, since the iPhone SDK does not currently track taps
					// for multiple touches
					if ( touch.tapCount > tapCount )
						tapCount = touch.tapCount;
					
					if ( touchPad )
					{	
						// For a touchpad, let's just set the position directly based on distance from initial touchdown
						position.x = Mathf.Clamp( ( touch.position.x - fingerDownPos.x ) / ( touchZone.width / 2 ), -1, 1 );
						position.y = Mathf.Clamp( ( touch.position.y - fingerDownPos.y ) / ( touchZone.height / 2 ), -1, 1 );
					}
					else
					{					
						// Change the location of the joystick graphic to match where the touch is
						gui.pixelInset.x =  Mathf.Clamp( guiTouchPos.x, guiBoundary.min.x, guiBoundary.max.x );
						gui.pixelInset.y =  Mathf.Clamp( guiTouchPos.y, guiBoundary.min.y, guiBoundary.max.y );		
					}
					
					if ( touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled )
						ResetJoystick();					
				}			
			}



		}
		float x,y,z;

		heading = trans.forward;
		trans.position += heading.normalized * velocity * Time.deltaTime;

		x = trans.position.x;
		y = trans.position.y;
		z = trans.position.z;

		if (x >1000)
			x = 1000;
		if (y> 1000)
			y = 1000;
		if (z > 1000)
			z = 1000;

		if (x<0)
			x=0;
		if (y<0)
			y=0;
		if (z<0)
			z=0;

		trans.position = new Vector3(x,y,z);
	*/}
}
