using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]
[RequireComponent (typeof(Light))]

public class DepthColor : MonoBehaviour {
	public Camera cam;
	public Light sunlight;
	protected Transform tram;

	// Use this for initialization
	void Start () 
	{
		SetHeightColor();

	}
	
	// Update is called once per frame
	void Update () 
	{
		SetHeightColor();

	}

	private void SetHeightColor()
	{
		float y = transform.position.y/1000f;
		Color c = new Color(0,y,y);
		cam.backgroundColor =  c;
		sunlight.intensity = y;

	}
}
