using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, ExecuteInEditMode]
public class PrefabManager : MonoBehaviour
{
    private Dictionary<string, PrefabMap> dictionary;

    [System.Serializable]
    public struct PrefabMapMap
    {
        public string mapName;
        public PrefabMap prefabMap;
    }
    public List<PrefabMapMap> prefabMaps;

    private void Reset()
    {
        name = GetType().ToString();
    }

    private void Awake()
    {
        if (dictionary == null)
        {
            dictionary = new Dictionary<string, PrefabMap>();
        }
        Reload();
    }

    private void Start()
    {
        if (Application.isEditor)
        {
            UpdateEditorView();
        }
    }
    void Update()
    {
        if (!Application.isPlaying)
        {
            if (dictionary != null && prefabMaps != null)
            {
                if (dictionary.Count != prefabMaps.Count)
                {
                    Reload();
                }
            }
        }
    }

    void UpdateEditorView()
    {
        if (dictionary != null)
        {

            if (dictionary.Count > 0)
            {

                foreach (var pMap in dictionary)
                {
                    PrefabMapMap pMapMap;
                    pMapMap.mapName = pMap.Key;
                    pMapMap.prefabMap = pMap.Value;
                    foreach (GameObject g in pMap.Value.GetAllPrefabs())
                    {
                        pMap.Value.prefabs.Add(new PrefabMap.Element(g.name, g));
                    }
                    prefabMaps.Add(pMapMap);
                }
            }
        }
    }
    private void LoadDictionary()
    {
        foreach (PrefabMapMap p in prefabMaps)
        {
            dictionary.Add(p.mapName, p.prefabMap);
        }
    }

    public PrefabMap this[string name]
    {
        get
        {
            Reload();
            if (!dictionary.ContainsKey(name))
            {
                dictionary.Add(name, new PrefabMap());
            }
            return dictionary[name];
        }
        set
        {
            if (dictionary[name] == null)
            {
                dictionary.Add(name, value);
            }
            else
            {
                dictionary[name] = value;
            }
        }
    }


    //SLOW, FOR LAZY USE ONLY
    public GameObject Get(string smartPath)
    {
        foreach (var pmm in dictionary)
        {
            foreach (var go in pmm.Value.prefabs)
            {
                if (smartPath.Equals(go.name))
                {
                    return go.value;
                }
            }
        }
        return null;
    }

    public T Get<T>(string smartPath = null)
    {
        string typeName = typeof(T).ToString();
        if (smartPath == null)
        {
            GameObject obj = dictionary[typeName][typeName];
            if (obj == null)
            {
                Debug.LogError("PM search for <" + typeName + "> with object of name: \"" + typeName + "\"failed");
            }
            return obj.GetComponent<T>();
        }
        GameObject obj2 = dictionary[typeName][smartPath];
        if (obj2 == null)
        {
            Debug.LogError("PM search for <" + typeName + "> with object of name: \"" + smartPath + "\"failed");
        }
        return obj2.GetComponent<T>();
    }


    public K Get<T, K>(string smartPath = null)
    {
        SmartBehaviour obj = Get<T>(smartPath) as SmartBehaviour;
        return obj.GetComponent<K>();
    }
    private void Reload()
    {
        if (dictionary == null)
        {
            dictionary = new Dictionary<string, PrefabMap>();
        }
        if (!Application.isPlaying)
        {

            if (dictionary.Count != prefabMaps.Count)
            {
                dictionary.Clear();
                LoadDictionary();
            }
        }
    }

    public string Register(System.Type classType, GameObject obj)
    {
        string classTypeString = classType.ToString();
        this[classTypeString][obj.transform.name] = obj;
        return "[\"" + classTypeString + "\"][\"" + obj.transform.name + "\"]";
    }
}