using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Data used for further statistical analysis
public class RelevanceData
{
    public int midiNum;
    public float frequency;                         //the value at which the note became relevant
    public float relevanceTime;                     //the time the note became relevant
    public RelevanceData(Note note)
    {
        midiNum = note.midiNum;
        frequency = note.frequencyIntensity;
        relevanceTime = Time.time;
    }
}

//Note data used for further statistical analysis
public class NoteData
{
    public int midiNum;
    public float time;
    public float frequency;
    public float stDevTotal;                            //the standard deviation relative to all notes in the song at the time this note was played
    public float stDevModified;                         //the standard deviation relative to all notes NOT ABOVE THE BASE THRESHOLD in the song at the time this note was played

    public NoteData(int _midiNum, float _frequency)
    {
        midiNum = _midiNum;
        frequency = _frequency;
        stDevTotal = 0;
        stDevModified = 0;
        time = Time.time;
    }

    public void SetStandardDeviation(float _stDev)
    {
        stDevTotal = _stDev;
    }

    public void SetModifiedStandardDeviation(float _stDev)
    {
        stDevModified = _stDev;
    }
}

public class SongData : SmartBehaviour
{
    public Tempo tempo;
    public KEY key;

    private float songStartTime;

    List<Note> notesInSong = new List<Note>();
    List<RelevanceData> relevanceData = new List<RelevanceData>();              //data about all "relevant notes" in song


    List<NoteData> currentFrameNoteData = new List<NoteData>();

    PriorityQueue<SortablePair> noteCounts = new PriorityQueue<SortablePair>();
    Dictionary<int, SortablePair> noteDict = new Dictionary<int, SortablePair>();

    public float stDevThreshold = 2;

    void Start()
    {
        for (int i = -1; i < 12; i++)
        {
            SortablePair s = new SortablePair(i, 0);
            noteDict[i] = s;
        }
    }

    public void Analyze()
    {
        DetermineRelevantNotes();
        pm.Get<NoteVisualizers>().UpdateVisualizers();

        currentFrameNoteData.Clear();
    }

    private void SetThresholdVisuals()
    {

    }

    private void DetermineRelevantNotes(int minMidiNote = 24, int MaxMidiNote = 88)
    {
        float meanFrequencyThisFrame = 0;
        int numNotes = MaxMidiNote - minMidiNote;
        int countedNumNotes = 0;

        List<float> frequenciesThisFrame = new List<float>();
        foreach (NoteData noteData in currentFrameNoteData)
        {
            if ((noteData.midiNum < minMidiNote) || (noteData.midiNum > MaxMidiNote))
                continue;

            meanFrequencyThisFrame += noteData.frequency;
            countedNumNotes++;
            frequenciesThisFrame.Add(noteData.frequency);
        }
        meanFrequencyThisFrame /= numNotes;

        float standardDeviation = getStandardDeviation(frequenciesThisFrame, meanFrequencyThisFrame);

        pm.Get<Equalizer>().SetThresholdVisual("mean", meanFrequencyThisFrame);

        pm.Get<Equalizer>().SetThresholdVisual("stDev1", meanFrequencyThisFrame + standardDeviation);
        pm.Get<Equalizer>().SetThresholdVisual("stDev2", meanFrequencyThisFrame + 2 * standardDeviation);
        pm.Get<Equalizer>().SetThresholdVisual("stDev3", meanFrequencyThisFrame + 3 * standardDeviation);

        if (countedNumNotes != numNotes)
        {
            Debug.LogError("COUNTED NUM NOTES DOES NOT MATCH ACTUAL NUM NOTES, counted: " + countedNumNotes + "num: " + numNotes);
        }

        foreach (NoteData noteData in currentFrameNoteData)
        {
            float noteDifferenceFromMean = noteData.frequency - meanFrequencyThisFrame;
            noteData.SetStandardDeviation(noteDifferenceFromMean / standardDeviation);


            if (noteData.stDevTotal > (standardDeviation * stDevThreshold))
            {
                //do something bleh relevantnotes
                pm.Get<NoteVisualizers>().AddNoteFrequency(noteData.midiNum, noteData.stDevTotal);
            }
        }

    }

    private float getStandardDeviation(List<float> floatList, float average)
    {
        float sumOfDerivation = 0;
        foreach (float value in floatList)
        {
            sumOfDerivation += (value) * (value);
        }
        float sumOfDerivationAverage = sumOfDerivation / (floatList.Count - 1);
        return (float)Math.Sqrt(sumOfDerivationAverage - (average * average));
    }

    public void AddNote(Note newNote)
    {
        if (currentFrameNoteData == null)
            currentFrameNoteData = new List<NoteData>();

        NoteData newNoteData = new NoteData(newNote.midiNum, newNote.frequencyIntensity);
        currentFrameNoteData.Add(newNoteData);





        //if (notesInSong == null)
        //{
        //    notesInSong = new List<Note>();
        //    noteDict = new Dictionary<int, SortablePair>();
        //}
        //if (notesInSong.Count == 0)
        //{
        //    songStartTime = Time.time;
        //}
        //notesInSong.Add(newNote);

        //noteDict[(newNote.midiNum + 120) % 12].count++;

        //Analyze();

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
