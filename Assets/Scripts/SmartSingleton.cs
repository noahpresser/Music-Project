using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SmartSingleton : SmartBehaviour{

    // Use this for initialization
    protected new void Awake()
    {
        base.Awake();
    }

    private void Reset()
    {
        name = GetType().ToString();
    }

}
