using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DodecTree<T> : ThreadedJob
{
    public Vector3[] InData;
    public Vector3[] OutData;
    private DodecTreeNode<T> root;
    private List<T> OutlierItems;
    private List<Vector3> OutlierPositions;
    private int maxOutliers;
    //private byte[,,]  map;

    //	public void SetMap(byte[] theMap)
    //	{
    //		map = theMap;
    //	}

    public DodecTree(Vector3 pos, float length)
    {
        maxOutliers = 6;
        root = new DodecTreeNode<T>(pos, length, null);
        OutlierItems = new List<T>();
        OutlierPositions = new List<Vector3>();
    }

    public bool Add(Vector3 position, T item)
    {
        if (root.Contains(position))
        {
            root.Add(position, item);
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
        root.DebugShow();
        for (int i = 0; i < OutlierItems.Count; i++)
        {
            Debug.DrawRay(OutlierPositions[i], Vector3.up + Vector3.right, Color.green);
        }
    }

    //	public bool Update(T item)
    //	{
    //		return true;
    //	}

    protected override void ThreadFunction1()
    {
        for (int i = 0; i < 1000000; i++)
        {
            InData[i % InData.Length] += InData[(i % 1) % InData.Length];
        }
    }

    protected override void OnFinished()
    {
        for (int i = 0; i < InData.Length; i++)
        {
            Debug.Log("Results(" + i + "): " + InData[i]);
        }
    }

    public void Remove(Vector3 pos, T item)
    {
        int i;

        if (root.Contains(pos))
        {
            root.Remove(pos, item);  // (pos,item)
        }
        else
        {
            for (i = 0; i < OutlierItems.Count; i++)
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

public class DodecTreeNode<T>
{
    public int minThreshold = 2;
    public int maxThreshold = 20;
    public bool isLeaf = false;
    public DodecTreeNode<T> parentNode;
    public int encompassCount;

    public int maxChildCount = 13;
    public List<T> contents;
    public List<Vector3> contentPositions;

    public DodecTreeNode<T>[] child;
    public Vector3 currentPosition;
    public float length;

    // Old Remove function
    //	public void Remove(Vector3 pos)
    //	{
    //		int i;
    //		DodecTreeNode<T> tempNode = Find (pos);
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
        int i, j;
        Vector3 temp;
        bool success = false;

        if (Contains(position))
        {
            if (isLeaf)
            {
                for (i = 0; i < contentPositions.Count; i++)
                {
                    if (contentPositions[i] == position)
                    {
                        contentPositions.RemoveAt(i);
                        contents.RemoveAt(i);
                    }
                }

                Debug.Log("Removing " + position.ToString() + " from " + currentPosition.ToString());
            }

            else
            {
               
                //Debug.Log("Going into child " + j);
                for (j = 0; j < child.Length; j++)
                {
                    if (child[j].Contains(position))
                    {
                        success = child[j].Remove(position, item);
                        break;
                    }
                }

            }
            if ((success) && (!isLeaf))
            {
                encompassCount--;
                if (encompassCount < maxThreshold)
                {
                    DodecTreeNode<T> tempNode;
                    // Collapse Node
                    for (i = 0; i < maxChildCount; i++)
                    {
                        tempNode = child[i];
                        for (j = 0; j < tempNode.contents.Count; j++)
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
    public DodecTreeNode<T> GetParent()
    {
        return parentNode;
    }
    public DodecTreeNode<T> Find(Vector3 pos)
    {
        Vector3 temp;
        int j;

        if (isLeaf)
        {
            for (j = 0; j < contentPositions.Count; j++)
            {
                if (contentPositions[j] == pos)
                {
                    return this;
                }
            }
        }
        else
        {
            
            for (j = 0; j < child.Length; j++)
            {
                if (child[j].Contains(pos))
                {
                    return child[j].Find(pos);

                }
            }
        }

        return null;
    }
    public DodecTreeNode<T> UpRoot(ref List<T> outlierItems, ref List<Vector3> outlierPos)
    {
        Vector3 temp;
        DodecTreeNode<T> tempNode;
        DodecTreeNode<T> holderNode;
        float halfLength = length / 2.0f;

        int i, j=0;
        float dist = (outlierPos[0] - currentPosition).magnitude;

        //for(i=1;i<child.Length;i++)
        //{
        //    temp = outlierPos[0] - currentPosition;
        //    if (temp.magnitude < dist)
        //    {
        //        j = i;
        //    }
        //}

        switch(j)
        {
            default:
                temp = currentPosition ;
                tempNode = new DodecTreeNode<T>(temp, length * 2, null);
                tempNode.BreakUp();
                tempNode.child[0] = this;
                break;
            case 1:
                temp = currentPosition + new Vector3(-halfLength, -halfLength, 0);
                tempNode = new DodecTreeNode<T>(temp, length * 2, null);
                tempNode.BreakUp();
                tempNode.child[1] = this;
                break;
            case 2:
                temp = currentPosition + new Vector3(0, -halfLength, -halfLength);
                tempNode = new DodecTreeNode<T>(temp, length * 2, null);
                tempNode.BreakUp();
                tempNode.child[2] = this;
                break;
            case 3:
                temp = currentPosition + new Vector3(halfLength, -halfLength, 0);
                tempNode = new DodecTreeNode<T>(temp, length * 2, null);
                tempNode.BreakUp();
                tempNode.child[3] = this;
                break;
            case 4:
                temp = currentPosition + new Vector3(0, -halfLength, halfLength);
                tempNode = new DodecTreeNode<T>(temp, length * 2, null);
                tempNode.BreakUp();
                tempNode.child[4] = this;
                break;
            case 5:
                temp = currentPosition + new Vector3(-halfLength, 0, -halfLength);
                tempNode = new DodecTreeNode<T>(temp, length * 2, null);
                tempNode.BreakUp();
                tempNode.child[5] = this;
                break;
            case 6:
                temp = currentPosition + new Vector3(halfLength, 0, -halfLength);
                tempNode = new DodecTreeNode<T>(temp, length * 2, null);
                tempNode.BreakUp();
                tempNode.child[6] = this;
                break;
            case 7:
                temp = currentPosition + new Vector3(halfLength, 0, -halfLength);
                tempNode = new DodecTreeNode<T>(temp, length * 2, null);
                tempNode.BreakUp();
                tempNode.child[7] = this;
                break;
            case 8:
                temp = currentPosition + new Vector3(-halfLength, 0, halfLength);
                tempNode = new DodecTreeNode<T>(temp, length * 2, null);
                tempNode.BreakUp();
                tempNode.child[8] = this;
                break;
            case 9:
                temp = currentPosition + new Vector3(-halfLength, halfLength, 0);
                tempNode = new DodecTreeNode<T>(temp, length * 2, null);
                tempNode.BreakUp();
                tempNode.child[9] = this;
                break;
            case 10:
                temp = currentPosition + new Vector3(0, halfLength, -halfLength);
                tempNode = new DodecTreeNode<T>(temp, length * 2, null);
                tempNode.BreakUp();
                tempNode.child[10] = this;
                break;
            case 11:
                temp = currentPosition + new Vector3(halfLength, halfLength, 0);
                tempNode = new DodecTreeNode<T>(temp, length * 2, null);
                tempNode.BreakUp();
                tempNode.child[11] = this;
                break;
            case 12:
                temp = currentPosition + new Vector3(0, halfLength, halfLength);
                tempNode = new DodecTreeNode<T>(temp, length * 2, null);
                tempNode.BreakUp();
                tempNode.child[12] = this;
                break;
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
        child = new DodecTreeNode<T>[maxChildCount];

        float halfX = length / 2.0f;
        float halfY = length / 2.0f;
        float halfZ = length / 2.0f;

        //Center
        temp = new Vector3(0, 0, 0);
        temp = temp + currentPosition;
        child[0] = new DodecTreeNode<T>(temp, halfX, this);
        /*   child[1] = new DodecTreeNode<T>(temp, halfX, this);
           child[2] = new DodecTreeNode<T>(temp, halfX, this);
           child[3] = new DodecTreeNode<T>(temp, halfX, this);
           child[4] = new DodecTreeNode<T>(temp, halfX, this);
           child[5] = new DodecTreeNode<T>(temp, halfX, this);
           child[6] = new DodecTreeNode<T>(temp, halfX, this);
           child[7] = new DodecTreeNode<T>(temp, halfX, this);
           child[8] = new DodecTreeNode<T>(temp, halfX, this);
           child[9] = new DodecTreeNode<T>(temp, halfX, this);
           child[10] = new DodecTreeNode<T>(temp, halfX, this);
           child[11] = new DodecTreeNode<T>(temp, halfX, this);
           child[12] = new DodecTreeNode<T>(temp, halfX, this);   */

        Debug.Log("Created " + temp.ToString());

        //Top set
        temp = new Vector3(halfX, halfY, 0);
        temp = temp + currentPosition;
        child[1] = new DodecTreeNode<T>(temp, halfX, this);
        Debug.Log("Created " + temp.ToString());

        temp = new Vector3(0, halfY, halfZ);
        temp = temp + currentPosition;
        child[2] = new DodecTreeNode<T>(temp, halfX, this);
        Debug.Log("Created " + temp.ToString());

        temp = new Vector3(-halfX, halfY, 0);
        temp = temp + currentPosition;
        child[3] = new DodecTreeNode<T>(temp, halfX, this);
        Debug.Log("Created " + temp.ToString());

        temp = new Vector3(0, halfY, -halfZ);
        temp = temp + currentPosition;
        child[4] = new DodecTreeNode<T>(temp, halfX, this);
        Debug.Log("Created " + temp.ToString());
        //*/
        // Middle Set
        temp = new Vector3(halfX, 0, halfZ);
        temp = temp + currentPosition;
        child[5] = new DodecTreeNode<T>(temp, halfX, this);
        Debug.Log("Created " + temp.ToString());

        temp = new Vector3(-halfX, 0, halfZ);
        temp = temp + currentPosition;
        child[6] = new DodecTreeNode<T>(temp, halfX, this);
        Debug.Log("Created " + temp.ToString());

        temp = new Vector3(-halfX, 0, -halfZ);
        temp = temp + currentPosition;
        child[7] = new DodecTreeNode<T>(temp, halfX, this);
        Debug.Log("Created " + temp.ToString());

        temp = new Vector3(halfX, 0, -halfZ);
        temp = temp + currentPosition;
        child[8] = new DodecTreeNode<T>(temp, halfX, this);
        Debug.Log("Created " + temp.ToString());
        //*/
        // Bottom Set
        temp = new Vector3(halfX, -halfY, 0);
        temp = temp + currentPosition;
        child[9] = new DodecTreeNode<T>(temp, halfX, this);
        Debug.Log("Created " + temp.ToString());

        temp = new Vector3(0, -halfY, halfZ);
        temp = temp + currentPosition;
        child[10] = new DodecTreeNode<T>(temp, halfX, this);
        Debug.Log("Created " + temp.ToString());

        temp = new Vector3(-halfX, -halfY, 0);
        temp = temp + currentPosition;
        child[11] = new DodecTreeNode<T>(temp, halfX, this);
        Debug.Log("Created " + temp.ToString());

        temp = new Vector3(0, -halfY, -halfZ);
        temp = temp + currentPosition;
        child[12] = new DodecTreeNode<T>(temp, halfX, this);
        Debug.Log("Created " + temp.ToString());
        //*/

        isLeaf = false;
    }

    public bool Add(Vector3 position, T item)
    {
        int i, j;
        Vector3 temp;

        if (Contains(position))
        {
            encompassCount++;
            if (isLeaf)
            {
                if (contents.Count < maxThreshold)
                {
                    contents.Add(item);
                    contentPositions.Add(position);
                    Debug.Log("Adding " + position.ToString() + " to " + currentPosition.ToString());
                }
                else
                {
                    contents.Add(item);
                    contentPositions.Add(position);
                    BreakUp();

                    for (i = 0; i < contents.Count; i++)
                    {
                        for (j = 0; j < child.Length; j++)
                        {
                            if (child[j].Contains(contentPositions[j]))
                            {
                                child[j].Add(contentPositions[i], contents[i]);
                                break;
                            }
                        }
                        /* temp = contentPositions[i] - currentPosition;
                         if (temp.x < 0)
                         {          //-
                             if (temp.y < 0)
                             {     //--
                                 if (temp.z < 0)
                                 {  //---
                                     j = 7;
                                 }
                                 else
                                 {            //--+
                                     j = 6;
                                 }
                             }
                             else
                             {                //-+
                                 if (temp.z < 0)
                                 { //-+-
                                     j = 5;
                                 }
                                 else
                                 {            //-++
                                     j = 4;
                                 }
                             }
                         }
                         else
                         {  //+
                             if (temp.y < 0)
                             {     //+-
                                 if (temp.z < 0)
                                 {  //+--
                                     j = 3;
                                 }
                                 else
                                 {            //+-+
                                     j = 2;
                                 }
                             }
                             else
                             {                //++
                                 if (temp.z < 0)
                                 { //++-
                                     j = 1;
                                 }
                                 else
                                 {            //+++
                                     j = 0;
                                 }
                             }
                         }
                         */
                        Debug.Log("Child " + j);
                        //child[j].Add(contentPositions[i], contents[i]);
                    }
                    contents.Clear();
                    contentPositions.Clear();
                    isLeaf = false;

                }
            }
            else
            {
                /*   temp = position - currentPosition;
                   if (temp.x < 0)
                   {          //-
                       if (temp.y < 0)
                       {     //--
                           if (temp.z < 0)
                           {  //---
                               j = 7;
                           }
                           else
                           {            //--+
                               j = 6;
                           }
                       }
                       else
                       {                //-+
                           if (temp.z < 0)
                           { //-+-
                               j = 5;
                           }
                           else
                           {            //-++
                               j = 4;
                           }
                       }
                   }
                   else
                   {  //+
                       if (temp.y < 0)
                       {     //+-
                           if (temp.z < 0)
                           {  //+--
                               j = 3;
                           }
                           else
                           {            //+-+
                               j = 2;
                           }
                       }
                       else
                       {                //++
                           if (temp.z < 0)
                           { //++-
                               j = 1;
                           }
                           else
                           {            //+++
                               j = 0;
                           }
                       }
                   }
                   */
                // Debug.Log("Going into child " + j);
                // return child[j].Add(position, item);
                for (j = 0; j < child.Length; j++)
                {
                    if (child[j].Contains(position))
                    {
                        return child[j].Add(position, item);
                    }
                }
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
        Vector3 distVector;


        float dist;

        distVector = pos - currentPosition;
        dist = distVector.magnitude;

        if (dist < length)
        {
            return true;
        }

        return false;
    }

    public void SetPosition(Vector3 pos)
    {
        currentPosition = pos;
    }

    public DodecTreeNode(Vector3 pos, float width, DodecTreeNode<T> parent)
    {
        contents = new List<T>();
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
        float halfLength = length / 2.0f;
        Color c = Color.HSVToRGB(255 * (1 / length), 255 * (1 / length), 255 * (1 / length));
        // c = Color.white;

        if (isLeaf)
        {
            //if (contents.Count > 0)
            // if (length > 516)
            {
                //Top set
                Debug.DrawLine(new Vector3(0, length, 0) + currentPosition, new Vector3(halfLength, halfLength, -halfLength) + currentPosition, c);
                Debug.DrawLine(new Vector3(halfLength, halfLength, -halfLength) + currentPosition, new Vector3(length, 0, 0) + currentPosition, c);
                Debug.DrawLine(new Vector3(length, 0, 0) + currentPosition, new Vector3(halfLength, halfLength, halfLength) + currentPosition, c);
                Debug.DrawLine(new Vector3(halfLength, halfLength, halfLength) + currentPosition, new Vector3(0, length, 0) + currentPosition, c);

                Debug.DrawLine(new Vector3(0, length, 0) + currentPosition, new Vector3(halfLength, halfLength, halfLength) + currentPosition, c);
                Debug.DrawLine(new Vector3(halfLength, halfLength, halfLength) + currentPosition, new Vector3(0, 0, length) + currentPosition, c);
                Debug.DrawLine(new Vector3(0, 0, length) + currentPosition, new Vector3(-halfLength, halfLength, halfLength) + currentPosition, c);
                Debug.DrawLine(new Vector3(-halfLength, halfLength, halfLength) + currentPosition, new Vector3(0, length, 0) + currentPosition, c);

                Debug.DrawLine(new Vector3(0, length, 0) + currentPosition, new Vector3(-halfLength, halfLength, halfLength) + currentPosition, c);
                Debug.DrawLine(new Vector3(-halfLength, halfLength, halfLength) + currentPosition, new Vector3(-length, 0, 0) + currentPosition, c);
                Debug.DrawLine(new Vector3(-length, 0, 0) + currentPosition, new Vector3(-halfLength, halfLength, -halfLength) + currentPosition, c);
                Debug.DrawLine(new Vector3(-halfLength, halfLength, -halfLength) + currentPosition, new Vector3(0, length, 0) + currentPosition, c);

                Debug.DrawLine(new Vector3(0, length, 0) + currentPosition, new Vector3(-halfLength, halfLength, -halfLength) + currentPosition, c);
                Debug.DrawLine(new Vector3(-halfLength, halfLength, -halfLength) + currentPosition, new Vector3(0, 0, -length) + currentPosition, c);
                Debug.DrawLine(new Vector3(0, 0, -length) + currentPosition, new Vector3(halfLength, halfLength, -halfLength) + currentPosition, c);
                Debug.DrawLine(new Vector3(halfLength, halfLength, -halfLength) + currentPosition, new Vector3(0, length, 0) + currentPosition, c);

                //Bottom set
                Debug.DrawLine(new Vector3(0, -length, 0) + currentPosition, new Vector3(halfLength, -halfLength, -halfLength) + currentPosition, c);
                Debug.DrawLine(new Vector3(halfLength, -halfLength, -halfLength) + currentPosition, new Vector3(length, 0, 0) + currentPosition, c);
                Debug.DrawLine(new Vector3(length, 0, 0) + currentPosition, new Vector3(halfLength, -halfLength, halfLength) + currentPosition, c);
                Debug.DrawLine(new Vector3(halfLength, -halfLength, halfLength) + currentPosition, new Vector3(0, -length, 0) + currentPosition, c);

                Debug.DrawLine(new Vector3(0, -length, 0) + currentPosition, new Vector3(halfLength, -halfLength, halfLength) + currentPosition, c);
                Debug.DrawLine(new Vector3(halfLength, -halfLength, halfLength) + currentPosition, new Vector3(0, 0, length) + currentPosition, c);
                Debug.DrawLine(new Vector3(0, 0, length) + currentPosition, new Vector3(-halfLength, -halfLength, halfLength) + currentPosition, c);
                Debug.DrawLine(new Vector3(-halfLength, -halfLength, halfLength) + currentPosition, new Vector3(0, -length, 0) + currentPosition, c);

                Debug.DrawLine(new Vector3(0, -length, 0) + currentPosition, new Vector3(-halfLength, -halfLength, halfLength) + currentPosition, c);
                Debug.DrawLine(new Vector3(-halfLength, -halfLength, halfLength) + currentPosition, new Vector3(-length, 0, 0) + currentPosition, c);
                Debug.DrawLine(new Vector3(-length, 0, 0) + currentPosition, new Vector3(-halfLength, -halfLength, -halfLength) + currentPosition, c);
                Debug.DrawLine(new Vector3(-halfLength, -halfLength, -halfLength) + currentPosition, new Vector3(0, -length, 0) + currentPosition, c);

                Debug.DrawLine(new Vector3(0, -length, 0) + currentPosition, new Vector3(-halfLength, -halfLength, -halfLength) + currentPosition, c);
                Debug.DrawLine(new Vector3(-halfLength, -halfLength, -halfLength) + currentPosition, new Vector3(0, 0, -length) + currentPosition, c);
                Debug.DrawLine(new Vector3(0, 0, -length) + currentPosition, new Vector3(halfLength, -halfLength, -halfLength) + currentPosition, c);
                Debug.DrawLine(new Vector3(halfLength, -halfLength, -halfLength) + currentPosition, new Vector3(0, -length, 0) + currentPosition, c);

                for (i = 0; i < contents.Count; i++)
                {
                    Debug.DrawRay(contentPositions[i], Vector3.up * 9, Color.blue);
                }


            }
        }
        else
        {
            Debug.Log("Encompassing " + encompassCount);

            // if (!isLeaf)
            for (i = 0; i < maxChildCount; i++)
            {
                child[i].DebugShow();
            }
        }//*/
        
    }
}


