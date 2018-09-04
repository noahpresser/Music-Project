using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SmoothDamper
{

    private Vector3 StartPos;
    public Vector3 startPos
    {
        get { return StartPos; }
        set
        {
            StartPos = value;
            if (smoothLocation != StartPos)
            {
                smoothLocation = StartPos;
            }
        }

    }

    public Vector3 target;
    public Vector3 currentVelocity;
    public float smoothTime;
    public float maxSpeed;

    public Vector3 smoothLocation;

    public Vector3 Smooth()
    {
        smoothLocation = Vector3.SmoothDamp(smoothLocation, target, ref currentVelocity, smoothTime, maxSpeed);
        return smoothLocation;
    }

    public void ResetSmoothing()
    {
        smoothLocation = startPos;
    }
    public bool FinishedSmoothing()
    {
        float differenceX = smoothLocation.x - target.x;
        float differenceY = smoothLocation.y - target.y;
        float differenceZ = smoothLocation.z - target.z;
        return (differenceX + differenceY + differenceZ) < .01f;
    }
}