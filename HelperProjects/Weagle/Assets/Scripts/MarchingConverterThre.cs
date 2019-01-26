using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MarchingConverterThreaded {
	
	// Just for the sake of convenience)
	struct  GridCell
	{
		public Vector3 [] P;
	}
	struct  Triangle
	{
		public  Vector3 [] P;
	}
	
	//private byte[,,] Gdata;
	// There are cells with the vertex coordinates
	private  GridCell [,,] _grids;
	// And then there are the triangles generated is stored for each cell
	private  Triangle [] _triangles;
	// The dimensions of the chunk
	private    int  _width;
	private    int  _height;
	private    int  _depth;
	private  Vector3 _transform;
	// Here are the values ​​(obtained by the noise)
	public    byte [,,] Gdata {get ; set ; }
	public  float[,,] Gdataf {get; set; }
	
	// Prepare _grids, _triangles etc.
	public    void  Init ( int width, int height, int depth, Vector3 transform)
	{
		_grids = new  GridCell [width, height, depth];
		_width = width;
		_height = height;
		_depth = depth;
		_transform = transform;
		
		for  ( int  i = 0; i <width; i ++)
			for  ( int  j = 0; j <height; j ++)
				for  ( int  k = 0; k <depth; k ++)
			{
				_grids [i, j, k].P = new  Vector3 [8];
				_grids [i, j, k].P [0] = new  Vector3 (i, j, k);
				_grids [i, j, k].P [1] = new  Vector3 (i + 1, j, k);
				_grids [i, j, k].P [2] = new  Vector3 (i + 1, j + 1, k);
				_grids [i, j, k].P [3] = new  Vector3 (i, j + 1, k);
				_grids [i, j, k].P [4] = new  Vector3 (i, j, k + 1);
				_grids [i, j, k].P [5] = new  Vector3 (i + 1, j, k + 1);
				_grids [i, j, k].P [6] = new  Vector3 (i + 1, j + 1, k + 1);
				_grids [i, j, k].P [7] = new  Vector3 (i, j + 1, k + 1);
			}
		_triangles = new  Triangle [16];
		for  ( int  i = 0; i <16; i ++)
		{
			_triangles [i] .P = new  Vector3 [3];
		}
	}
	public   void  Init ( int width, int height, int depth, Vector3 transform, byte [,,] gdata)
	{
		Init ( width,  height,  depth, transform);
		Gdata = gdata;
	}
	
	public   void  Init ( int width, int height, int depth, Vector3 transform, float [,,] gdata)
	{
		Init ( width,  height,  depth, transform);
		Gdataf = gdata;
	}
	
	// For convenience and avoid unnecessary calculation and allocation;)
	private  byte  GetVal ( int  x, int  y, int  z, int  i)
	{
		int l=x;
		int j=y;
		int k=z;
		try
		{
			
			switch  (i)
			{
			case  0:
			{
				break;
			}
			case  1:
			{
				l=x+1;
				break;
			}
			case  2:
			{
				l=x+1;
				j=y+1;
				break;
			}
			case  3:
			{
				j=y+1;
				break;
			}
			case  4:
			{
				k=z+1;
				break;
			}
			case  5:
			{
				l=x+1;
				k=z+1;
				break;
			}
			case  6:
			{
				l=x+1;
				j=y+1;
				k=z+1;
				break;
			}
			case  7:
			{
				j=y+1;
				k=z+1;
				break;
			}
			}
			return Gdata[l,j,k];
		}
		catch (Exception e)
		{
			return	ChunkManager.GetTheoreticalByte(new Vector3(l,j,k) + _transform);
		}
		
		return  0;
	}
	
	private  float  GetValf ( int  x, int  y, int  z, int  i)
	{
		int l=x;
		int j=y;
		int k=z;
		try
		{
			switch  (i)
			{
			case  0:
			{
				break;
			}
			case  1:
			{
				l=x+1;
				break;
			}
			case  2:
			{
				l=x+1;
				j=y+1;
				break;
			}
			case  3:
			{
				j=y+1;
				break;
			}
			case  4:
			{
				k=z+1;
				break;
			}
			case  5:
			{
				l=x+1;
				k=z+1;
				break;
			}
			case  6:
			{
				l=x+1;
				j=y+1;
				k=z+1;
				break;
			}
			case  7:
			{
				j=y+1;
				k=z+1;
				break;
			}
			}
			return Gdataf[l,j,k];
		}
		catch (Exception e)
		{
			return	ChunkManager.GetTheoreticalMesh(new Vector3(l,j,k) + _transform);
		}
		
		return  0;
	}
	private    Vector3 GetPos ( int  x, int  y, int  z, int  i)
	{
		return  _grids [x, y, z] .P [i];
	}
	
	//	public  static  MarchCubesPrimitive Process ( float  isolevel)
	//	{
	//		//var  RES = new  MarchCubesPrimitive ();
	//		Process ( isolevel, ref  RES);
	//		return  RES;
	//	}
	// This happens all the work. MarchCubesPrimitive - implementation GeometricPrimitive Example XNA. isolevel - level on the isosurface.
	public   void  Process ( float  isolevel, ref List<Vector3> verts, ref List<int> tris, ref List<Vector2> uvs )
	{
		
		int triNum  = 0 ;
		
		for  ( int  i = 0; i < _width; i ++)
			for  ( int  j= 0; j < _height; j ++)
				for  ( int  k = 0; k <_depth; k ++)
			{
				// Fill _triangles and get the number of triangles
				triNum = Polygonise (i, j, k, isolevel);
				if  (triNum> 0)
				{
					Build(ref verts,ref tris,ref uvs, triNum); // Here buffers
				}
			}
		//		if  (0 == primitive.CurrentVertex) primitive.isDrawable = false ;
		//		else  primitive.InitializePrimitive (GraphicsDevice);
	}
	
	public   void  Processf ( float  isolevel, ref List<Vector3> verts, ref List<int> tris, ref List<Vector2> uvs )
	{
		
		int triNum  = 0 ;
		
		for  ( int  i = 0; i < _width; i ++)
			for  ( int  j= 0; j < _height; j ++)
				for  ( int  k = 0; k <_depth; k ++)
			{
				// Fill _triangles and get the number of triangles
				triNum = Polygonisef (i, j, k, isolevel);
				if  (triNum> 0)
				{
					Buildf(ref verts,ref tris,ref uvs, triNum); // Here buffers
				}
			}
		//		if  (0 == primitive.CurrentVertex) primitive.isDrawable = false ;
		//		else  primitive.InitializePrimitive (GraphicsDevice);
	}
	
	int  Polygonise ( int  x, int  y, int  z, float  isolevel)
	{
		var  vertlist = new  Vector3 [12];
		int  i, ntriang = 0;
		byte  cubeindex = 0;
		// Define a cube in front of us (in fact, the index in the table)
		if  (GetVal (x, y, z, 0)> isolevel) cubeindex |= 1;
		if  (GetVal (x, y, z, 1)> isolevel) cubeindex |= 2;
		if  (GetVal (x, y, z, 2)> isolevel) cubeindex |= 4;
		if  (GetVal (x, y, z, 3)> isolevel) cubeindex |= 8;
		if  (GetVal (x, y, z, 4)> isolevel) cubeindex |= 16;
		if  (GetVal (x, y, z, 5)> isolevel) cubeindex |= 32;
		if  (GetVal (x, y, z, 6)> isolevel) cubeindex |= 64;
		if  (GetVal (x, y, z, 7)> isolevel) cubeindex |= 128;
		
		/* Cube is entirely in / out of the surface */
		if  (_edgeTable [cubeindex] == 0)
			return  0;
		
		// We are looking for specific provisions peaks using a linear interpolation
		/* Find the vertices where the surface intersects the cube */
		if  ((_edgeTable [cubeindex] & 1)> 0)
			vertlist [0] =  VertexInterp (isolevel, GetPos (x, y, z, 0), GetPos (x, y, z, 1), GetVal (x, y, z, 0), GetVal (x, y, z, 1));
		if  ((_edgeTable [cubeindex] & 2)> 0)
			vertlist [1] =	VertexInterp (isolevel, GetPos (x, y, z, 1), GetPos (x, y, z, 2), GetVal (x, y, z, 1), GetVal (x, y, z, 2));
		if  ((_edgeTable [cubeindex] & 4)> 0)
			vertlist [2] =	VertexInterp (isolevel, GetPos (x, y, z, 2), GetPos (x, y, z, 3), GetVal (x, y, z, 2), GetVal (x, y, z, 3));
		if  ((_edgeTable [cubeindex] & 8)> 0)
			vertlist [3] =	VertexInterp (isolevel, GetPos (x, y, z, 3), GetPos (x, y, z, 0), GetVal (x, y, z, 3), GetVal (x, y, z, 0));
		if  ((_edgeTable [cubeindex] & 16)> 0)
			vertlist [4] =	VertexInterp (isolevel, GetPos (x, y, z, 4), GetPos (x, y, z, 5), GetVal (x, y, z, 4), GetVal (x, y, z, 5));
		if  ((_edgeTable [cubeindex] & 32)> 0)
			vertlist [5] =	VertexInterp (isolevel, GetPos (x, y, z, 5), GetPos (x, y, z, 6), GetVal (x, y, z, 5), GetVal (x, y, z, 6));
		if  ((_edgeTable [cubeindex] & 64)> 0)
			vertlist [6] =	VertexInterp (isolevel, GetPos (x, y, z, 6), GetPos (x, y, z, 7), GetVal (x, y, z, 6), GetVal (x, y, z, 7));
		if  ((_edgeTable [cubeindex] & 128)> 0)
			vertlist [7] =	VertexInterp (isolevel, GetPos (x, y, z, 7), GetPos (x, y, z, 4), GetVal (x, y, z, 7), GetVal (x, y, z, 4));
		if  ((_edgeTable [cubeindex] & 256)> 0)
			vertlist [8] =  VertexInterp (isolevel, GetPos (x, y, z, 0), GetPos (x, y, z, 4), GetVal (x, y, z, 0), GetVal (x, y, z, 4));
		if  ((_edgeTable [cubeindex] & 512)> 0)
			vertlist [9] =	VertexInterp (isolevel, GetPos (x, y, z, 1), GetPos (x, y, z, 5), GetVal (x, y, z, 1), GetVal (x, y, z, 5));
		if  ((_edgeTable [cubeindex] & 1024)> 0)
			vertlist [10] =	VertexInterp (isolevel, GetPos (x, y, z, 2), GetPos (x, y, z, 6), GetVal (x, y, z, 2), GetVal (x, y, z, 6));
		if  ((_edgeTable [cubeindex] & 2048)> 0)
			vertlist [11] =	VertexInterp (isolevel, GetPos (x, y, z, 3), GetPos (x, y, z, 7), GetVal (x, y, z, 3), GetVal (x, y, z, 7));
		// Well, creating a triangle vertex indices take from _triTable, as determined according to the top of cubeindex  
		/* Create the triangle */
		for  (i = 0; _triTable [cubeindex, i] != -1; i += 3)
		{
			_triangles [ntriang] .P [0] = vertlist [_triTable [cubeindex, i]];
			_triangles [ntriang] .P [1] = vertlist [_triTable [cubeindex, i + 1]];
			_triangles [ntriang] .P [2] = vertlist [_triTable [cubeindex, i + 2]];
			ntriang ++;
		}
		return  ntriang;
	}
	
	int  Polygonisef ( int  x, int  y, int  z, float  isolevel)
	{
		var  vertlist = new  Vector3 [12];
		int  i, ntriang = 0;
		byte  cubeindex = 0;
		// Define a cube in front of us (in fact, the index in the table)
		if  (GetValf (x, y, z, 0)> isolevel) cubeindex |= 1;
		if  (GetValf (x, y, z, 1)> isolevel) cubeindex |= 2;
		if  (GetValf (x, y, z, 2)> isolevel) cubeindex |= 4;
		if  (GetValf (x, y, z, 3)> isolevel) cubeindex |= 8;
		if  (GetValf (x, y, z, 4)> isolevel) cubeindex |= 16;
		if  (GetValf (x, y, z, 5)> isolevel) cubeindex |= 32;
		if  (GetValf (x, y, z, 6)> isolevel) cubeindex |= 64;
		if  (GetValf (x, y, z, 7)> isolevel) cubeindex |= 128;
		
		/* Cube is entirely in / out of the surface */
		if  (_edgeTable [cubeindex] == 0)
			return  0;
		
		// We are looking for specific provisions peaks using a linear interpolation
		/* Find the vertices where the surface intersects the cube */
		if  ((_edgeTable [cubeindex] & 1)> 0)
			vertlist [0] =  VertexInterpf (isolevel, GetPos (x, y, z, 0), GetPos (x, y, z, 1), GetValf (x, y, z, 0), GetValf (x, y, z, 1));
		if  ((_edgeTable [cubeindex] & 2)> 0)
			vertlist [1] =	VertexInterpf (isolevel, GetPos (x, y, z, 1), GetPos (x, y, z, 2), GetValf (x, y, z, 1), GetValf (x, y, z, 2));
		if  ((_edgeTable [cubeindex] & 4)> 0)
			vertlist [2] =	VertexInterpf (isolevel, GetPos (x, y, z, 2), GetPos (x, y, z, 3), GetValf (x, y, z, 2), GetValf (x, y, z, 3));
		if  ((_edgeTable [cubeindex] & 8)> 0)
			vertlist [3] =	VertexInterpf (isolevel, GetPos (x, y, z, 3), GetPos (x, y, z, 0), GetValf (x, y, z, 3), GetValf (x, y, z, 0));
		if  ((_edgeTable [cubeindex] & 16)> 0)
			vertlist [4] =	VertexInterpf (isolevel, GetPos (x, y, z, 4), GetPos (x, y, z, 5), GetValf (x, y, z, 4), GetValf (x, y, z, 5));
		if  ((_edgeTable [cubeindex] & 32)> 0)
			vertlist [5] =	VertexInterpf (isolevel, GetPos (x, y, z, 5), GetPos (x, y, z, 6), GetValf (x, y, z, 5), GetValf (x, y, z, 6));
		if  ((_edgeTable [cubeindex] & 64)> 0)
			vertlist [6] =	VertexInterpf (isolevel, GetPos (x, y, z, 6), GetPos (x, y, z, 7), GetValf (x, y, z, 6), GetValf (x, y, z, 7));
		if  ((_edgeTable [cubeindex] & 128)> 0)
			vertlist [7] =	VertexInterpf (isolevel, GetPos (x, y, z, 7), GetPos (x, y, z, 4), GetValf (x, y, z, 7), GetValf (x, y, z, 4));
		if  ((_edgeTable [cubeindex] & 256)> 0)
			vertlist [8] =  VertexInterpf (isolevel, GetPos (x, y, z, 0), GetPos (x, y, z, 4), GetValf (x, y, z, 0), GetValf (x, y, z, 4));
		if  ((_edgeTable [cubeindex] & 512)> 0)
			vertlist [9] =	VertexInterpf (isolevel, GetPos (x, y, z, 1), GetPos (x, y, z, 5), GetValf (x, y, z, 1), GetValf (x, y, z, 5));
		if  ((_edgeTable [cubeindex] & 1024)> 0)
			vertlist [10] =	VertexInterpf (isolevel, GetPos (x, y, z, 2), GetPos (x, y, z, 6), GetValf (x, y, z, 2), GetValf (x, y, z, 6));
		if  ((_edgeTable [cubeindex] & 2048)> 0)
			vertlist [11] =	VertexInterpf (isolevel, GetPos (x, y, z, 3), GetPos (x, y, z, 7), GetValf (x, y, z, 3), GetValf (x, y, z, 7));
		// Well, creating a triangle vertex indices take from _triTable, as determined according to the top of cubeindex  
		/* Create the triangle */
		for  (i = 0; _triTable [cubeindex, i] != -1; i += 3)
		{
			_triangles [ntriang] .P [0] = vertlist [_triTable [cubeindex, i]];
			_triangles [ntriang] .P [1] = vertlist [_triTable [cubeindex, i + 1]];
			_triangles [ntriang] .P [2] = vertlist [_triTable [cubeindex, i + 2]];
			ntriang ++;
		}
		return  ntriang;
	}
	// Simple linear interpolation, probably not very fast, but the bottleneck is not here
	Vector3 VertexInterp ( float  isolevel, Vector3 p1, Vector3 p2, byte valp1, byte valp2)
	{
		float  mu;
		Vector3 P = new  Vector3 ();
		//		if  ((valp1 >0) && (valp2 >0))
		//			return  p1;
		//		if (valp1 > 1)
		//			return p1;
		//		if (valp2 > 1)
		//			return p2;
		//		
		//		if  (Mathf.Abs (isolevel - valp1) <0.00001)
		//			return  p1;
		//		if  (Mathf.Abs (isolevel - valp2) <0.00001)
		//			return  p2;
		//		if  (Mathf.Abs (valp1 - valp2) <0.00001)
		//			return  p1;
		//		mu = (isolevel - valp1) / (valp2 - valp1);
		//		P.x = ( float ) (p1.x + mu * (p2.x - p1.x));
		//		P.y = ( float ) (p1.y + mu * (p2.y - p1.y));
		//		P.z = ( float ) (p1.z + mu * (p2.z - p1.z));
		
		if (valp1 < valp2)
		{
			return p1;
		}
		else
		{
			return p2;
		}
		return  P;
	}
	
	Vector3 VertexInterpf ( float  isolevel, Vector3 p1, Vector3 p2, float valp1, float valp2)
	{
		float  mu;
		Vector3 P = new  Vector3 ();
		if  (Mathf.Abs (isolevel - valp1) <0.00001)
			return  p1;
		if  (Mathf.Abs (isolevel - valp2) <0.00001)
			return  p2;
		if  (Mathf.Abs (valp1 - valp2) <0.00001)
			return  p1;
		mu = (isolevel - valp1) / (valp2 - valp1);
		P.x = ( float ) (p1.x + mu * (p2.x - p1.x));
		P.y = ( float ) (p1.y + mu * (p2.y - p1.y));
		P.z = ( float ) (p1.z + mu * (p2.z - p1.z));	
		return  P;
	}	
	// Again, all trivial, move indexes and to the top of the received triangles and add to the clipboard object
	void  Build ( ref List<Vector3> verts, ref List<int> tris, ref List<Vector2> uvs, int triCount )
	{
		int index;
		
		for  ( int  i = 0; i <triCount; i ++)
		{
			index  = verts.Count;
			verts.Add (_triangles [i].P [0]);
			verts.Add (_triangles [i].P [2]);
			verts.Add (_triangles [i].P [1]);
			
			tris.Add(index + 0);
			tris.Add(index + 1);
			tris.Add(index + 2);
			
			// Consider a simple normal for all vertices of the triangle, one
			//	Vector3 normal = Vector3.Cross (_triangles[i].P [1] - _triangles[i].P [0], _triangles[i].P [2] - _triangles[i].P [0]);
			
			
			
			uvs.Add(new Vector2(0,0));
			uvs.Add(new Vector2(0,1));
			uvs.Add(new Vector2(1,1));
		}
	}
	
	void  Buildf ( ref List<Vector3> verts, ref List<int> tris, ref List<Vector2> uvs, int triCount )
	{
		int index;
		
		for  ( int  i = 0; i <triCount; i ++)
		{
			index  = verts.Count;
			verts.Add (_triangles [i].P [0]);
			verts.Add (_triangles [i].P [2]);
			verts.Add (_triangles [i].P [1]);
			
			tris.Add(index + 0);
			tris.Add(index + 1);
			tris.Add(index + 2);
			
			// Consider a simple normal for all vertices of the triangle, one
			//	Vector3 normal = Vector3.Cross (_triangles[i].P [1] - _triangles[i].P [0], _triangles[i].P [2] - _triangles[i].P [0]);
			
			
			
			uvs.Add(new Vector2(0,0));
			uvs.Add(new Vector2(0,1));
			uvs.Add(new Vector2(1,1));
		}
	}
	
	// Determine the index into the edge table which
	// Tells us which vertices are inside of the surface
	// A pair of tables on them define the configuration of triangles =)
	static  int [] _edgeTable = new  int [256] {
		0x0, 0x109, 0x203, 0x30a, 0x406, 0x50f, 0x605, 0x70c,
		0x80c, 0x905, 0xa0f, 0xb06, 0xc0a, 0xd03, 0xe09, 0xf00,
		0x190, 0x99, 0x393, 0x29a, 0x596, 0x49f, 0x795, 0x69c,
		0x99c, 0x895, 0xb9f, 0xa96, 0xd9a, 0xc93, 0xf99, 0xe90,
		0x230, 0x339, 0x33, 0x13a, 0x636, 0x73f, 0x435, 0x53c,
		0xa3c, 0xb35, 0x83f, 0x936, 0xe3a, 0xf33, 0xc39, 0xd30,
		0x3a0, 0x2a9, 0x1a3, 0xaa, 0x7a6, 0x6af, 0x5a5, 0x4ac,
		0xbac, 0xaa5, 0x9af, 0x8a6, 0xfaa, 0xea3, 0xda9, 0xca0,
		0x460, 0x569, 0x663, 0x76a, 0x66, 0x16f, 0x265, 0x36c,
		0xc6c, 0xd65, 0xe6f, 0xf66, 0x86a, 0x963, 0xa69, 0xb60,
		0x5f0, 0x4f9, 0x7f3, 0x6fa, 0x1f6, 0xff, 0x3f5, 0x2fc,
		0xdfc, 0xcf5, 0xfff, 0xef6, 0x9fa, 0x8f3, 0xbf9, 0xaf0,
		0x650, 0x759, 0x453, 0x55a, 0x256, 0x35f, 0x55, 0x15c,
		0xe5c, 0xf55, 0xc5f, 0xd56, 0xa5a, 0xb53, 0x859, 0x950,
		0x7c0, 0x6c9, 0x5c3, 0x4ca, 0x3c6, 0x2cf, 0x1c5, 0xcc,
		0xfcc, 0xec5, 0xdcf, 0xcc6, 0xbca, 0xac3, 0x9c9, 0x8c0,
		0x8c0, 0x9c9, 0xac3, 0xbca, 0xcc6, 0xdcf, 0xec5, 0xfcc,
		0xcc, 0x1c5, 0x2cf, 0x3c6, 0x4ca, 0x5c3, 0x6c9, 0x7c0,
		0x950, 0x859, 0xb53, 0xa5a, 0xd56, 0xc5f, 0xf55, 0xe5c,
		0x15c, 0x55, 0x35f, 0x256, 0x55a, 0x453, 0x759, 0x650,
		0xaf0, 0xbf9, 0x8f3, 0x9fa, 0xef6, 0xfff, 0xcf5, 0xdfc,
		0x2fc, 0x3f5, 0xff, 0x1f6, 0x6fa, 0x7f3, 0x4f9, 0x5f0,
		0xb60, 0xa69, 0x963, 0x86a, 0xf66, 0xe6f, 0xd65, 0xc6c,
		0x36c, 0x265, 0x16f, 0x66, 0x76a, 0x663, 0x569, 0x460,
		0xca0, 0xda9, 0xea3, 0xfaa, 0x8a6, 0x9af, 0xaa5, 0xbac,
		0x4ac, 0x5a5, 0x6af, 0x7a6, 0xaa, 0x1a3, 0x2a9, 0x3a0,
		0xd30, 0xc39, 0xf33, 0xe3a, 0x936, 0x83f, 0xb35, 0xa3c,
		0x53c, 0x435, 0x73f, 0x636, 0x13a, 0x33, 0x339, 0x230,
		0xe90, 0xf99, 0xc93, 0xd9a, 0xa96, 0xb9f, 0x895, 0x99c,
		0x69c, 0x795, 0x49f, 0x596, 0x29a, 0x393, 0x99, 0x190,
		0xf00, 0xe09, 0xd03, 0xc0a, 0xb06, 0xa0f, 0x905, 0x80c,
		0x70c, 0x605, 0x50f, 0x406, 0x30a, 0x203, 0x109, 0x0
	};
	
	static  int [,] _triTable = new  int [256, 16]
	{{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1},
		{3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1},
		{3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1},
		{3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1},
		{9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1},
		{1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1},
		{9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
		{2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1},
		{8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1},
		{9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
		{4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1},
		{3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1},
		{1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1},
		{4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1},
		{4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1},
		{9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1},
		{1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
		{5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1},
		{2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1},
		{9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
		{0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
		{2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1},
		{10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1},
		{4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1},
		{5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1},
		{5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1},
		{9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1},
		{0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1},
		{1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1},
		{10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1},
		{8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1},
		{2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1},
		{7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1},
		{9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1},
		{2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1},
		{11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1},
		{9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1},
		{5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1},
		{11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1},
		{11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
		{1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1},
		{9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1},
		{5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1},
		{2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
		{0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
		{5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1},
		{6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1},
		{0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1},
		{3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1},
		{6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1},
		{5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1},
		{1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
		{10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1},
		{6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1},
		{1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1},
		{8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1},
		{7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1},
		{3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
		{5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1},
		{0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1},
		{9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1},
		{8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1},
		{5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1},
		{0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1},
		{6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1},
		{10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1},
		{10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1},
		{8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1},
		{1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1},
		{3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1},
		{0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1},
		{10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1},
		{0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1},
		{3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1},
		{6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1},
		{9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1},
		{8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1},
		{3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1},
		{6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1},
		{0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1},
		{10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1},
		{10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1},
		{1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1},
		{2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1},
		{7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1},
		{7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1},
		{2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1},
		{1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1},
		{11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1},
		{8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1},
		{0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1},
		{7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
		{10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
		{2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
		{6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1},
		{7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1},
		{2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1},
		{1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1},
		{10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1},
		{10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1},
		{0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1},
		{7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1},
		{6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1},
		{8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1},
		{9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1},
		{6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1},
		{1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1},
		{4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1},
		{10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1},
		{8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1},
		{0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1},
		{1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1},
		{8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1},
		{10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1},
		{4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1},
		{10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
		{5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
		{11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1},
		{9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
		{6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1},
		{7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1},
		{3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1},
		{7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1},
		{9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1},
		{3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1},
		{6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1},
		{9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1},
		{1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1},
		{4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1},
		{7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1},
		{6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1},
		{3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1},
		{0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1},
		{6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1},
		{1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1},
		{0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1},
		{11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1},
		{6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1},
		{5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1},
		{9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1},
		{1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1},
		{1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1},
		{10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1},
		{0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1},
		{5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1},
		{10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1},
		{11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1},
		{0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1},
		{9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1},
		{7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1},
		{2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1},
		{8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1},
		{9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1},
		{9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1},
		{1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1},
		{9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1},
		{9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1},
		{5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1},
		{0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1},
		{10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1},
		{2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1},
		{0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1},
		{0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1},
		{9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1},
		{5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1},
		{3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1},
		{5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1},
		{8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1},
		{0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1},
		{9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1},
		{0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1},
		{1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1},
		{3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1},
		{4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1},
		{9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1},
		{11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1},
		{11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1},
		{2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1},
		{9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1},
		{3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1},
		{1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1},
		{4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1},
		{4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1},
		{0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1},
		{3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1},
		{3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1},
		{0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1},
		{9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1},
		{1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
		{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};
}





