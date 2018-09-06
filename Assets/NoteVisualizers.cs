using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteVisualizers : SmartSingleton
{
    public GameObject noteVisualizerPrefab;
    private Vector3 noteVisualizerStartPos;
    private float noteVisualizerStartScale;
    public float noteVisualizerMaxScale;
    List<GameObject> noteVisualizers = new List<GameObject>();

    Dictionary<int, float> notesAndSummedSongFreqencies = new Dictionary<int, float>();
    Dictionary<int, float> midiNotesAndFrequenciesPure = new Dictionary<int, float>();


    float totalSummedFrequencies = 0;
    public bool oneFrameTrueAllFramesFalse = false;

    [Range(24, 88)]
    public int minNote = 24;

    [Range(24, 88)]
    public int maxNote = 88;
    void Start()
    {
        noteVisualizerStartPos = noteVisualizerPrefab.GetComponent<Transform>().position;
        Rect r = noteVisualizerPrefab.transform.GetChild(0).GetComponent<RectTransform>().rect;
        noteVisualizerStartScale = 0;
        noteVisualizerMaxScale = (r.height);
    }

    void SpawnNoteVisualizers()
    {
        RectTransform parent = GetComponent<RectTransform>();
        for (int i = 0; i < 12; i++)
        {
            GameObject go = Instantiate(noteVisualizerPrefab, parent);
            Vector2 spawnPos = go.GetComponent<RectTransform>().anchoredPosition;
            spawnPos.x += i * noteVisualizerPrefab.GetComponent<RectTransform>().rect.width;
            go.GetComponent<RectTransform>().anchoredPosition = spawnPos;
            go.name = ((NOTE_NAME)(i)).ToString();
            go.transform.GetChild(0).GetComponent<Image>().color = pm.Get<HueHelper>("HueHelperTreble").colorsInOrder[i];
            noteVisualizers.Add(go);

            notesAndSummedSongFreqencies[i] = 0;
        }
    }


    public void UpdateVisualizers()
    {
        if (oneFrameTrueAllFramesFalse)
            totalSummedFrequencies = 0;

        if (noteVisualizers.Count == 0)
        {
            SpawnNoteVisualizers();
        }

        foreach (var item in notesAndSummedSongFreqencies)
        {
            totalSummedFrequencies += item.Value;
        }
        foreach (var item in notesAndSummedSongFreqencies)
        {
            if (item.Key == -1)
            {
                continue;
            }
            Rect r = noteVisualizers[item.Key].transform.GetChild(0).GetComponent<RectTransform>().rect;
            r.height = Mathf.Lerp(noteVisualizerStartScale, noteVisualizerMaxScale, (float)item.Value / (float)totalSummedFrequencies);
            noteVisualizers[item.Key].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(r.width, r.height);
            noteVisualizers[item.Key].transform.GetChild(1).GetComponent<Text>().text = ((NOTE_NAME)(item.Key)).ToString();
        }

        if (oneFrameTrueAllFramesFalse)
        {
            for (int i = 0; i < 12; i++)
            {
                notesAndSummedSongFreqencies[i] = 0;
            }
        }
    }
    internal void AddNoteFrequencyMidi(int noteNum, float noteFrequency)
    {
        if (noteNum < 21)
            return;

        midiNotesAndFrequenciesPure.Add(noteNum, noteFrequency);

    }

    internal void AddNoteFrequency(int noteNum, float noteRelevanve)
    {
        //todo separate this into treble and bass
        int pureNote = (noteNum + 120) % 12;
        if (oneFrameTrueAllFramesFalse)
        {
            notesAndSummedSongFreqencies[pureNote] = noteRelevanve;
        }
        else
        {
            if (!notesAndSummedSongFreqencies.ContainsKey(pureNote))
            {
                notesAndSummedSongFreqencies[pureNote] = 0;
            }
            notesAndSummedSongFreqencies[pureNote] += noteRelevanve;
        }
    }
}
