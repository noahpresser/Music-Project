using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public enum INTERVAL_NAMES
{
    U,
    m2,
    M2,
    m3,
    M3,
    P4,
    TT,
    P5,
    m6,
    M6,
    m7,
    M7,
    P8,
    m9,
    M9,
    m10,
    M10,
    P11,
    P12,
    m13,
    M13,
    m14,
    M14,
    P15,
    MAX
}
public enum NOTE_NAME
{
    C,
    Cs,
    D,
    Eb,
    E,
    F,
    Fs,
    G,
    Ab,
    A,
    Bb,
    B,
    MAX
}

public class ToneManager : SmartSingleton
{
    public static  int GetNoteNumber(NOTE_NAME n)
    {
        return (int)n;
    }
}
