using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Octree <T> :ThreadedJob  
{
	public Vector3[] InData;
	public Vector3[] OutData;
	private OctreeNode<T> root;
	private List<T> OutlierItems;
	private List<Vector3> OutlierPositions;
	private int maxOutliers;
	//private byte[,,]  map;

//	public void SetMap(byte[] theMap)
//	{
//		map = theMap;
//	}

	public Octree(Vector3 pos, float length)
	{
		maxOutliers = 6;
		root = new OctreeNode<T>(pos, length, null);
		OutlierItems = new List<T>();
		OutlierPositions = new List<Vector3>();
	}

	public bool Add(Vector3 position, T item)
	{
		if (root.Contains (position)) 
		{
			root.Add (position, item);
		} 
		else 
		{
			if (OutlierItems.Count < maxOutliers)
			{
				OutlierItems.Add(item);
				OutlierPositions.Add(position);
			}
			else
			{
				OutlierItems.Add(item);
				OutlierPositions.Add(position);
				root = root.UpRoot(ref OutlierItems, ref OutlierPositions);
			}
		}

		return true;
	}

	public void DebugShow()
	{
		root.DebugShow ();
		for (int i=0; i<OutlierItems.Count; i++) 
		{
			Debug.DrawRay(OutlierPositions[i],Vector3.up+Vector3.right,Color.green);
		}
	}

//	public bool Update(T item)
//	{
//		return true;
//	}

	protected override void ThreadFunction1()
	{
		for (int i=0; i< 100000; i++) 
		{
			InData[i%InData.Length] += InData[(i%1) % InData.Length];
		}
	}

	protected override void OnFinished()
	{
		for (int i =0; i< InData.Length; i++) 
		{
			Debug.Log("Results(" + i + "): " + InData[i]);
		}
	}

	public void Remove(Vector3 pos, T item)
	{
		int i;

		if (root.Contains (pos)) 
		{
			root.Remove(pos, item);  // (pos,item)
		} 
		else 
		{
			for (i=0;i<OutlierItems.Count;i++)
			{
				if (OutlierItems[i].Equals(item))
				{
					OutlierPositions.RemoveAt(i);
					OutlierItems.RemoveAt(i);
					break;
				}
			}
		}
	}
}
 
public class OctreeNode<T>
{
	public int minThreshold = 2;
	public int maxThreshold = 5;
	public bool isLeaf = false;
	public OctreeNode<T> parentNode;
	public int encompassCount;

	public List<T> contents;
	public List<Vector3> contentPositions;

	public OctreeNode<T>[] child;
	public Vector3 currentPosition;
	public float length;

	// Old Remove function
//	public void Remove(Vector3 pos)
//	{
//		int i;
//		OctreeNode<T> tempNode = Find (pos);
//		if (tempNode != null) 
//		{
//			for (i=0;i<tempNode.contentPositions.Count;i++)
//			{
//				if (tempNode.contentPositions[i] == pos)
//				{
//					tempNode.contentPositions.RemoveAt(i);
//					tempNode.contents.RemoveAt(i);
//
//					while(tempNode != null)
//					{
//						tempNode.encompassCount--;
//						tempNode = tempNode.GetParent();					
//					}

//	//				if (contents.Count < minThreshold)
//	//				{
//  //
//	//					tempNode.isLeaf = true;
//	//				}
//				}
//			}
//		}
	
//	}

