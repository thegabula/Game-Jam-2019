using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelChunk : MonoBehaviour {

    public int width = 64;
    public int height = 64;
    public int depth = 64;

    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    public byte[] map;
    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();

    public void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        map = new byte[width * height*depth];
       
    }

    public void SetMap(byte [,,] newMap)
    {
        int index =0;
        width = newMap.GetUpperBound(0) +1;
        height = newMap.GetUpperBound(1) +1;
        depth = newMap.GetUpperBound(2)+1;

        byte[] tempMap = new byte[newMap.Length];
        for (int z = 0; z<depth; z++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tempMap[index] = newMap[x, y, z];
                    index++;
                }
            }
        }
        map = tempMap;
        CalculateMesh();
    }

    [ContextMenu("Calculate Mesh")]
    public void CalculateMesh()
    {
        verts = new List<Vector3>();
        tris = new List<int>();

        int index = 0;
        for(int z = 0; z< depth; z++)
        {
           for(int y = 0; y< height; y++)
            {
                for (int x = 0; x< width; x++)
                {
                    switch(map[index])
                    {
                        default:
                            break;                        
                        case 1:
                            CreateVoxel(map[index], x, y, z);
                            break;
                    }

                    // flat increment the index
                    index++;
                }
            }
        }


        Mesh mesh = new Mesh();        
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    /// <summary>
    /// Return wheter or not the block in question is opaque
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public bool IsOpaque(int x, int y, int z)
    {
        if ((x < 0) || (y < 0) || (z < 0) || (x >= width) || (y >= height) || (z >= depth))
            return false;


        int index = x + y * width + z * width * height;
        switch (map[index])
        {
            default:
                return false;
            case 1:
                return true;
        }

    }

    public void CreateVoxel(byte material, int x, int y, int z)
    {
        Vector3 corner = new Vector3(x, y, z);

        if(!IsOpaque(x,y,z-1))
            CreateVoxelFace(corner, Vector3.right, Vector3.up*3);
        if (!IsOpaque(x, y, z + 1))
            CreateVoxelFace(corner + Vector3.forward + Vector3.right, -Vector3.right, Vector3.up*3);
        if (!IsOpaque(x+1, y, z))
            CreateVoxelFace(corner+Vector3.right, Vector3.forward, Vector3.up*3);
        if (!IsOpaque(x-1, y, z))
            CreateVoxelFace(corner + Vector3.forward, -Vector3.forward, Vector3.up*3);
        if (!IsOpaque(x, y-1, z))
            CreateVoxelFace(corner, Vector3.forward, Vector3.right);
        if (!IsOpaque(x, y+1, z))
            CreateVoxelFace(corner + Vector3.up*3 + Vector3.forward, -Vector3.forward, Vector3.right);
    }

    public void CreateVoxelFace(Vector3 corner, Vector3 right, Vector3 up)
    {
        int index = verts.Count;

        verts.Add(corner);
        verts.Add(corner + right);
        verts.Add(corner + right + up);
        verts.Add(corner + up);

        tris.Add(index+1);
        tris.Add(index );
        tris.Add(index + 2);

        tris.Add(index + 3);
        tris.Add(index + 2);
        tris.Add(index);
    }
}
