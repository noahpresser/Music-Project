﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Equalizer : SmartBehaviour
{

    public GameObject cubeEqualizer;
    public float startScale;
    public float maxScale;
    public bool showThresholds  = false;



    private int numBins = 8192;

    GameObject[] equalizerCylinders = new GameObject[8192];


    public float thresholdMultiplier;
    public Dictionary<string, ThresholdVisual> thresholdVisuals = new Dictionary<string, ThresholdVisual>();
    // Use this for initialization
    void Start()
    {
        GameObject t0 = Instantiate(cubeEqualizer);
        t0.AddComponent<ThresholdVisual>().Initialize("mean", Color.grey, startScale, 0);
        thresholdVisuals.Add("mean", t0.GetComponent<ThresholdVisual>());

        GameObject t1 = Instantiate(cubeEqualizer);
        t1.AddComponent<ThresholdVisual>().Initialize("stDev1", Color.red, startScale, 1);
        thresholdVisuals.Add("stDev1", t1.GetComponent<ThresholdVisual>());

        GameObject t2 = Instantiate(cubeEqualizer);
        t2.AddComponent<ThresholdVisual>().Initialize("stDev2", Color.yellow, startScale, 2);
        thresholdVisuals.Add("stDev2", t2.GetComponent<ThresholdVisual>());

        GameObject t3 = Instantiate(cubeEqualizer);
        t3.AddComponent<ThresholdVisual>().Initialize("stDev3", Color.green, startScale, 3);
        thresholdVisuals.Add("stDev3", t3.GetComponent<ThresholdVisual>());

        if (!showThresholds)
        {
            foreach (var item in thresholdVisuals)
            {
                item.Value.gameObject.SetActive(false);
                //item.Value.GetComponent<ThresholdVisual>().GetComponent<MeshRenderer>().material.SetColor("_Color", Color.clear);
            }
        }
    }

    public float SetThresholdVisual(string visualName, float threshold)
    {
        GameObject thresh = thresholdVisuals[visualName].gameObject;
        SetAndLerpIntensity(ref thresh, threshold);
        return threshold;
    }

    void SpawnCylindersInitially()
    {
        for (int i = 0; i < numBins; i++)
        {
            equalizerCylinders[i] = GameObject.Instantiate(cubeEqualizer);

            Color equalizerBarColor = GetEqualizerBarColor(i);
            equalizerCylinders[i].GetComponent<MeshRenderer>().material.SetColor("_Color", equalizerBarColor);
            equalizerCylinders[i].GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", equalizerBarColor);
            Vector3 spawnPos = new Vector3(i * cubeEqualizer.GetComponent<MeshRenderer>().bounds.extents.x * 6, transform.position.y, transform.position.z);
            equalizerCylinders[i].transform.position = spawnPos;
            equalizerCylinders[i].transform.localScale = new Vector3(startScale, transform.localScale.y, transform.localScale.z);
        }
    }

    void SpawnCylindersEqualRange()
    {
        Color lastSpawnColor = Color.red;

        Dictionary<int, int> noteBinCount = new Dictionary<int, int>();
        for (int i = 0; i < numBins; i++)
        {
            Color equalizerBarColor = GetEqualizerBarColor(i);
            int noteValue = pm.Get<FrequencyAnalyzer>().GetNoteFromBin(i);
            if (!noteBinCount.ContainsKey(noteValue))
            {
                noteBinCount[noteValue] = 0;
            }
            noteBinCount[noteValue]++;
        }

        int maxNoteBinCount = 0;
        foreach (var item in noteBinCount)
        {
            if (item.Value > maxNoteBinCount)
            {
                maxNoteBinCount = item.Value;
            }
            //Debug.Log("Note: " + item.Key + " Frequency:" + pm.Get<FrequencyAnalyzer>().GetFrequencyFromNote(item.Key) + " Bincount: " + item.Value);
        }

        float noteStartPos = 0;
        float maxYScale = 1;
        int lastNote = 0;
        int noteInBinIndex = 1;
        for (int i = 0; i < numBins; i++)
        {
            int noteValue = pm.Get<FrequencyAnalyzer>().GetNoteFromBin(i);
            if (lastNote != noteValue)
            {
                noteStartPos += maxYScale;
                noteInBinIndex = 1;
            }
            float yScale = (float)noteInBinIndex / (float)noteBinCount[noteValue];
            float xStartPos = yScale;
            equalizerCylinders[i] = GameObject.Instantiate(cubeEqualizer);
            Color equalizerBarColor = GetEqualizerBarColor(i);
            equalizerCylinders[i].GetComponent<MeshRenderer>().material.SetColor("_Color", equalizerBarColor);
            Vector3 spawnPos = new Vector3(noteStartPos + yScale * (noteInBinIndex), transform.position.y, transform.position.z);
            equalizerCylinders[i].transform.position = spawnPos;
            equalizerCylinders[i].transform.localScale = new Vector3(startScale, yScale, transform.localScale.z);
            noteInBinIndex++;
        }
    }

    List<int> notes = new List<int>();
    Dictionary<int, List<int>> NoteToBins = new Dictionary<int, List<int>>();
    void SpawnOnlyNoteCylinders()
    {
        Dictionary<int, int> binToNote = new Dictionary<int, int>();
        for (int i = 0; i < numBins; i++)
        {
            Color equalizerBarColor = GetEqualizerBarColor(i);
            int noteValue = pm.Get<FrequencyAnalyzer>().GetNoteFromBin(i);
            binToNote[i] = noteValue;
        }

        for (int i = 0; i < numBins; i++)
        {
            Color equalizerBarColor = GetEqualizerBarColor(i);
            int noteValue = pm.Get<FrequencyAnalyzer>().GetNoteFromBin(i);
            if (!NoteToBins.ContainsKey(noteValue))
            {
                NoteToBins[noteValue] = new List<int>();
            }
            NoteToBins[noteValue].Add(i);
        }
        //Debug.Log("noteToBins: " + NoteToBins.Count);
        foreach (var note in NoteToBins.Keys)
        {
            notes.Add(note);
        }
        //Debug.Log("notes: " + notes.Count);

        notes.Sort();
        foreach (var note in notes)
        {
            //Debug.Log("Notes");
        }
        int octaveNumber = -2;
        GameObject parentOrganizer = new GameObject("first org", typeof(Transform));
        parentOrganizer.transform.parent = transform;
        for (int i = 0; i < notes.Count; i++)
        {
            if (notes[i] % 12 == 0)
            {
                string name = "Octave: " + octaveNumber;
                parentOrganizer = new GameObject(name, typeof(Transform));
                parentOrganizer.transform.parent = transform;
                octaveNumber++;
            }

            equalizerCylinders[i] = GameObject.Instantiate(cubeEqualizer, parentOrganizer.transform);
            equalizerCylinders[i].name = "Note " + (NOTE_NAME)(notes[i] % 12) + " | " + notes[i].ToString();

            int noteColor = (notes[i] + 120) % 12;
            Color equalizerBarColor = pm.Get<HueHelper>("HueHelperTreble").colorsInOrder[noteColor];
            equalizerCylinders[i].GetComponent<MeshRenderer>().material.SetColor("_Color", equalizerBarColor);
            Vector3 spawnPos = new Vector3(i * cubeEqualizer.GetComponent<MeshRenderer>().bounds.extents.x * 8, transform.position.y, transform.position.z);
            equalizerCylinders[i].transform.position = spawnPos;
            equalizerCylinders[i].transform.localScale = new Vector3(startScale, transform.localScale.y * 2, transform.localScale.z);
        }
    }
    private Color GetEqualizerBarColor(int i)
    {
        int note = pm.Get<FrequencyAnalyzer>().GetNoteFromBin(i);
        //Debug.Log("note:" + note);
        if (note < 0)
        {
            note += 120;
        }
        return pm.Get<HueHelper>("HueHelperTreble").colorsInOrder[note % 12];
    }

    public void BoostNotesNew(float[] bins, float[] vols)
    {
        float sumOfFrequenciesInBins = 0;
        for (int i = 0; i < bins.Length; i++)
        {
            sumOfFrequenciesInBins += bins[i];
        }
        if (equalizerCylinders[0] == null)
        {
            //SpawnCylindersInitially();
            SpawnOnlyNoteCylinders();
        }
        for (int i = 0; i < notes.Count; i++)
        {
            //boost equalizer based on bintensity
            //equalizerCylinders[i].GetComponent<MeshRenderer>().material.SetFloat("_Equalize", bins[i] * 50);
            equalizerCylinders[i].GetComponent<MeshRenderer>().material.SetFloat("_Equalize", 1);
            //equalizerCylinders[i].GetComponent<MeshRenderer>().material.SetFloat("_EmmissiveBoost", vols[i] * 100);
            equalizerCylinders[i].GetComponent<MeshRenderer>().material.SetFloat("_EmmissiveBoost", 1);

            float biggestIntensityInNotesOfBins = 0;
            foreach (int item in NoteToBins[notes[i]])
            {
                if (bins[item] > biggestIntensityInNotesOfBins)
                {
                    biggestIntensityInNotesOfBins = bins[item];
                }
                sumOfFrequenciesInBins += bins[item];
            }


            SetAndLerpIntensity(ref equalizerCylinders[i], biggestIntensityInNotesOfBins);

            pm.Get<SongAnalyzer>().AddNoteToSong(notes[i], biggestIntensityInNotesOfBins);
        }
    }

    private void SetAndLerpIntensity(ref GameObject equalizerCylinder, float noteFrequency)
    {
        Vector3 vStartScale = new Vector3(startScale, equalizerCylinder.transform.localScale.y, equalizerCylinder.transform.localScale.z);
        Vector3 vEndScale = new Vector3(maxScale, equalizerCylinder.transform.localScale.y, equalizerCylinder.transform.localScale.z);
        equalizerCylinder.transform.localScale = Vector3.LerpUnclamped(vStartScale, vEndScale, noteFrequency * 50);
        if (equalizerCylinder.transform.localScale.x < vStartScale.x)
        {
            equalizerCylinder.transform.localScale = vStartScale;
        }
    }

    public void BoostNotes(float[] bins, float[] vols)
    {
        if (equalizerCylinders[0] == null)
        {
            //SpawnCylindersInitially();
            SpawnCylindersEqualRange();
        }


        for (int i = 0; i < bins.Length; i++)
        {
            //boost equalizer based on bintensity
            //equalizerCylinders[i].GetComponent<MeshRenderer>().material.SetFloat("_Equalize", bins[i] * 50);
            equalizerCylinders[i].GetComponent<MeshRenderer>().material.SetFloat("_Equalize", 1);
            equalizerCylinders[i].GetComponent<MeshRenderer>().material.SetFloat("_EmmissiveBoost", vols[i] * 100);
            Vector3 vStartScale = new Vector3(startScale, equalizerCylinders[i].transform.localScale.y, equalizerCylinders[i].transform.localScale.z);
            Vector3 vEndScale = new Vector3(maxScale, equalizerCylinders[i].transform.localScale.y, equalizerCylinders[i].transform.localScale.z);
            equalizerCylinders[i].transform.localScale = Vector3.LerpUnclamped(vStartScale, vEndScale, bins[i] * 50);

            if (equalizerCylinders[i].transform.localScale.x < vStartScale.x)
            {
                equalizerCylinders[i].transform.localScale = vStartScale;
            }
        }
    }

}