	public bool Remove(Vector3 position, T item)
	{
		int i,j;
		Vector3 temp;
		bool success = false;
		
		if (Contains (position)) 
		{
			if (isLeaf) 
			{
				for (i=0;i<contentPositions.Count;i++)
				{
					if (contentPositions[i] == position)
					{
						contentPositions.RemoveAt(i);
						contents.RemoveAt(i);
					}
				}

				Debug.Log ("Removing " + position.ToString () + " from " + currentPosition.ToString ());
			} 
						 
			else 
			{
				temp = position - currentPosition;
				if (temp.x < 0) {          //-
					if (temp.y < 0) {     //--
						if (temp.z < 0) {  //---
							j = 7;
						} else {            //--+
							j = 6;
						}
					} else {                //-+
						if (temp.z < 0) { //-+-
							j = 5;
						} else {            //-++
							j = 4;
						}
					}
				} else {  //+
					if (temp.y < 0) {     //+-
						if (temp.z < 0) {  //+--
							j = 3;
						} else {            //+-+
							j = 2;
						}
					} else {                //++
						if (temp.z < 0) { //++-
							j = 1;
						} else {            //+++
							j = 0;
						}
					}
				}
				
				Debug.Log ("Going into child " + j);
				success = child [j].Remove(position, item);
			}
			if ((success)&&(!isLeaf))
			{
				encompassCount--;
				if(encompassCount < maxThreshold )
				{
					OctreeNode<T> tempNode;
					// Collapse Node
					for(i =0;i<8;i++)
					{
						tempNode = child[i];
						for(j = 0;j<tempNode.contents.Count;j++)
						{
							contents.Add(tempNode.contents[j]);
							contentPositions.Add(tempNode.contentPositions[j]);
						}
						tempNode.contents.Clear();
						tempNode.contentPositions.Clear();
						tempNode = null;
					}
					isLeaf = true;
				}
			}
		} 
		else 
		{
			return false;
		}
		return true;
	
	}
	public OctreeNode<T> GetParent()
	{
		return parentNode;
	}
	public OctreeNode<T> Find(Vector3 pos)
	{
		Vector3 temp;
		int j;

		if (isLeaf) 
		{
			for (j=0;j<contentPositions.Count;j++)
			{
				if (contentPositions[j] == pos)
				{
					return this;
				}
			}
		}
		else
		{
			temp = pos - currentPosition;
			if (temp.x < 0) {          //-
				if (temp.y < 0) {     //--
					if (temp.z < 0) {  //---
						j = 7;
					} else {            //--+
						j = 6;
					}
				} else {                //-+
					if (temp.z < 0) { //-+-
						j = 5;
					} else {            //-++
						j = 4;
					}
				}
			} else {  //+
				if (temp.y < 0) {     //+-
					if (temp.z < 0) {  //+--
						j = 3;
					} else {            //+-+
						j = 2;
					}
				} else {                //++
					if (temp.z < 0) { //++-
						j = 1;
					} else {            //+++
						j = 0;
					}
				}
			}
		
			Debug.Log ("Found in child " + j);
			return child [j].Find(pos);
		}

		return null;
	}
	public OctreeNode<T> UpRoot(ref List<T> outlierItems, ref List<Vector3> outlierPos)
	{
		Vector3 temp;
		OctreeNode<T> tempNode;
		OctreeNode<T> holderNode;

		int i,j;

		temp = outlierPos[0] - currentPosition;
		if (temp.x < 0) {          //-
			if (temp.y < 0) {      //--
				if (temp.z < 0) {  //---
					temp = currentPosition + new Vector3 (-length / 2, -length / 2, -length / 2);
					tempNode = new OctreeNode<T> (temp, length*2, null);
					tempNode.BreakUp ();
					tempNode.child [0] = this;
				} else {            //--+
					temp = currentPosition + new Vector3 (-length / 2, -length / 2, length / 2);
					tempNode = new OctreeNode<T> (temp, length*2, null);
					tempNode.BreakUp ();
					tempNode.child [1] = this;
				}
			} else {              //-+
				if (temp.z < 0) { //-+-
					temp = currentPosition + new Vector3 (-length / 2, length / 2, -length / 2);
					tempNode = new OctreeNode<T> (temp, length*2, null);
					tempNode.BreakUp ();
					tempNode.child [2] = this;
				} else {            //-++
					temp = currentPosition + new Vector3 (-length / 2, length / 2, length / 2);
					tempNode = new OctreeNode<T> (temp, length*2, null);
					tempNode.BreakUp ();
					tempNode.child [3] = this;
				}
			}
		} else {                   //+
			if (temp.y < 0) {      //+-
				if (temp.z < 0) {  //+--
					temp = currentPosition + new Vector3 (length / 2, -length / 2, -length / 2);
					tempNode = new OctreeNode<T> (temp, length*2, null);
					tempNode.BreakUp ();
					tempNode.child [4] = this;
				} else {            //+-+
					temp = currentPosition + new Vector3 (length / 2, -length / 2, length / 2);
					tempNode = new OctreeNode<T> (temp, length*2, null);
					tempNode.BreakUp ();
					tempNode.child [5] = this;
				}
			} else {              //++
				if (temp.z < 0) { //++-
					temp = currentPosition + new Vector3 (length / 2, length / 2, -length / 2);
					tempNode = new OctreeNode<T> (temp, length*2, null);
					tempNode.BreakUp ();
					tempNode.child [6] = this;
				} else {           //+++
					temp = currentPosition + new Vector3 (length / 2, length / 2, length / 2);
					tempNode = new OctreeNode<T> (temp, length*2, null);
					tempNode.BreakUp ();
					tempNode.child [7] = this;
				}
			}
			
		}
        this.parentNode = tempNode;

        for (i = outlierItems.Count - 1; i >= 0; i--)
        {
            if (tempNode.Contains(outlierPos[i]))
            {
                tempNode.Add(outlierPos[i], outlierItems[i]);
                outlierPos.RemoveAt(i);
                outlierItems.RemoveAt(i);
            }
        }
        return tempNode;
	}

