using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//https://medium.com/giant-scam/algorithmic-beat-mapping-in-unity-real-time-audio-analysis-using-the-unity-api-6e9595823ce4

public class FrequencyAnalyzer : SmartSingleton
{

    public class SpectralFluxInfo
    {
        public float time;
        public float spectralFlux;
        public float threshold;
        public float prunedSpectralFlux;
        public bool isPeak;
    }

    public AudioSource audioSource;

    float[] curSpectrum = new float[8192];
    float[] prevSpectrum = new float[8192];
    public int thresholdWindowSize = 15;
    public float thresholdMultiplier = 1;
    public int indexToProcess = 1;

    List<SpectralFluxInfo> spectralFluxSamplesTreble = new List<SpectralFluxInfo>();
    List<SpectralFluxInfo> spectralFluxSamplesBass = new List<SpectralFluxInfo>();


    public int audioClipSampleRate = 44100;
    //keep the last 20
    CircularBuffer<float[]> pastFrequencies = new CircularBuffer<float[]>(20);
    CircularBuffer<float[]> pastOutputData = new CircularBuffer<float[]>(20);

    public float hzPerBin;

    private Dictionary<int, float> binsToFrequencies = new Dictionary<int, float>();

    public List<float[]> frequencyMinRanges;

    public int tuningFreq = 440;

    private float[] spectrum;

