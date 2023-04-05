using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Time_Storage : MonoBehaviour
{
    private float SimTime;

    private void Awake()
    {
        SimTime = Time.time;
    }

    public float getTime()
    {
        return SimTime;
    }
}
