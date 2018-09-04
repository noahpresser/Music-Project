using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSpheres : SmartBehaviour {

    public GameObject sphere;

    List<Transform> circleOfFifths = new List<Transform>(12);
    public float distance;
    public float scale;


    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    

    public void GrowCircle(Dictionary<int, SortablePair> noteCounts, int totalCount)
    {
        for (int i = 0; i < 12; i++)
        {
            circleOfFifths[i].transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(scale, scale, scale), (float)noteCounts[i].count / (float)totalCount);
        }
    }

    public void SpawnCircle()
    {
        for (int i = 0; i < 12; i++)
        {
            circleOfFifths.Add(sphere.transform);
        }
        for (int i = 0; i < 12; i++)
        {
            Vector3 spawnLoc = transform.position;
            spawnLoc.x += distance *  Mathf.Sin((Mathf.Deg2Rad)*((float)(360 * ((float)i / (float)12))));
            spawnLoc.y += distance * Mathf.Cos((Mathf.Deg2Rad) * ((float)(360 * ((float)i / (float)12))));
        int index = i;
            if (i % 2 == 1)
            {
                index = (i + 6) % 12;
            }
            circleOfFifths[index] = (GameObject.Instantiate(sphere, spawnLoc, Quaternion.identity, transform).transform);
            circleOfFifths[index].localScale = circleOfFifths[index].localScale * scale;
        }
        for (int i = 0; i < 12; i++)
        {
            //circleOfFifths[i].GetComponent<MeshRenderer>().material.color = GetComponent<HueHelper>().colorsInOrder[i];
            Material m = circleOfFifths[i].GetComponent<MeshRenderer>().material;
            m.SetColor("_Color", GetComponent<HueHelper>().colorsInOrder[i]);
        }
    }

    internal void SetIntensity(int i, float noteIntensity)
    {
        Material m = circleOfFifths[i].GetComponent<MeshRenderer>().material;
        //float newIntensity = Mathf.Lerp(0, 100, noteIntensity * 100
        float newIntensity = noteIntensity * 100;
        if (newIntensity < 0f)
        {
            newIntensity = 0f;
        }
        m.SetFloat("_EmissiveBoost", newIntensity );
        //Debug.Log("inte: " + newIntensity);
    }
}
