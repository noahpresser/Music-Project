using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HueManager : SmartSingleton
{

    public List<HueLamp> lights;
    public static HueManager instance;

    public class CircularBuffer<T>
    {
        public List<T> items;

        public T this[int i]
        {
            get { return items[i % items.Count]; }
            set { items[i % items.Count] = value; }
        }
    }

    public List<Color> colors;

    private int currentColor;

    public float nextUpdateTime = 0;
    private void Awake()
    {
        base.Awake();
        if (instance)
        {
            Debug.LogError("two hue managers");
        }
        else
        {
            instance = this;
        }
    }
    // Use this for initialization
    void Start()
    {
        foreach (var l in lights)
        {
            colors.Add(Color.red);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetColor(int index, Color c)
    {
        lights[index].color = c;
    }
    public void SetAllColors(Color c)
    {
        foreach (var light in lights)
        {
            light.color = c;
        }
        return;
        //SetColorsWithDelay(c);
    }

    public void SetColorsWithDelay(Color c)
    {
        if (Time.time > nextUpdateTime)
        {
            for (int i = 0; i < 3; i++)
            {
                colors[i] = colors[(i + 1)];
            }
            colors[3] = c;

            for (int i = 0; i < colors.Count; i++)
            {
                lights[i].color = colors[i];
            }

            nextUpdateTime = Time.time + .25f;
        }
    }


    List<int> last200 = new List<int>();
    Dictionary<int, int> countOfNotes = new Dictionary<int, int>();

    public void SetColorsByPriority(int noteAsInt)
    {
        {
            //prioritize list by number of reads
            if (last200.Count >= 2000)
            {
                last200.RemoveAt(0);
            }
            last200.Add(noteAsInt);
        }
        countOfNotes.Clear();
        foreach (int i in last200)
        {
            if (!countOfNotes.ContainsKey(i))
            {
                countOfNotes[i] = 0;
            }
            countOfNotes[i]++;
        }


        int lightIndex = 3;
        while (lightIndex >= 0)
        {
            int noteName = 0;
            int highestNoteCount = 0;
            string whatup = "";
            foreach (var item in countOfNotes)
            {
                if (item.Value > highestNoteCount)
                {
                    highestNoteCount = item.Value;
                    noteName = item.Key;
                }
            }
            Debug.Log("lightIndex: " + lightIndex + " noteName " + (NOTE_NAME)noteName + " count " + highestNoteCount);
            countOfNotes.Remove(noteName);
            Color fullTintColor = pm.Get<HueHelper>().colorsInOrder[noteName];
            fullTintColor.a = 1;
            lights[lightIndex].color = fullTintColor;

            lightIndex--;
        }

    }
}
