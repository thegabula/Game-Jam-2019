using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScript : MonoBehaviour
{
    public GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(true);
        StartCoroutine("Flash");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Flash()
    {
        yield return new  WaitForSeconds(5);
        panel.SetActive(false);
        yield return 0;
    }
}
