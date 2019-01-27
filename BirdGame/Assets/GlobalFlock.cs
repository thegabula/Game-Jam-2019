using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFlock : MonoBehaviour
{
    public GameObject bugPrefab;
    public static int sendboxSize = 2;

    static int numBugs = 5;
    public static GameObject[] allBugs = new GameObject[ numBugs ];

    public static Vector3 goalPos = Vector3.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i=0; i < numBugs; i++) {

            Vector3 pos = new Vector3( Random.Range(-sendboxSize, sendboxSize),
                                       Random.Range(-sendboxSize, sendboxSize),
                                       Random.Range(-sendboxSize, sendboxSize)+10);
            allBugs[i] = Instantiate(bugPrefab, pos, Quaternion.identity);
        }
        //Debug.Log( allBugs.Length.ToString() );
    }

    // Update is called once per frame
    void Update()
    {
        if ( Random.Range(0, 10000)<50)
        {
            goalPos = new Vector3(Random.Range(-sendboxSize, sendboxSize),
                                  Random.Range(-sendboxSize, sendboxSize),
                                  Random.Range(-sendboxSize, sendboxSize));
        }
    }
}
