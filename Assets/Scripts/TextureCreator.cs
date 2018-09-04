using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureCreator : SmartBehaviour {

    //used for height and width
    public Texture2D source;
    public List<float> dummyNoteIntensities = new List<float>();

    public GameObject water;
    private void Start()
    {
        for (int i = 0; i < 124; i++)
        {
            dummyNoteIntensities.Add(i);
        }
        CreateNormalMap(source, dummyNoteIntensities .ToArray());
    }
    public Texture2DArray CreateNormalMap(Texture2D source, float[] noteIntensities)
    {
        Texture2DArray normalTexture;
        float xDelta = 0;
        float yDelta = 0;
        normalTexture = new Texture2DArray(source.width, source.height, 1, TextureFormat.ARGB32, true);

        water.GetComponent<MeshRenderer>().material.SetTexture("_Texture2D_Array", normalTexture);
        return normalTexture;
    }
}
