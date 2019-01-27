using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public static TreeManager instance;

    public float minDistance;
    public float maxDistance;
    public List<GameObject> treeList;
    // Start is called before the first frame update
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

        treeList = new List<GameObject>();

    }

    public void RemoveTree(GameObject tree)
    {
        //foreach (GameObject temp in treeList)
        //{
        //    if (temp.gameObject == tree)
        //    {
                treeList.Remove(tree);
        //    }
        //}
    }

    public void AddTree(GameObject tree)
    {
        treeList.Add(tree);
    }

    public bool SafeToAddTree(GameObject tree)
    {
        float safeDistance = (Random.value * maxDistance) + minDistance;

        Vector3 plantedPos;
        Vector3 testPos = tree.transform.position ;

        Vector3 closestPos;
        float closestDist =9999;

        List<GameObject> tempList = treeList;
        // Find Closest tree
        foreach(GameObject temp in tempList)
        {
            plantedPos = temp.transform.position;

            if (Vector3.Distance(plantedPos, testPos) < closestDist)
            {
                closestPos = plantedPos;
                closestDist = Vector3.Distance(plantedPos, testPos);
            }
        }

        if (closestDist > safeDistance)
        {
            return true;
        }
        else
        {
            return false;
        }


    }

}
