using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, ExecuteInEditMode]
public class PrefabMap
{
    Dictionary<string, GameObject> dictionary;

    [System.Serializable]
    public struct Element
    {
        public string name;
        public GameObject value;
        public Element(string _name, GameObject _value)
        {
            name = _name;
            value = _value;
        }
    };

    public List<Element> prefabs;

    public PrefabMap()
    {
        prefabs = new List<Element>();

        dictionary = new Dictionary<string, GameObject>();
        LoadDictionary();
    }

    private void LoadDictionary()
    {
        foreach (Element e in prefabs)
        {
            dictionary.Add(e.name, e.value);
        }
    }


    public GameObject this[string name]
    {
        get
        {
            Reload();
            if (!dictionary.ContainsKey(name))
            {
                dictionary.Add(name, null);
            }
            return dictionary[name];
        }
        set
        {
            if (!dictionary.ContainsKey(name))
            {
                dictionary.Add(name, value);
            }
            else
            {
                dictionary[name] = value;
            }
        }
    }

    public List<GameObject> GetAllPrefabs()
    {
        Reload();
        List<GameObject> allPrefabs = new List<GameObject>();
        foreach (var item in dictionary)
        {
            allPrefabs.Add(item.Value);
        }
        return allPrefabs;
    }

    private void Reload()
    {
        if (!Application.isPlaying)
        {

            if (dictionary == null)
            {
                dictionary = new Dictionary<string, GameObject>();
            }
            if (dictionary.Count != prefabs.Count)
            {
                dictionary.Clear();
                LoadDictionary();
            }
        }
    }
}