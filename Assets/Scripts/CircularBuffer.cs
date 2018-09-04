using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularBuffer<T>
{
    public List<T> buffer;

    int indexCounter = 0;

    public CircularBuffer(int defaultSize = 0)
    {
        buffer = new List<T>(defaultSize);
        for (int i = 0; i < defaultSize; i++)
        {
            buffer.Add(default(T));
        }
    }

    public bool empty()
    {
        return buffer.Count == 0;
    }

    public T front()
    {
        return buffer[0];
    }

    public T this[int index]
    {
        get
        {
            if (buffer.Count == 0)
            {
                return default(T);
            }
            int i = index % buffer.Count;
            return buffer[i];
        }
        set
        {
            int i = index % buffer.Count;
            buffer[i] = value;
        }
    }

    internal T pop()
    {
        T obj = front();
        buffer.RemoveAt(0);
        return obj;
    }

    //this is a post increment
    public void incrementBuffer()
    {
        indexCounter++;
    }
    public T AccessAndIncrement()
    {
        return this[indexCounter++];
    }
    public void SetAndIncrement(T value)
    {
        this[indexCounter++] = value;
    }

    public void Clear()
    {
        for (int i = buffer.Count - 1; i >= 0 ; i--)
        {
            GameObject.DestroyImmediate(buffer[i] as GameObject);
        }
    }

    public void Add(T item)
    {
        buffer.Add(item);
    }


    public int Index()
    {
        return indexCounter;
    }

    public List<T>.Enumerator GetEnumerator()
    {
        return buffer.GetEnumerator();
    }
}