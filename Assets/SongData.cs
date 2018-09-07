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
    public RelevanceData(NoteData note)
    {
        midiNum = note.midiNum;
        frequency = note.frequency;
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
    public class DataPortion
    {
        public float previousFrameMeanFrequency = 0;
        public float currentFrameMeanFrequency = 0;
        public float stDevThisFrame = 0;
        public List<RelevanceData> relevanceData = new List<RelevanceData>();              //data about all "relevant notes" in song
        public Dictionary<int, NoteData> previousFrameNoteData = new Dictionary<int, NoteData>();
        public Dictionary<int, NoteData> currentFrameNoteData = new Dictionary<int, NoteData>();
        public PriorityQueue<SortablePair> noteCounts = new PriorityQueue<SortablePair>();
        public Dictionary<int, SortablePair> noteDict = new Dictionary<int, SortablePair>();
    }

    public Tempo tempo;
    public KEY key;

    private float songStartTime;
    List<Note> notesInSong = new List<Note>();

    Dictionary<string, DataPortion> songData = new Dictionary<string, DataPortion>();


    public float stDevThreshold = 2;
    private List<NoteVisualizers> allNoteVisualizers = new List<NoteVisualizers>();

    void Start()
    {
        foreach (var item in GameObject.FindObjectsOfType<NoteVisualizers>())
        {
            if (item.gameObject.activeInHierarchy)
            {
                allNoteVisualizers.Add(item);
            }
        }
        foreach (var item in allNoteVisualizers)
        {
            songData[item.name] = new DataPortion();
            for (int i = -1; i < 12; i++)
            {
                SortablePair s = new SortablePair(i, 0);
                songData[item.name].noteDict[i] = s;
            }
        }
    }

    public void Analyze()
    {
        foreach (NoteVisualizers noteVisual in allNoteVisualizers)
        {
            DetermineRelevantNotes(noteVisual, noteVisual.minNote, noteVisual.maxNote);
            noteVisual.UpdateVisualizers();
        }
        ResetFrame();
    }
    public void ResetFrame()
    {
        //deep copy
        foreach (NoteVisualizers noteVisual in allNoteVisualizers)
        {
            DataPortion dp = songData[noteVisual.name];
            foreach (var item in dp.currentFrameNoteData)
            {
                dp.previousFrameNoteData[item.Key] = item.Value;
            }
            dp.currentFrameNoteData.Clear();
            dp.previousFrameMeanFrequency = dp.currentFrameMeanFrequency;
            dp.currentFrameMeanFrequency = 0;
        }

    }

    private void SetThresholdVisuals()
    {

    }

    private void DetermineRelevantNotes(NoteVisualizers noteVisualizer, int minMidiNote = 24, int MaxMidiNote = 88)
    {
        DataPortion dp = songData[noteVisualizer.name];
        int numNotes = MaxMidiNote - minMidiNote + 1;
        int countedNumNotes = 0;

        List<float> frequenciesThisFrame = new List<float>();
        foreach (var noteDataPair in dp.currentFrameNoteData)
        {
            NoteData noteData = noteDataPair.Value;
            if ((noteData.midiNum < minMidiNote) || (noteData.midiNum > MaxMidiNote))
                continue;

            dp.currentFrameMeanFrequency += noteData.frequency;
            countedNumNotes++;
            frequenciesThisFrame.Add(noteData.frequency);
        }
        dp.currentFrameMeanFrequency /= numNotes;

        dp.stDevThisFrame = getStandardDeviation(frequenciesThisFrame, dp.currentFrameMeanFrequency);

        pm.Get<Equalizer>().SetThresholdVisual("mean", dp.currentFrameMeanFrequency);

        pm.Get<Equalizer>().SetThresholdVisual("stDev1", dp.currentFrameMeanFrequency + dp.stDevThisFrame);
        pm.Get<Equalizer>().SetThresholdVisual("stDev2", dp.currentFrameMeanFrequency + 2 * dp.stDevThisFrame);
        pm.Get<Equalizer>().SetThresholdVisual("stDev3", dp.currentFrameMeanFrequency + 3 * dp.stDevThisFrame);

        if (countedNumNotes != numNotes)
        {
            Debug.LogError("COUNTED NUM NOTES DOES NOT MATCH ACTUAL NUM NOTES, counted: " + countedNumNotes + "num: " + numNotes);
        }

        foreach (var noteDataPair in dp.currentFrameNoteData)
        {
            if ((noteDataPair.Key < minMidiNote) || (noteDataPair.Key > MaxMidiNote))
                continue;

            NoteData noteData = noteDataPair.Value;
            float noteDifferenceFromMean = noteData.frequency - dp.currentFrameMeanFrequency;
            noteData.SetStandardDeviation(noteDifferenceFromMean / dp.stDevThisFrame);


            if (IsRelevant(noteData, dp, dp.currentFrameMeanFrequency))
            {
                //do something bleh relevantnotes
                noteVisualizer.AddNoteFrequency(noteData.midiNum, noteData.stDevTotal);
                AddRelevanceData(noteVisualizer, noteDataPair);
            }
        }

    }

    private bool IsRelevant(NoteData noteData, DataPortion dp, float meanFrequency)
    {
        float stDevsAboveMean = ((noteData.frequency - meanFrequency) / dp.stDevThisFrame);
        return stDevsAboveMean > stDevThreshold;

    }
    private void AddRelevanceData(NoteVisualizers noteVisual, KeyValuePair<int, NoteData> noteDataPair)
    {
        DataPortion dp = songData[noteVisual.name];
        //if note was not relevant last frame, add it to the list of relevance data for this frame
        if (dp.previousFrameNoteData.Count == dp.currentFrameNoteData.Count)
        {
            NoteData previousFrameNoteData = dp.previousFrameNoteData[noteDataPair.Key];
            if (!IsRelevant(previousFrameNoteData, dp, dp.previousFrameMeanFrequency))
            {
                RelevanceData r = new RelevanceData(noteDataPair.Value);
                Debug.Log("New relevant Note: " + noteVisual.name + " Note name: " + noteDataPair.Value.midiNum  + " | " + ((NOTE_NAME)((noteDataPair.Key + 120) % 12)) + " Time: " + Time.time);
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
        foreach (var noteVisualizer in allNoteVisualizers)
        {
            DataPortion dp = songData[noteVisualizer.name];
            if (dp.currentFrameNoteData == null)
                dp.currentFrameNoteData = new Dictionary<int, NoteData>();
            NoteData newNoteData = new NoteData(newNote.midiNum, newNote.frequencyIntensity);
            dp.currentFrameNoteData[newNoteData.midiNum] = newNoteData;

        }




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

        //noteCounts.Clear();
        //foreach (var item in noteDict)
        //{
        //    if (item.Key == -1)
        //    {
        //        continue;
        //    }
        //    noteCounts.Enqueue(item.Value);
        //}

        //string str = "Sorted :\n";
        //Dictionary<int, int> rankings = new Dictionary<int, int>();
        //List<NOTE_NAME> rankedNotes = new List<NOTE_NAME>();
        //int index = noteCounts.Count - 1;

        //SortablePair s = noteCounts.Peek();
        //while (noteCounts.Count > 0)
        //{
        //    s = noteCounts.Dequeue();
        //    rankedNotes.Insert(0, (NOTE_NAME)s.note);
        //    str += (NOTE_NAME)s.note + " " + s.count + " || ";
        //}
        ////Debug.Log(str);
        //return rankedNotes;
        throw new NotImplementedException();
    }
    private void DetermineBassAndTrebleNotes()
    {
        //todo
    }
}
