using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFlock : MonoBehaviour
{
    public GameObject bugPrefab;
    public static float sendboxSize = 1;

    //public GameObject goalMarker;

    static int numBugs = 5;
    public static GameObject[] allBugs = new GameObject[ numBugs ];

    public static Vector3 goalPos;

    // Start is called before the first frame update
    private void Awake()
    {
        goalPos = GenGoalPos();
      //  goalMarker.transform.position = goalPos;
    }
    void Start()
    {
        for (int i=0; i < numBugs; i++) {

            Vector3 pos = GenGoalPos();

            allBugs[i] = Instantiate(bugPrefab, pos, Quaternion.identity);
        }
        //Debug.Log( allBugs.Length.ToString() );
    }

    // Update is called once per frame
    void Update()
    {
        if ( Random.Range(0, 10000)<50)
        {
            goalPos = GenGoalPos();
         //   goalMarker.transform.position = goalPos;
        }
    }

    public Vector3 GenGoalPos()
    {
        float tempfloat = Random.Range(-sendboxSize, sendboxSize);

        Vector3 temp  = new Vector3(Random.Range(-sendboxSize, sendboxSize) + transform.position.x,
                                  Random.Range(-sendboxSize, sendboxSize) + transform.position.y,
                                  Random.Range(-sendboxSize, sendboxSize) + transform.position.z);

        return temp;
    }
}
