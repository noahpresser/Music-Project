using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EqualizeObj : MonoBehaviour {

    public GameObject prefab;
    public float maxHeight;
    public float currentHeight;
    public int binNum;
    public Color color;
    public EqualizeObj(float _maxHeight, float _currentHeight, int _binNum, Color _color)
    {
        maxHeight = _maxHeight;
        currentHeight = _maxHeight;
        binNum = _binNum;
        color = _color;
    }


    void SetHeight(float newHeight)
    {
        currentHeight = newHeight;
        GetComponent<MeshRenderer>().material.SetFloat("_Equalize", newHeight);
    }
}
