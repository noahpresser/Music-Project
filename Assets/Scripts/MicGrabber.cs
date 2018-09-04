using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MicGrabber : SmartSingleton
{
    public bool useMic;
    public AudioMixerGroup silentMixer;
    // Use this for initialization
    void Start()
    {

        foreach (var item in Microphone.devices)
        {
        }

        if (!useMic)
        {
            return;
        }
        AudioSource audio = pm.Get<FrequencyAnalyzer, AudioSource>();
        audio.outputAudioMixerGroup = silentMixer;
            audio.clip = Microphone.Start(Microphone.devices[Microphone.devices.Length - 1], true, 1, 44100);
        audio.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { };
        audio.Play();
        audio.volume = 1;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void grabMic()
    {

    }
}
