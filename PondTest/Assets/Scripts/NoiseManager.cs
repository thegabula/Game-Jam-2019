using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseManager : MonoBehaviour
{
    public static NoiseManager instance = null;

    public float offsetFactor1;
    public float offsetFactor2;
    public float offsetFactor3;

    // Start is called before the first frame update
    private void Awake()
    {
        instance = new NoiseManager();
    }
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);


       // offsetFactor1 = 0.02f;
       // offsetFactor2 = 0.1f;
       // offsetFactor3 = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetHeight(float x, float z)
    {
        float value = 0;
        float tempX = x;
        float tempZ = z;

        float max = 5;
        float min = 4;

        Vector3 temp = new Vector3(tempX+50,0, tempZ);


        //tempX *= offsetFactor1;
        //tempZ *= offsetFactor1;

        if (offsetFactor1 != 0)
        {
            value = Mathf.PerlinNoise(temp.x * offsetFactor1, temp.z * offsetFactor1) * (14);
        }

        if (offsetFactor2 != 0)
        {
            value += Mathf.PerlinNoise(temp.x * offsetFactor2, temp.z * offsetFactor2) * (1);
        }

        if (offsetFactor3 != 0)
        {
            value += Mathf.PerlinNoise(temp.x * offsetFactor3, temp.z * offsetFactor3) * (1);
        }

        if (value > max)
        {
            Debug.Log("Max = " _ value);
            max = value;
        }
        if (value < min)
        {
            Debug.Log("Min = " + value);
            min = value;
        }

        return value;
    }
}
