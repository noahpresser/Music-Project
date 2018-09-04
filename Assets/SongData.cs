using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongData : SmartBehaviour
{
    public Tempo tempo;
    public KEY key;

    private float songStartTime;

    //todo make this a sortable dictionary with a priority queue
    List<Note> notesInSong = new List<Note>();

    PriorityQueue<SortablePair> noteCounts = new PriorityQueue<SortablePair>();
    Dictionary<int, SortablePair> noteDict = new Dictionary<int, SortablePair>();
    void Start()
    {
        for (int i = -1; i < 12; i++)
        {
            SortablePair s = new SortablePair(i, 0);
            noteDict[i] = s;
        }
    }

    void Analyze()
    {
        DetermineTempo();
        DetermineKey();
        pm.Get<NoteVisualizers>().UpdateVisualizers();
    }

    public void AddNote(Note newNote)
    {
        if (notesInSong == null)
        {
            notesInSong = new List<Note>();
            noteDict = new Dictionary<int, SortablePair>();
        }
        if (notesInSong.Count == 0)
        {
            songStartTime = Time.time;
        }
        notesInSong.Add(newNote);

        noteDict[(newNote.midiNum + 120) % 12].count++;

        Analyze();

    }

    private void StartSong()
    {
        songStartTime = Time.time;
    }
    private void DetermineTempo()
    {
        //loop through all powerful notes and determing the basic tempo

    }

    private void DetermineKey()
    {
        List<NOTE_NAME> mostCommonNotes = RankNotes();
        NOTE_NAME[] top7Notes = new NOTE_NAME[7];

        //go through top 7 and determine key
        for (int i = 0; i < 7; i++)
        {

        }
    }
    private List<NOTE_NAME> RankNotes()
    {
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
        List<NOTE_NAME> rankedNotes = new List<NOTE_NAME>();
        int index = noteCounts.Count - 1;

        SortablePair s = noteCounts.Peek();
        while (noteCounts.Count > 0)
        {
            s = noteCounts.Dequeue();
            rankedNotes.Insert(0, (NOTE_NAME)s.note);
            str += (NOTE_NAME)s.note + " " + s.count + " || ";
        }
        //Debug.Log(str);
        return rankedNotes;
    }

    private void DetermineBassAndTrebleNotes()
    {
        //todo
    }
}
