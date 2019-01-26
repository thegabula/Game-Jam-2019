using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class StickToSurface : MonoBehaviour {
	public Camera MainCam;
	public float Gravity;


	Ray pointDown;
	private Quaternion m_CharacterTargetRot;
	private Quaternion m_CameraTargetRot;

	private float rotationX;



	private Vector3 move;
	private Transform cam; // A reference to the main camera in the scenes transform
	private Vector3 camForward; // The current forward direction of the camera
	// Use this for initialization
	public float moveSpeed =6;
	public float turnSpeed =90;
	private float lerpSpeed =10;
	private bool isGrounded;
	private float deltaGround = 0.2f;
	public float jumpSpeed = 10;
	private Vector3 surfaceNormal;
	private Vector3 myNormal;
	public float distGround =1;
	private bool jumping = false;
	private Transform myTransform;
	public BoxCollider boxCollider;
	private Rigidbody rigidbody;

	void Start () 
	{
		pointDown.direction = Vector3.down;
		pointDown.origin = transform.position+ new Vector3(0,2,0);	

		if (MainCam != null)
		{
			cam = MainCam.transform;
		}
		else
		{
			Debug.LogWarning(
				"Warning: no main camera found. Ball needs a Camera tagged \"MainCamera\", for camera-relative controls.");
			// we use world-relative controls in this case, which may not be what the user wants, but hey, we warned them!
		}

		myNormal = transform.up;
		myTransform = transform;
		rigidbody = GetComponent<Rigidbody> ();
		rigidbody.freezeRotation = true;
		m_CameraTargetRot = cam.localRotation;
		rotationX = cam.localRotation.x;

		//distGround = boxCollider..bounds
	}

	private void FixedUpdate()
	{
		rigidbody.AddForce (-Gravity * rigidbody.mass * myNormal);
	}
	// Update is called once per frame

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
			angle += 360f;
		if (angle > 360f)
			angle -= 360f;

		return Mathf.Clamp (angle, min, max);
	}

	void Update () 
	{
//		float h = CrossPlatformInputManager.GetAxis("Horizontal");
//		float v = CrossPlatformInputManager.GetAxis("Vertical");
//		float yRot = Input.GetAxis("Mouse X") * 2;
//		float xRot = Input.GetAxis("Mouse Y") * 2;
//		rotationX += xRot;
//		rotationX = ClampAngle (rotationX, -30, 40);
//			// we use world-relative directions in the case of no main camera
//
		//m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);

		//m_CameraTargetRot *= Quaternion.Euler (-3 * rotationX, 0f, 0f);
//		m_CameraTargetRot *= Quaternion.AngleAxis(rotationX,Vector3.left);
		//
//			if (cam.eulerAngles.x < 30)
//				cam.eulerAngles =new  Vector3(30, cam.eulerAngles.y, cam.eulerAngles.z);


		//transform.localRotation = Quaternion.Slerp (transform.localRotation, m_CharacterTargetRot,2 * Time.deltaTime);
		//cam.localRotation = Quaternion.Slerp (cam.localRotation, m_CameraTargetRot, lerpSpeed * Time.deltaTime);
//		cam.localRotation = cam.localRotation * m_CameraTargetRot;
	//	cam.localRotation = Quaternion.Slerp (cam.localRotation, m_CameraTargetRot, 2 * Time.deltaTime);

		RaycastHit hits;
//		if (Physics.Raycast(pointDown,out hits,15.0f))
//		{
//			Debug.DrawRay(hits.point, hits.normal,Color.blue);
//		}
//		
//		
//		
//		pointDown.origin = transform.position;
//		Debug.DrawRay(pointDown.origin,pointDown.direction,Color.yellow);
//
//		transform.position.y = hits.distance - transform.collider.bounds.extents;

//		RaycastHit hit;
//		Debug.DrawLine(transform.position, transform.position+transform.up);
//		Debug.DrawLine(transform.position, transform.position+transform.right);
//		Debug.DrawLine(transform.position, transform.position-transform.up);
//		Debug.DrawLine(transform.position, transform.position-transform.right);
//		//if (Physics.SphereCast(transform.position, 0.50f, -transform.up, out hit, 3))
//		if (Physics.Raycast(transform.position, -transform.up,out hit, 5))
//		//if (Physics.Raycast(transform.position, -transform.up, out hit, 5f))
//		{
//			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.Cross(transform.right, hit.normal), hit.normal), Time.deltaTime * 25.0f);
//		}        
//		myNormal = transform.up;
//		//rigidbody.AddForce(-transform.up * Time.deltaTime*Gravity);
//
//		move = (v*Vector3.forward + h*Vector3.right).normalized;
//		//rigidbody.AddForce (transform.forward * Time.deltaTime + move);
//		//transform.position +=(move * Time.deltaTime*5);	
//		transform.Translate (move * Time.deltaTime * 5);

		if (jumping)
			return;

		Ray ray;
		if (Input.GetButtonDown ("Jump")) 
		{
//			ray = new Ray(myTransform.position, myTransform.forward);
//			if (Physics.Raycast(ray, out hits,jumpRange))
//			{
//				JumpToWall(hits.point, hits.normal);
//			}
//			else if (isGrounded)
//			{
				rigidbody.velocity += jumpSpeed * myNormal;
			//}
		}
		myTransform.Rotate(0, Input.GetAxis("Mouse X")*turnSpeed*lerpSpeed*Time.deltaTime, 0);
		// update surface normal and isGrounded:
		ray = new Ray(myTransform.position, -myNormal); // cast ray downwards
		//if (Physics.Raycast(ray, out hits)){ // use it to update myNormal and isGrounded
		if (Physics.SphereCast(ray,0.5f, out hits))//transform.position, 0.50f, -transform.up, out hit, 3))
		{
		//	Debug.Log(hits.distance);
			isGrounded = hits.distance <= distGround + deltaGround;
			if (isGrounded)
			{
				surfaceNormal = hits.normal;
			}
			else
			{
				surfaceNormal = Vector3.up;
			}
		}
		else {
			isGrounded = false;
			// assume usual ground normal to avoid "falling forever"
			surfaceNormal = Vector3.up;
		}
		myNormal = Vector3.Lerp(myNormal, surfaceNormal, lerpSpeed*Time.deltaTime);
		// find forward direction with new myNormal:
		Vector3 myForward = Vector3.Cross(myTransform.right, myNormal);
		//Vector3 myForward = myTransform.forward;
		// align character to the new myNormal while keeping the forward direction:
		if (isGrounded) 
		{
			Quaternion targetRot = Quaternion.LookRotation (myForward, myNormal);
			myTransform.rotation = Quaternion.Lerp(myTransform.rotation, targetRot, lerpSpeed*Time.deltaTime);
		} 


		// move the character forth/back with Vertical axis:
		myTransform.Translate(Input.GetAxis("Horizontal")*moveSpeed*Time.deltaTime, 0, Input.GetAxis("Vertical")*moveSpeed*Time.deltaTime);
		}
	
	private void JumpToWall(Vector3 point, Vector3 normal){
		// jump to wall
		jumping = true; // signal it's jumping to wall
		rigidbody.isKinematic = true; // disable physics while jumping
		Vector3 orgPos = myTransform.position;
		Quaternion orgRot = myTransform.rotation;
		Vector3 dstPos = point + normal * (distGround + 0.5f); // will jump to 0.5 above wall
		Vector3 myForward = Vector3.Cross(myTransform.right, normal);
		Quaternion dstRot = Quaternion.LookRotation(myForward, normal);
		
		StartCoroutine (jumpTime (orgPos, orgRot, dstPos, dstRot, normal));
		//jumptime
	}

	private IEnumerator jumpTime(Vector3 orgPos, Quaternion orgRot, Vector3 dstPos, Quaternion dstRot, Vector3 normal) {
		for (float t = 0.0f; t < 1.0f; ){
			t += Time.deltaTime*3;
			myTransform.position = Vector3.Lerp(orgPos, dstPos, t);
			myTransform.rotation = Quaternion.Slerp(orgRot, dstRot, t);
			yield return null; // return here next frame
		}
		myNormal = normal; // update myNormal
		rigidbody.isKinematic = false; // enable physics
		jumping = false; // jumping to wall finished
		
	}

	void OnCollisionStay(Collision collide)
	{
		RaycastHit hits;
		pointDown.direction = myNormal;
		if (Physics.Raycast(pointDown,out hits,3.0f))
		{
		//	Debug.DrawRay(hits.point, hits.normal,Color.green);
		}
		
		
		
		pointDown.origin = transform.position;
		//Debug.DrawRay(pointDown.origin,pointDown.direction,Color.red);
	}
}