    public float delayAfterPeakDetection = .25f;
    private float nextDetectionTime = 0;
    private void Start()
    {
        if (!GameObject.Find("Video Player"))
        {
            audioClipSampleRate = GetComponent<AudioSource>().clip.frequency;
        }
        hzPerBin = (audioClipSampleRate / 2f) / (8192f);
        for (int i = 0; i < 20; i++)
        {
            pastFrequencies.Add(new float[8192]);
        }
        //File.Delete("debug.txt");
        for (int i = 0; i < 8192; i++)
        {
            binsToFrequencies[i] = i * hzPerBin;
            string str = "index: " + i + "freq: " + i * hzPerBin + Environment.NewLine;

            //File.AppendAllText("debug.txt", str);
        }
        spectrum = new float[8192];
    }



    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("Video Player"))
        {
            AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        }
        else
        {
            audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        }

        pastFrequencies.SetAndIncrement(spectrum);
        float[] vols = new float[8192];

        if (GameObject.Find("Video Player"))
            AudioListener.GetOutputData(vols, 0);
        else
            audioSource.GetOutputData(vols, 0);

        pastOutputData.SetAndIncrement(vols);

        //analyzeSpectrum(spectrum, audioSource.time);
        //analyzeSpectrumMain(spectrum, audioSource.time);

        pm.Get<Equalizer>().BoostNotesNew(spectrum, vols);
        //pm.Get<Equalizer>().BoostNotes(spectrum, vols);

        pm.Get<SongAnalyzer>().AnalyzeData();

        //if (spectralFluxSamplesTreble.Count >= thresholdWindowSize)
        //pm.Get<Equalizer>().BoostNotes(pastFrequencies[pastFrequencies.Index() - thresholdWindowSize], pastOutputData[pastOutputData.Index() - thresholdWindowSize]);
    }

    public void setCurSpectrum(float[] spectrum)
    {
        curSpectrum.CopyTo(prevSpectrum, 0);
        spectrum.CopyTo(curSpectrum, 0);
    }

    public float calculateRectifiedSpectralFlux(int min, int max)
    {
        float sum = 0f;

        // Aggregate positive changes in spectrum data
        for (int i = min; i < max; i++)
        {
            sum += Mathf.Max(0f, curSpectrum[i] - prevSpectrum[i]);
        }
        return sum;
    }

    float getFluxThreshold(List<SpectralFluxInfo> samplesToAnalyze, int spectralFluxIndex)
    {
        // How many samples in the past and future we include in our average
        int windowStartIndex = Mathf.Max(0, spectralFluxIndex - thresholdWindowSize / 2);
        int windowEndIndex = Mathf.Min(samplesToAnalyze.Count - 1, spectralFluxIndex + thresholdWindowSize / 2);

        // Add up our spectral flux over the window
        float sum = 0f;
        for (int i = windowStartIndex; i < windowEndIndex; i++)
        {
            sum += samplesToAnalyze[i].spectralFlux;
        }

        // Return the average multiplied by our sensitivity multiplier
        float avg = sum / (windowEndIndex - windowStartIndex);
        return avg * thresholdMultiplier;
    }

    float getPrunedSpectralFlux(List<SpectralFluxInfo> samplesToAnalyz, int spectralFluxIndex)
    {
        return Mathf.Max(0f, samplesToAnalyz[spectralFluxIndex].spectralFlux - samplesToAnalyz[spectralFluxIndex].threshold);
    }

    bool isPeak(List<SpectralFluxInfo> samplesToAnalyze, int spectralFluxIndex)
    {
        if (samplesToAnalyze[spectralFluxIndex].prunedSpectralFlux > samplesToAnalyze[spectralFluxIndex + 1].prunedSpectralFlux &&
            samplesToAnalyze[spectralFluxIndex].prunedSpectralFlux > samplesToAnalyze[spectralFluxIndex - 1].prunedSpectralFlux)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void analyzeSpectrumCurrent(float[] spectrum, float time)
    {
        // Get current spectral flux from spectrum
        SpectralFluxInfo curInfo = new SpectralFluxInfo();
        curInfo.time = time;
        curInfo.spectralFlux = calculateRectifiedSpectralFlux(512 + 256, 8192);
        spectralFluxSamplesTreble.Add(curInfo);

        // We have enough samples to detect a peak
        //if (spectralFluxSamplesTreble.Count >= thresholdWindowSize)
        {
            // Get Flux threshold of time window surrounding index to process
            //spectralFluxSamplesTreble[indexToProcess].threshold = getFluxThreshold(spectralFluxSamplesTreble, indexToProcess);

            // Only keep amp amount above threshold to allow peak filtering
            //spectralFluxSamplesTreble[indexToProcess].prunedSpectralFlux = getPrunedSpectralFlux(spectralFluxSamplesTreble, indexToProcess);

            // Now that we are processed at n, n-1 has neighbors (n-2, n) to determine peak
            //int indexToDetectPeak = indexToProcess - 1;

            //bool curPeak = isPeak(spectralFluxSamplesTreble, indexToDetectPeak);
            //if (curPeak)
            {
                //spectralFluxSamplesTreble[indexToDetectPeak].isPeak = true;
                {
                    int bin = DetermineHighestBin(spectrum, 0, 13);
                    int note = GetNoteFromBin(bin);
                    float frequency = GetFrequencyFromBin(bin);
                    if (frequency == 0)
                    {
                        return;
                    }
                    //Debug.Log(string.Format("Peak: " + indexToDetectPeak + " Frequency: " + frequency + " Note: " + (NOTE_NAMES)(note % 12)));
                    note += 120;
                    pm.Get<HueManager>().SetColor(0, pm.Get<HueHelper>("HueHelperBass").colorsInOrder[note % 12]);
                    pm.Get<HueManager>().SetColor(1, pm.Get<HueHelper>("HueHelperBass").colorsInOrder[note % 12]);
                    pm.Get<HueHelper>("HueHelperBass").UpdateNote(note % 12);

                    /******************************************/
                    int volBin = DetermineHighestBin(pastOutputData[pastOutputData.Index() - 1], 0, 13);

                    float noteIntensity = pastOutputData[pastOutputData.Index() - 1][volBin];
                    //pm.Get<HueHelper>("HueHelperBass").GetComponent<ColorSpheres>().SetIntensity(note % 12, noteIntensity);
                    /******************************************/
                }
                {
                    int bin = DetermineHighestBin(spectrum, 13, 200);
                    int note = GetNoteFromBin(bin);
                    float frequency = GetFrequencyFromBin(bin);
                    if (frequency == 0)
                    {
                        return;
                    }
                    //Debug.Log(string.Format("Peak: " + indexToDetectPeak + " Frequency: " + frequency + " Note: " + (NOTE_NAMES)(note % 12)));
                    note += 120;
                    pm.Get<HueManager>().SetColor(2, pm.Get<HueHelper>("HueHelperTreble").colorsInOrder[note % 12]);
                    pm.Get<HueManager>().SetColor(3, pm.Get<HueHelper>("HueHelperTreble").colorsInOrder[note % 12]);
                    pm.Get<HueHelper>("HueHelperTreble").UpdateNote(note % 12);

                    /******************************************/
                    int volBin = DetermineHighestBin(pastOutputData[pastOutputData.Index() - 1], 13, 200);
                    float noteIntensity = pastOutputData[pastOutputData.Index() - 1][volBin];
                    //pm.Get<HueHelper>("HueHelperTreble").GetComponent<ColorSpheres>().SetIntensity(note % 12, noteIntensity);
                    /******************************************/

                }
            }
        }
    }
    public void analyzeSpectrum(float[] spectrum, float time)
    {
        // Get current spectral flux from spectrum
        SpectralFluxInfo curInfo = new SpectralFluxInfo();
        curInfo.time = time;
        curInfo.spectralFlux = calculateRectifiedSpectralFlux(512 + 256, 8192);
        spectralFluxSamplesTreble.Add(curInfo);

        // We have enough samples to detect a peak
        if (spectralFluxSamplesTreble.Count >= thresholdWindowSize)
        {
            // Get Flux threshold of time window surrounding index to process
            spectralFluxSamplesTreble[indexToProcess].threshold = getFluxThreshold(spectralFluxSamplesTreble, indexToProcess);

            // Only keep amp amount above threshold to allow peak filtering
            spectralFluxSamplesTreble[indexToProcess].prunedSpectralFlux = getPrunedSpectralFlux(spectralFluxSamplesTreble, indexToProcess);

            // Now that we are processed at n, n-1 has neighbors (n-2, n) to determine peak
            int indexToDetectPeak = indexToProcess - 1;

            bool curPeak = isPeak(spectralFluxSamplesTreble, indexToDetectPeak);
            if (curPeak)
            {
                spectralFluxSamplesTreble[indexToDetectPeak].isPeak = true;
                {
                    int bin = DetermineHighestBin(pastFrequencies[pastFrequencies.Index() - thresholdWindowSize], 0, 13);
                    int note = GetNoteFromBin(bin);
                    float frequency = GetFrequencyFromBin(bin);
                    if (frequency == 0)
                    {
                        return;
                    }
                    //Debug.Log(string.Format("Peak: " + indexToDetectPeak + " Frequency: " + frequency + " Note: " + (NOTE_NAMES)(note % 12)));
                    note += 120;
                    pm.Get<HueManager>().SetColor(0, pm.Get<HueHelper>("HueHelperBass").colorsInOrder[note % 12]);
                    pm.Get<HueManager>().SetColor(1, pm.Get<HueHelper>("HueHelperBass").colorsInOrder[note % 12]);
                    pm.Get<HueHelper>("HueHelperBass").UpdateNote(note % 12);

                    /******************************************/
                    int volBin = DetermineHighestBin(pastOutputData[pastOutputData.Index() - thresholdWindowSize], 0, 13);
                    float noteIntensity = pastOutputData[pastOutputData.Index() - thresholdWindowSize][volBin];
                    //pm.Get<HueHelper>("HueHelperBass").GetComponent<ColorSpheres>().SetIntensity(note % 12, noteIntensity);
                    /******************************************/
                }
                {
                    int bin = DetermineHighestBin(pastFrequencies[pastFrequencies.Index() - thresholdWindowSize], 13, 200);
                    int note = GetNoteFromBin(bin);
                    float frequency = GetFrequencyFromBin(bin);
                    if (frequency == 0)
                    {
                        return;
                    }
                    //Debug.Log(string.Format("Peak: " + indexToDetectPeak + " Frequency: " + frequency + " Note: " + (NOTE_NAMES)(note % 12)));
                    note += 120;
                    pm.Get<HueManager>().SetColor(2, pm.Get<HueHelper>("HueHelperTreble").colorsInOrder[note % 12]);
                    pm.Get<HueManager>().SetColor(3, pm.Get<HueHelper>("HueHelperTreble").colorsInOrder[note % 12]);
                    pm.Get<HueHelper>("HueHelperTreble").UpdateNote(note % 12);

                    /******************************************/
                    int volBin = DetermineHighestBin(pastOutputData[pastOutputData.Index() - thresholdWindowSize], 13, 200);
                    float noteIntensity = pastOutputData[pastOutputData.Index() - thresholdWindowSize][volBin];
                    //pm.Get<HueHelper>("HueHelperTreble").GetComponent<ColorSpheres>().SetIntensity(note % 12, noteIntensity);
                    /******************************************/

                }
            }
        }
    }
    public void analyzeSpectrumMain(float[] spectrum, float time)
    {
        if (Time.time > nextDetectionTime)
        {

            setCurSpectrum(spectrum);

            //analyzeSpectrum(spectrum, time);
            analyzeSpectrumCurrent(spectrum, time);


            if (spectralFluxSamplesTreble.Count >= thresholdWindowSize)
            {

                indexToProcess++;
            }
            nextDetectionTime = Time.time + delayAfterPeakDetection;
        }
    }

    public int GetNoteFromFrequency(float frequency)
    {
        //halfStepsToA4  =  12*log2(freqn/440 Hz).
        int halfStepsToA4 = Mathf.RoundToInt((12 * Mathf.Log(frequency / tuningFreq, 2)));

        //c is four half steps above a4 so
        int noteNum = 57 + halfStepsToA4;
        return noteNum;
    }

    public float GetFrequencyFromBin(int binNumber)
    {
        return binsToFrequencies[binNumber];
    }
    public int GetNoteFromBin(int binNumber)
    {
        float frequency = GetFrequencyFromBin(binNumber);
        if (frequency == 0)
        {
            return 0;
        }
        return GetNoteFromFrequency(frequency);
    }

    internal float GetFrequencyFromNote(int note)
    {
        int a4 = 69;
        int halfStepsToA4 = note - a4;
        float twelfthRootOfTwo = 1.059463094359f;
        float frequency = 440f * Mathf.Pow(twelfthRootOfTwo, halfStepsToA4);
        return frequency;
    }

    public int DetermineHighestBin(float[] bins, int minRange, int maxRange)
    {
        int maxIndex = 0;
        float maxVal = 0;
        for (int i = minRange; i < maxRange; i++)
        {
            if (bins[i] > maxVal)
            {
                maxVal = bins[i];
                maxIndex = i;
            }
        }
        return maxIndex;
    }
}
