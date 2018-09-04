using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum KEY
{
    C,
    Cs,
    D,
    Eb,
    E,
    F,
    Fs,
    G,
    Ab,
    A,
    Bb,
    B,
    MAX
}

public enum CIRCLE_OF_FIFTHS
{
    C,
    G,
    D,
    A,
    E,
    B,
    Fs,
    Cs,
    Ab,
    Eb,
    Bb,
    F
}

public class CircleOfFifths
{

    //determines the harmonic strength of one note compared to another
    public static float DetermineHarmonicStrength()
    {
        return 0;
    }
}
public class Note
{
    public int midiNum;
    public float frequencyIntensity;
    public float time = 0;

    public Note(int _midiNum, float _frequencyIntensity)
    {
        midiNum = _midiNum;
        frequencyIntensity = _frequencyIntensity;
        time = Time.time;
    }
}

public class Tempo
{
    float BPM;
    public Tempo(float _BPM)
    {
        BPM = _BPM;
    }
}



public class SongAnalyzer : SmartSingleton
{


    private SongData trackedSongData;

    public KEY currentKey;
    // Use this for initialization
    void Start()
    {
        trackedSongData = pm.Get<SongData>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void AddNoteToSong(int midiNum, float frequency)
    {
        trackedSongData.AddNote(new Note(midiNum, frequency));
        DisplayData();
    }
    private void DisplayData()
    {
        currentKey = trackedSongData.key;
    }
}
