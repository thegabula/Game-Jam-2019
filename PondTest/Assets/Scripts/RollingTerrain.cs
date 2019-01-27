using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingTerrain : MonoBehaviour
{
    public Mesh mesh;
    public MeshFilter filter;
    public MeshRenderer renderer;

    public int width;
    public int depth;
    public int height;

    public int widthDivs;
    public int depthDivs;

    public GameObject pondPrefab;
    public GameObject insectSwarmPrefab;
    public GameObject treePrefab;

    public GameObject myTree;

    public bool underWater;
    public bool hasBugs;
    public bool hasTree;

    public float waterHeight;
    public float bugHeight;
    public float treeHeight;


    public GameObject GetTree()
    {
        return myTree;
    }

    // Start is called before the first frame update
    void Start()
    {
        int i, j, index;
        float y;

        

        int heightScale = 5;
        float detailScale = 5.0f;
        underWater = false;
        hasBugs = false;
        hasTree = false;

        mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] verticies = mesh.vertices; //new Vector3[width * depth] ;

        index = 0;
        for (i = 0; i < verticies.Length; i++)
        {
            verticies[i].y = NoiseManager.instance.GetHeight((verticies[i].x  + this.transform.position.x)/detailScale, (verticies[i].z + this.transform.position.z)/detailScale)* heightScale;

            int tempx, tempy;
            tempx = i % 10;
            tempy = i / 10;

            if (verticies[i].y < waterHeight && (!underWater ))
            {
                underWater = true;
                // Vector3 tempV = new Vector3((verticies[i].x + this.transform.position.x) / detailScale, verticies[i].y + this.transform.position.y, (verticies[i].z + this.transform.position.z)/detailScale);
                // Vector3 tempV = new Vector3(verticies[i].x + this.transform.position.x, 30, verticies[i].z + this.transform.position.z);
                Vector3 tempV = new Vector3(this.transform.position.x, waterHeight, this.transform.position.z);
                GameObject t = (GameObject)Instantiate(pondPrefab, tempV, Quaternion.identity);
                t.transform.parent = transform;
            }

            if (verticies[i].y < bugHeight && verticies[i].y > bugHeight-1 && !hasBugs)
            {
                hasBugs = true;

                Vector3 tempV = new Vector3(this.transform.position.x, 42, this.transform.position.z);
                GameObject t = (GameObject)Instantiate(insectSwarmPrefab, tempV, Quaternion.identity);
                t.transform.parent = transform;

            }

            if (verticies[i].y > treeHeight && !hasTree)
            {
                hasTree = true;
                // randomly decide if want to plant a tree
                if (Random.value > 0.5f)
                {
                    //Cycle through tree list and see if any other tree too close
                    if (TreeManager.instance.SafeToAddTree(gameObject))
                    {
                        Vector3 tempV = new Vector3(this.transform.position.x, verticies[i].y-1, this.transform.position.z);
                        GameObject t = (GameObject)Instantiate(treePrefab, tempV, Quaternion.identity);
                         t.transform.parent = transform;

                        myTree = t;
                        TreeManager.instance.AddTree(t);
                    }

                }
            }
        }

        mesh.vertices = verticies;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        this.gameObject.AddComponent<MeshCollider>();
    }

    // Update is called once per frame
    //private void OnDestroy()
    //{
    //    if (myTree != null)
    //    {
    //        TreeManager.instance.RemoveTree(myTree);
    //    }
    //}
}
