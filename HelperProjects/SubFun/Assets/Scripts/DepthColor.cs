using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]
[RequireComponent (typeof(Light))]

public class DepthColor : MonoBehaviour {
	public Camera cam;
	public Light sunlight1;
	public Light sunlight2;
	public Light sunlight3;
	public Light sunlight4;
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
		
		float g = y-0.2f;
		float b = y;
		Color c = new Color(0,g,b);
		//cam.backgroundColor =  c;
		RenderSettings.fogColor = c;
		//sunlight1.intensity = y+0.5f;
		sunlight2.intensity = 1;
//		sunlight3.intensity = y;
//		sunlight4.intensity = y;

	}
}
