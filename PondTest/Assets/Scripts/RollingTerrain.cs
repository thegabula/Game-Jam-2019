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


    // Start is called before the first frame update
    void Start()
    {
        int i, j, index;
        float y;

        int heightScale = 5;
        float detailScale = 5.0f;

        mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] verticies = mesh.vertices; //new Vector3[width * depth] ;

        index = 0;
        for (i = 0; i < verticies.Length; i++)
        {
            verticies[i].y = Mathf.PerlinNoise((verticies[i].x  + this.transform.position.x)/detailScale, (verticies[i].z + this.transform.position.z)/detailScale)* heightScale;
  
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
