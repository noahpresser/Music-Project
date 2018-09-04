using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class SortablePair : System.IComparable
{
    public int note;
    public int count;

    public SortablePair(int key, int value)
    {
        note = key;
        count = value;
    }

    public int CompareTo(int other)
    {
        return count - other;
    }

    public int CompareTo(object obj)
    {
        SortablePair pair = (SortablePair)(obj);
        return CompareTo(pair.count);
    }
}

public class HueHelper : SmartBehaviour
{

    public int numNotes = 30;
    CircularBuffer<int> lastXNotes;
    private int circularIndex = 0;

    public List<Color> colorsOfNotes;
    private CircularBuffer<Color> notesAsCircle;
    public List<Color> colorsInOrder = new List<Color>();
    ColorSpheres myColorSphere;
    
    PriorityQueue<SortablePair> noteCounts = new PriorityQueue<SortablePair>();
    Dictionary<int, SortablePair> noteDict = new Dictionary<int, SortablePair>();
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < 12; i++)
        {
            colorsInOrder.Add(Color.red);
        }
        for (int i = 0; i < colorsOfNotes.Count; i++)
        {
            Color c = colorsOfNotes[i];
            if (i % 2 == 1)
            {
                colorsInOrder[(i + 6) % 12] = c;
            }
            else
            {
                colorsInOrder[i] = c;
            }
        }
        lastXNotes = new CircularBuffer<int>(numNotes);
        for (int i = 0; i < numNotes; i++)
        {
            lastXNotes[i] = -1;
        }

        for (int i = -1; i < 12; i++)
        {
            SortablePair s = new SortablePair(i, 0);
            noteDict[i] = s;
        }
        //myColorSphere = GetComponent<ColorSpheres>();
        //myColorSphere.SpawnCircle();    
    }

    public string testNum;
    public bool input;
    private void Update()
    {
        if (input)
        {
            for (int i = 0; i < 100; i++)
            {
                //UpdateNote(UnityEngine.Random.Range(0, 12));
            }
            input = false;
        }
    }
    public void ClearNotes()
    {
        foreach (var item in noteDict)
        {
            item.Value.count = 0;
        }
    }


    public void UpdateNote(int newNote)
    {
        //decrement the count of the note to be removed
        DecrementCount(lastXNotes[circularIndex]);
        lastXNotes[circularIndex] = newNote;
        IncrementCount(newNote);
        circularIndex++;

        noteCounts.Clear();
        foreach (var item in noteDict)
        {
            if (item.Key == -1)
            {
                continue;
            }
            noteCounts.Enqueue(item.Value);
        }

        string str = "Sorted :\n";
        Dictionary<int, int> rankings = new Dictionary<int, int>();
        int index = noteCounts.Count - 1;
        while (noteCounts.Count > 0)
        {
            SortablePair s = noteCounts.Dequeue();
            rankings[s.note] = index--;
            str += "Note: " + s.note + "Rank: " + rankings[s.note] + " Count: " + s.count + " ||||| ";
        }

        //Debug.Log(str);
        //myColorSphere.GrowCircle(noteDict, numNotes);
    }

    private void IncrementCount(int i)
    {
        noteDict[i].count++;
    }

    private void DecrementCount(int i)
    {
        noteDict[i].count--;
    }

}
