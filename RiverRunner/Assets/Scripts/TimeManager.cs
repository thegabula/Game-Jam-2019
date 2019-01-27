using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour {

    public Text currentTimeUI;
    public float currentTime;
	// Use this for initialization
	void Start ()
    {
        currentTime = 0;
        currentTimeUI.text = ((int) currentTime).ToString();		
	}
	
	// Update is called once per frame
	void Update ()
    {
        currentTime += Time.deltaTime;
        currentTimeUI.text = ((int)currentTime).ToString();
	}

    public void ResetTime()
    {
        currentTime = 0;        

    }
}
