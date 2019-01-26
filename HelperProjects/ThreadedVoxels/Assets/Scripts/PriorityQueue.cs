using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class PriorityQueue<TValue>
{
    private List<KeyValuePair<float,TValue>> _queue;

    public PriorityQueue()
    {
        _queue = new List<KeyValuePair<float,TValue>>();
    }

	public int Count ()
	{
		return  _queue.Count;
	}

    public void Enqueue(float p, TValue v)
    {
        KeyValuePair<float, TValue> temp = new KeyValuePair<float, TValue>(p, v);
        _queue.Add(temp);

        int pos = _queue.Count - 1;

        while (pos > 0)
        {
            int parentPos = (pos - 1) / 2;
            if (_queue[pos].Key < _queue[parentPos].Key)
            {
                SwapElement(pos, parentPos);
                pos = parentPos;
            }
            else
                break;
        }

    }

    public void SwapElement(int p1, int p2)
    {
        KeyValuePair<float, TValue> temp = _queue[p1];
        _queue[p1] = _queue[p2];
        _queue[p2] = temp;
    }

    public TValue Deqeue()
    {
        if (IsEmpty())
        {
            return default(TValue);
        }

        KeyValuePair<float, TValue> temp = _queue[0];
        ReSort();

        return temp.Value;            
    }

    private void ReSort()
    {
        if (_queue.Count <= 1)
        {
            _queue.Clear();
            return;
        }
        //Take off top value and resort
        _queue[0] = _queue[_queue.Count - 1];
        _queue.RemoveAt(_queue.Count - 1);

        int pos = 0;

        while (true)
        {
            int smallest = pos;
            int left = pos * 2+1;
            int right = pos * 2 + 2;
            if ((left < _queue.Count) && (_queue[smallest].Key > _queue[left].Key))
            {
                smallest = left;
            }
            if ((right < _queue.Count) && (_queue[smallest].Key > _queue[right].Key))
            {
                smallest = right;
            }

            if (smallest != pos)
            {
                SwapElement(smallest, pos);
                pos = smallest;
            }
            else
            {
                break;
            }
        }
    }
    public bool IsEmpty()
    {
        if (_queue == null)
            return true;

        if (_queue.Count == 0)
            return true;

        return false;
    }
    
}