	private void BreakUp()
	{
		Vector3 temp;
		child = new OctreeNode<T>[8];

		float halfX = length / 4.0f;
		float halfY = length / 4.0f;
		float halfZ = length / 4.0f;
		
		temp = new Vector3 (halfX, halfY, halfZ);
		temp = temp + currentPosition;
		child [0] = new OctreeNode<T> (temp, halfX * 2, this);
		Debug.Log ("Created " + temp.ToString ());
		
		temp = new Vector3 (halfX, halfY, -halfZ);
		temp = temp + currentPosition;
		child [1] = new OctreeNode<T> (temp, halfX * 2, this);
		Debug.Log ("Created " + temp.ToString ());
		
		temp = new Vector3 (halfX, -halfY, halfZ);
		temp = temp + currentPosition;
		child [2] = new OctreeNode<T> (temp, halfX * 2, this);
		Debug.Log ("Created " + temp.ToString ());
		
		temp = new Vector3 (halfX, -halfY, -halfZ);
		temp = temp + currentPosition;
		child [3] = new OctreeNode<T> (temp, halfX * 2, this);
		Debug.Log ("Created " + temp.ToString ());
		
		temp = new Vector3 (-halfX, halfY, halfZ);
		temp = temp + currentPosition;
		child [4] = new OctreeNode<T> (temp, halfX * 2, this);
		Debug.Log ("Created " + temp.ToString ());
		
		temp = new Vector3 (-halfX, halfY, -halfZ);
		temp = temp + currentPosition;
		child [5] = new OctreeNode<T> (temp, halfX * 2, this);
		Debug.Log ("Created " + temp.ToString ());
		
		temp = new Vector3 (-halfX, -halfY, halfZ);
		temp = temp + currentPosition;
		child [6] = new OctreeNode<T> (temp, halfX * 2, this);
		Debug.Log ("Created " + temp.ToString ());
		
		temp = new Vector3 (-halfX, -halfY, -halfZ);
		temp = temp + currentPosition;
		child [7] = new OctreeNode<T> (temp, halfX * 2, this);
		Debug.Log ("Created " + temp.ToString ());

		isLeaf = false;
	}
	
	public bool Add(Vector3 position, T item)
	{
		int i,j;
		Vector3 temp;
		
		if (Contains (position)) 
		{
			encompassCount++;
			if (isLeaf) 
			{
				if (contents.Count < maxThreshold) 
				{
					contents.Add (item);
					contentPositions.Add (position);
					Debug.Log ("Adding " + position.ToString () + " to " + currentPosition.ToString ());
				} 
				else 
				{
					contents.Add (item);
					contentPositions.Add (position);
					BreakUp();

					for (i=0; i<contents.Count; i++) 
					{
						temp = contentPositions [i] - currentPosition;
						if (temp.x < 0) {          //-
							if (temp.y < 0) {     //--
								if (temp.z < 0) {  //---
									j = 7;
								} else {            //--+
									j = 6;
								}
							} else {                //-+
								if (temp.z < 0) { //-+-
									j = 5;
								} else {            //-++
									j = 4;
								}
							}
						} else {  //+
							if (temp.y < 0) {     //+-
								if (temp.z < 0) {  //+--
									j = 3;
								} else {            //+-+
									j = 2;
								}
							} else {                //++
								if (temp.z < 0) { //++-
									j = 1;
								} else {            //+++
									j = 0;
								}
							}
						}

						Debug.Log ("Child " + j);
						child [j].Add (contentPositions [i], contents [i]);
					}
					contents.Clear ();
					contentPositions.Clear ();
					isLeaf = false;

				}
			} 
			else 
			{
				temp = position - currentPosition;
				if (temp.x < 0) {          //-
					if (temp.y < 0) {     //--
						if (temp.z < 0) {  //---
							j = 7;
						} else {            //--+
							j = 6;
						}
					} else {                //-+
						if (temp.z < 0) { //-+-
							j = 5;
						} else {            //-++
							j = 4;
						}
					}
				} else {  //+
					if (temp.y < 0) {     //+-
						if (temp.z < 0) {  //+--
							j = 3;
						} else {            //+-+
							j = 2;
						}
					} else {                //++
						if (temp.z < 0) { //++-
							j = 1;
						} else {            //+++
							j = 0;
						}
					}
				}

				Debug.Log ("Going into child " + j);
				return child [j].Add (position, item);
			}
		} 
		else 
		{
			return false;
		}
		return true;
	}

