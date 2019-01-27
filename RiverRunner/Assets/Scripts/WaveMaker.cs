using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMaker : MonoBehaviour
{
    public float noiseScale = 0.51f;
    public float noiseSpeed = -0.6f;
    public float noiseStrength = 1f;
    public float noiseWalk = 3f;
    public float largeNoiseScale = 0.51f;
    public float largeNoiseSpeed = -0.6f;
    public float largeNoiseStrength = 1f;

    public int width = 1;
    public int depth = 1;
    public int widthDivisions = 2;
    public int depthDivisions = 2;
    public static bool Flowing = false;

    public float foamSpeed = 0.025f;

    private Vector3[] baseHeight;
    private List<Vector3> verticies;
    private List<int> tris;
    private List<Vector2> uvs;

    public float maxY = 0;
    public float minY = 0;
    public float DebugDelta = 0;

    public GameObject holderPrefab;
    public GameObject[] tempHolders;

    MeshCollider mc;

    // Use this for initialization
    void Start()
    {
        mc = GetComponent<MeshCollider>();
        int i, j;
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        verticies = new List<Vector3>();
        tris = new List<int>();
        uvs = new List<Vector2>();
        Vector3[] tempGrid = new Vector3[widthDivisions * depthDivisions + 1];
        Vector2[] tempUvs = new Vector2[widthDivisions * depthDivisions + 1];
        // tempHolders = new GameObject[widthDivisions * depthDivisions +1 ];
        int index = 0;
        Vector2 uvUnit = new Vector2(1 / widthDivisions, 1 / depthDivisions);
        float xStep = width / (float)widthDivisions;
        float zStep = depth / (float)depthDivisions;

        for (i = 0; i < widthDivisions; i++)
        {
            for (j = 0; j < depthDivisions; j++)
            {
                Vector3 vertex = new Vector3(((-width / 2f)) + i * xStep, 0, ((-depth / 2f)) + j * zStep);
                tempGrid[index] = vertex;
                //tempHolders[index] = (GameObject)Instantiate(holderPrefab, (vertex+transform.position), Quaternion.identity);
                index++;
                Vector2 uv = new Vector2((float)(i / (float)widthDivisions), (float)(j / (float)depthDivisions));
                tempUvs[index] = uv;
                // BuildFace(vertex, Vector3.right, Vector3.forward, uv, uvUnit);
            }
        }
        // tempHolders[tempHolders.Length - 1] = holderPrefab;
        for (i = 0; i < widthDivisions - 1; i++)
        {
            for (j = 0; j < depthDivisions - 1; j++)
            {
                BuildFace(tempGrid, i * widthDivisions + j, (i + 1) * widthDivisions + j, (i + 1) * widthDivisions + (j + 1), i * widthDivisions + (j + 1));
            }
        }

        //mesh.vertices = verticies.ToArray();
        mesh.vertices = tempGrid;
        mesh.triangles = tris.ToArray();
        //mesh.uv = uvs.ToArray();
        mesh.uv = tempUvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();        
    }

    // Update is called once per frame
    void Update()
    {
        if (Flowing)
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            Vector2 tempOffset = mr.material.GetVector("_MainTex");
            float tempY = tempOffset.y;
            float offset = (Time.deltaTime * foamSpeed) % 1.0f;
            mr.material.SetVector("_MainTex", new Vector2(0, (offset + tempY) % 1.0f));
            //    mr.material.SetFloat("_FlowSpeed", (offset ) % 1.0f);
            //    mr.material.SetFloat("_Speed", (offset) % 1.0f);
        }
        else
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            Vector2 tempOffset = mr.material.GetVector("_MainTex");
            float tempY = tempOffset.y;
            float offset = (Time.deltaTime * 0.5f);
            mr.material.SetVector("_MainTex", new Vector2(0, (offset + tempY) % 1.0f));
            //    mr.material.SetFloat("_FlowSpeed", (offset) % 1.0f);
            //    mr.material.SetFloat("_Speed", (offset) % 1.0f);----------****
        }

    }

    private void FixedUpdate()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector2[] uvs = mesh.uv;
        // Position of parent game object
        Vector3 ParentPos = GetComponentInParent<Transform>().transform.position;
        MapGenerator tempMap = MapGenerator.findChunk(ParentPos);

        if (tempMap != null)
        {
            // Position of rock grid
            Vector3 RockGridPos = tempMap.transform.position;

            // Position of Water Plane
            Vector3 WaterPos = transform.position;

            byte[,] rockFields = tempMap.GetPushLayer();
            Vector2 TransformedCoords;

            if (baseHeight == null)
                baseHeight = mesh.vertices;

            Vector3[] vertices = new Vector3[baseHeight.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = baseHeight[i];
                vertex.y += Mathf.PerlinNoise(ParentPos.x / 3 + baseHeight[i].x * (largeNoiseSpeed), (ParentPos.z / 3 * largeNoiseScale + baseHeight[i].z) * (largeNoiseSpeed)) * largeNoiseStrength;

                if (vertex.y > maxY)
                {
                    maxY = vertex.y;
                }
                if (vertex.y < minY)
                {
                    minY = vertex.y;
                }
                vertices[i] = vertex;
            }

            mesh.vertices = vertices;
            mesh.RecalculateNormals();

            mc.sharedMesh = mesh;

        }
        //Mesh mesh = GetComponent<MeshFilter>().mesh;
        //Vector2[] uvs = mesh.uv;
        //for (int i = 0; i < mesh.uv.Length; i++)
        //{ 
        //    uvs[i].y += Time.deltaTime*0.1f;
        //}
        //mesh.uv = uvs;
        /*if (Flowing)
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            Vector2 tempOffset = mr.material.GetTextureOffset("_MainTex");
            float tempY = tempOffset.y;
            float offset = (Time.deltaTime * foamSpeed) % 1.0f;
            mr.material.SetTextureOffset("_MainTex", new Vector2(0, (offset + tempY) % 1.0f));
        }
        else
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            Vector2 tempOffset = mr.material.GetTextureOffset("_MainTex");
            float tempY = tempOffset.y;
            float offset = (Time.deltaTime * -0.5f);
            mr.material.SetTextureOffset("_MainTex", new Vector2(0, (offset+tempY)%1.0f));
        }
        */
       


    }
    void BuildFace(Vector3[] grid, int p1, int p2, int p3, int p4)
    {
        tris.Add(p1);
        tris.Add(p4);
        tris.Add(p3);
        tris.Add(p3);
        tris.Add(p2);
        tris.Add(p1);

        //uvs.Add(uv);
        //uvs.Add(new Vector2(uv.x, uv.y + uvUnit.y));
        //uvs.Add(new Vector2(uv.x + uvUnit.x, uv.y + uvUnit.y));
        //uvs.Add(new Vector2(uv.x + uvUnit.x, uv.y));
    }

    public float GetWaterHeight(Vector3 pos)
    {
        float result;
        float dist = 9999;
        float mag = 0;
        int index = 0;
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        if (baseHeight == null)
            baseHeight = mesh.vertices;


        Vector2 barrelCoord = new Vector2(pos.x, pos.z); // world coordinates
        Vector2 vertexCoord;
        Vector3 ParentPos = transform.position;
        Vector3 temp;
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            Vector3 tempV;
            temp = ParentPos + vertex;
            tempV = temp - pos;
            vertexCoord = new Vector2(temp.x, temp.z);
            mag = (vertexCoord - barrelCoord).magnitude;
            if (mag < dist)
            {
                dist = mag;
                index = i;
            }
        }

        result = vertices[index].y;
        //Debug.Log("hight " + result + " from " + index);
        return result;
    }

}