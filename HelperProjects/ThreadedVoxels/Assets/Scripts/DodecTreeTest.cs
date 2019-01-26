using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DodecTreeTest : MonoBehaviour
{
    DodecTree<GameObject> myJob1;
    DodecTree<int> myJob2;
    DodecTree<int> myJob5;
    DodecTree<int> myJob3;
    DodecTree<int> myJob4;

    public GameObject dummy;

    int count;
    List<Vector3> EntityPos;
    List<GameObject> Entities;



    // Use this for initialization
    void Start()
    {
        myJob1 = new DodecTree<GameObject>(new Vector3(10,10,10), 300);
        myJob1.InData = new Vector3[10];
        myJob1.Start();
        count = 0;
        EntityPos = new List<Vector3>();
        Entities = new List<GameObject>();

        //		myJob2 = new DodecTree<int> ();
        //		myJob2.InData = new Vector3[40];
        //		myJob2.Start ();
        //
        //		myJob3 = new DodecTree<int> ();
        //		myJob3.InData = new Vector3[60];
        //		myJob3.Start ();
        Vector3 tempPos;

        for (int i = 0; i < 30; i++)
        {
            count++;
            tempPos = new Vector3(Random.Range(-2000,2000), Random.Range(-2000, 2000), Random.Range(-2000, 2000));
            GameObject tempObj = (GameObject)Instantiate(dummy, tempPos, Quaternion.identity);
            myJob1.Add(tempPos, tempObj);
            EntityPos.Add(tempPos);
            Entities.Add(tempObj);
        }
        //
        //		myJob4 = new DodecTree ();
        //		myJob4.InData = new Vector3[90];
        //		myJob4.Start ();
        //
        //		myJob5 = new DodecTree ();
        //		myJob5.InData = new Vector3[40];
        //		myJob5.Start ();

    }

    // Update is called once per frame
    void Update()
    {
        if (myJob1 != null)
        {
            if (myJob1.Update())
            {
                //				myJob1 = null;
                if (EntityPos.Count > 0)
                {
                    //myJob1.Add(new Vector3(Random.Range(-10-count,10+count), Random.Range(-10-count,10+count), Random.Range(-10-count,10+count)), 2);
                 //   myJob1.Remove(EntityPos[0], Entities[0]);
                 //   Destroy(Entities[0]);
                 //   EntityPos.RemoveAt(0);
                 //   Entities.RemoveAt(0);

                // Move points 
                for(int i = 0;i<EntityPos.Count;i++)
                    {
                        EntityPos[i] += new Vector3(1, 0, 0);
                        Entities[i].transform.position += new Vector3(1, 0, 0);
                    }
                    
                }
                myJob1.Start();
               
            }
        }
        Debug.DrawRay(Vector3.zero, EntityPos[0] - Vector3.zero, Color.red);
        myJob1.DebugShow();
        //count++;
        Debug.Log("Point count " + EntityPos.Count);


        //		if (myJob2 != null) 
        //		{
        //			if(myJob2.Update())
        //			{
        //				myJob2 = null;
        //			}
        //		}
        //
        //
        //		if (myJob3 != null) 
        //		{
        //			if(myJob3.Update())
        //			{
        //				myJob3 = null;
        //			}
        //		}
        //
        //		if (myJob4 != null) 
        //		{
        //			if(myJob4.Update())
        //			{
        //				myJob4= null;
        //			}
        //		}
        //
        //		if (myJob5 != null) 
        //		{
        //			if(myJob5.Update())
        //			{
        //				myJob5 = null;
        //			}
        //		}

    }
}
