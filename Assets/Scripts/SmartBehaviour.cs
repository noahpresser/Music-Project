using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SmartBehaviour : MonoBehaviour {

    internal PrefabManager pm;

    // Use this for initialization
    protected void Awake()
    {
        pm = GameObject.FindGameObjectWithTag("PrefabManager").GetComponent<PrefabManager>();
        pm.Register(GetType(), gameObject);
    }

}
