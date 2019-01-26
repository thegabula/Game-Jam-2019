using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour {
	public static World currentWorld;
	public static float XBoundary;
	public static float YBoundary;
	public static float ZBoundary;

	public BaseFish bFish;
	public int FishTotal;
    public Bounds swimBounds;

	public GUITexture AccelTexture;
	public GUITexture SteerTexture;
	public GUITexture SteerBackingTexture;
	
	public bool isGameOver;
	public bool isPaused;


	// Use this for initialization
	void Awake () 
	{
		XBoundary = 1000;
		YBoundary = 1000;
		ZBoundary = 1000;
		currentWorld = this;
	
		int i;
		BaseFish tempFish;
        //count = (int)((Random.value*10)+10);
        //for (i=count;i<2000;i++) 
        for (i = 0; i < FishTotal; i++)
        {

            tempFish = (BaseFish)Instantiate(bFish, Vector3.forward, Quaternion.identity);
            tempFish.GetComponent<FishPropel>().swimBounds = swimBounds;
        }
	
		isGameOver = false;
		isPaused = false;
	    Screen.sleepTimeout = SleepTimeout.NeverSleep;
		//SteerTexture.guiTexture.pixelInset = new Rect(0,100,100,200);
//	    SteerTexture.guiTexture.pixelInset = new Rect(Screen.width-200,100,200,200);
//		AccelTexture.pixelInset = new Rect(0,100,200,200);
//		
//		SteerBackingTexture.pixelInset     = new Rect(Screen.width-200,100,200,200);

	     	     
	}
	
	// Update is called once per frame
	void Update ()
	{
	  if(Input.GetKeyDown(KeyCode.Escape))
	  {
	     isPaused = !isPaused;
	  }    
	  
	  if (BaseFish.fishList.Count == 0)
	  {
	     isGameOver = true;
	     isPaused = true;
	  }
	  
	}
	void OnGUI()
	{
	    int CenterX = Screen.width/2;
	    int CenterY = Screen.height/2;
	    int ScreenUnitX = Screen.width/10;
	    int ScreenUnitY = Screen.height/10;
	    GUIStyle customGUI = new GUIStyle(GUI.skin.button);
	    	    
	    customGUI.fontSize = ScreenUnitY;
	    
	    GUI.Box (new Rect(CenterX-ScreenUnitX,0,ScreenUnitX*2,ScreenUnitY*2), BaseFish.fishList.Count.ToString(),customGUI);
	    
	    if (isGameOver)
	    {
	       if ((GUI.Button(new Rect(CenterX-(ScreenUnitX*2),CenterY-(ScreenUnitY*1.5f),ScreenUnitX*4,ScreenUnitY*3), "Play Again", customGUI)))
	       {		
				isGameOver = false;
				isPaused = false;	
				BaseFish tempFish;
									
				GUI.Box (new Rect(CenterX-ScreenUnitX,0,ScreenUnitX*2,ScreenUnitY*2), "Resetting",customGUI);
				int i;
				//count = (int)((Random.value*10)+10);
				//for (i=count;i<2000;i++) 
				for (i=0;i<FishTotal;i++)
				{
					tempFish =  (BaseFish) Instantiate(bFish,Vector3.forward,Quaternion.identity);
					BaseFish.fishList[i].isAlive = true;
				}	
				
						
	       }
	    }
		
		if (isPaused)
		{
			if (GUI.Button(new Rect(CenterX-ScreenUnitX,Screen.height-(ScreenUnitY *2), ScreenUnitX*2, ScreenUnitY*2),"Quit", customGUI))
			{
				Application.Quit();
			}
		}
	    
	}
}
