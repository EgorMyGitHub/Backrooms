using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Timer : MonoBehaviour
{
    public int second { get; private set; }
    
    public float millisecond { get; private set; }

    private bool isStartTimer;
    
    public void StartTimer()
    {
        isStartTimer = true;
    }

    private void Update()
    {
        if(!isStartTimer)
            return;

        millisecond += Time.deltaTime * 1000;

        if (millisecond > 1000)
        {
            millisecond = 0;
            second++;
        }
    }
}
