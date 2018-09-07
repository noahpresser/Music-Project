using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThresholdVisual : MonoBehaviour
{
    public void Initialize(string name, Color startColor, float startScale)
    {
        name = "Mean";
        GetComponent<MeshRenderer>().material.SetFloat("_Equalize", 1);
        GetComponent<MeshRenderer>().material.SetColor("_Color", startColor);
        transform.localScale = new Vector3(startScale, 10000, 1);
    }

    internal void Initialize(string name, Color startColor, float startScale, float positionOffset)
    {
        name = "Mean";
        GetComponent<MeshRenderer>().material.SetFloat("_Equalize", 1);
        GetComponent<MeshRenderer>().material.SetColor("_Color", startColor);
        transform.localScale = new Vector3(startScale, 10000, 1);
        transform.Translate(new Vector3(0, 0, positionOffset));
    }
}