using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonLoader : SmartBehaviour {
    [System.Serializable]
    public class JsonData
    {
        public JsonItem[] items;
    }

    [System.Serializable]
    public class JsonItem
    {
        public string key;
        public string value;
    }

    public Dictionary<string, string> dictionary = new Dictionary<string, string>();

    public string filePath;

    public object JsonConvert { get; private set; }

    // Use this for initialization
    new void Awake()
    {
        base.Awake();
    }
    public bool LoadData(string fileName, bool appendDataPath = false)
    {
        return true;
        string filePath = fileName;
        if (appendDataPath)
        {
            filePath = Path.Combine(Application.dataPath, fileName);
        }

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            JsonData loadedData = JsonUtility.FromJson<JsonData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                dictionary.Add(loadedData.items[i].key, loadedData.items[i].value);
            }

            Debug.Log("Data loaded, dictionary contains: " + dictionary.Count + " entries");
        }
        else
        {
            Debug.LogError("Cannot find file!" + filePath);
            return false;
        }
        return true;
    }
    public void SaveData()
    {

        JsonData jData = new JsonData();
        jData.items = new JsonItem[dictionary.Count];
        int index = 0;
        foreach (var item in dictionary)
        {
            jData.items[index] = new JsonItem();
            jData.items[index].key = item.Key;
            jData.items[index].value = item.Value;
            index++;
        }
        string jsonData = JsonUtility.ToJson(jData);
        string configDir = Path.Combine(Application.streamingAssetsPath, "config");
        if (!Directory.Exists(configDir))
        {
            Directory.CreateDirectory(configDir);
        }
        File.WriteAllText(Path.Combine(configDir, "config.json"), jsonData);
    }
}