	public bool Contains(Vector3 pos)
	{
		Vector3 TopRight;
		Vector3 BottomLeft;

		TopRight = new Vector3 (length / 2.0f, length / 2.0f, length / 2.0f) + currentPosition;
		BottomLeft = new Vector3 (-length / 2.0f, -length / 2.0f, -length / 2.0f) + currentPosition;

		if ((pos.x <= TopRight.x) && (pos.x >= BottomLeft.x)) 
		{
			if ((pos.z <= TopRight.z) && (pos.z >= BottomLeft.z)) 
			{
				if ((pos.y <= TopRight.y) && (pos.y >= BottomLeft.y)) 
				{
					return true;
				}
			}
		}
		return false;
	}

	public void SetPosition(Vector3 pos)
	{
		currentPosition = pos;
	}

	public OctreeNode(Vector3 pos, float width, OctreeNode<T> parent)
	{
		contents  = new List<T>();
		contentPositions = new List<Vector3>();

		currentPosition = pos;
		length = width;
		isLeaf = true;
		encompassCount = 0;

		if (parent != null) 
		{
			parentNode = parent;
		}

	}

	public void DebugShow()
	{
		int i;
		float halfLength = length/2.0f;
		if (isLeaf)
		{
			if (contents.Count>0)
			{
				Debug.DrawLine(new Vector3(-halfLength,-halfLength,-halfLength) + currentPosition, new Vector3(-halfLength,-halfLength,halfLength)+currentPosition);
				Debug.DrawLine(new Vector3(-halfLength,-halfLength,halfLength) + currentPosition, new Vector3(-halfLength,halfLength,halfLength)+currentPosition);
				Debug.DrawLine(new Vector3(-halfLength,halfLength,halfLength) + currentPosition, new Vector3(-halfLength,halfLength,-halfLength)+currentPosition);
				Debug.DrawLine(new Vector3(-halfLength,halfLength,-halfLength) + currentPosition, new Vector3(-halfLength,-halfLength,-halfLength)+currentPosition);

				Debug.DrawLine(new Vector3(-halfLength,-halfLength,-halfLength) + currentPosition, new Vector3(halfLength,-halfLength,-halfLength)+currentPosition);
				Debug.DrawLine(new Vector3(-halfLength,-halfLength,halfLength) + currentPosition, new Vector3(halfLength,-halfLength,halfLength)+currentPosition);
				Debug.DrawLine(new Vector3(-halfLength,halfLength,halfLength) + currentPosition, new Vector3(halfLength,halfLength,halfLength)+currentPosition);
				Debug.DrawLine(new Vector3(-halfLength,halfLength,-halfLength) + currentPosition, new Vector3(halfLength,halfLength,-halfLength)+currentPosition);

				Debug.DrawLine(new Vector3(halfLength,-halfLength,-halfLength) + currentPosition, new Vector3(halfLength,-halfLength,halfLength)+currentPosition);
				Debug.DrawLine(new Vector3(halfLength,-halfLength,halfLength) + currentPosition, new Vector3(halfLength,halfLength,halfLength)+currentPosition);
				Debug.DrawLine(new Vector3(halfLength,halfLength,halfLength) + currentPosition, new Vector3(halfLength,halfLength,-halfLength)+currentPosition);
				Debug.DrawLine(new Vector3(halfLength,halfLength,-halfLength) + currentPosition, new Vector3(halfLength,-halfLength,-halfLength)+currentPosition);

				Debug.DrawLine(new Vector3(-halfLength,-halfLength,-halfLength) + currentPosition, new Vector3(halfLength,halfLength,halfLength)+currentPosition);
				Debug.DrawLine(new Vector3(-halfLength,-halfLength,halfLength) + currentPosition, new Vector3(halfLength,halfLength,-halfLength)+currentPosition);
				Debug.DrawLine(new Vector3(-halfLength,halfLength,-halfLength) + currentPosition, new Vector3(halfLength,-halfLength,halfLength)+currentPosition);
				Debug.DrawLine(new Vector3(-halfLength,halfLength,halfLength) + currentPosition, new Vector3(halfLength,-halfLength,-halfLength)+currentPosition);


				for(i=0;i<contents.Count;i++)
				{
					Debug.DrawRay(contentPositions[i],Vector3.up*9,Color.blue);
				}
			}
		} 
		else 
		{
			Debug.Log("Encompassing " + encompassCount);
			for (i=0;i<8;i++)
			{
				child[i].DebugShow();
			}
		}
	}

}
