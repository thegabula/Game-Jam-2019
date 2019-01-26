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

    public bool underWater;

    // Start is called before the first frame update
    void Start()
    {
        int i, j, index;
        float y;

        

        int heightScale = 5;
        float detailScale = 5.0f;
        underWater = false;

        mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] verticies = mesh.vertices; //new Vector3[width * depth] ;

        index = 0;
        for (i = 0; i < verticies.Length; i++)
        {
            verticies[i].y = NoiseManager.instance.GetHeight((verticies[i].x  + this.transform.position.x)/detailScale, (verticies[i].z + this.transform.position.z)/detailScale)* heightScale;

            int tempx, tempy;
            tempx = i % 10;
            tempy = i / 10;

            if (verticies[i].y < 30 && (!underWater ))
            {
                underWater = true;
                // Vector3 tempV = new Vector3((verticies[i].x + this.transform.position.x) / detailScale, verticies[i].y + this.transform.position.y, (verticies[i].z + this.transform.position.z)/detailScale);
                // Vector3 tempV = new Vector3(verticies[i].x + this.transform.position.x, 30, verticies[i].z + this.transform.position.z);
                Vector3 tempV = new Vector3(this.transform.position.x, 30, this.transform.position.z);
                GameObject t = (GameObject)Instantiate(pondPrefab, tempV, Quaternion.identity);
                t.transform.parent = transform;
            }
        }

        mesh.vertices = verticies;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        this.gameObject.AddComponent<MeshCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
