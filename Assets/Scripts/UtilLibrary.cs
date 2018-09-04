/* Created 8-9-2018 by Noah Presser to provide various common utility functions for the enVu codebase
 */
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UtilLibrary : MonoBehaviour
{

    /**************************************************************************
     * Enables or disables all and object and all of it's children in the hierarchy
     * Useful for menus
     **************************************************************************/
    public static void EnableGameObjectAndChildren(GameObject g, bool enable)
    {
        Transform tf = g.transform;
        for (int i = 0; i < tf.childCount; i++)
        {
            Transform childTF = tf.GetChild(i);
            if (tf.childCount > 0)
            {
                EnableGameObjectAndChildren(childTF.gameObject, enable);
            }
            childTF.gameObject.SetActive(enable);
        }
        g.SetActive(enable);
    }


    /**************************************************************************
    * Saves a texture as a png to the given filepath
    **************************************************************************/
    public static void WriteTextureToFile(Texture2D textureToSave, string filePath)
    {
        byte[] bytes;
        bytes = textureToSave.EncodeToPNG();
        System.IO.File.WriteAllBytes(
            filePath, bytes);
    }

    public static Texture2D LoadTexture(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }
}
