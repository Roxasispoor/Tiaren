using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {
    private bool isFinished;
    private float targetTime;

    public Timer()
    {
        this.IsFinished = false;
    }

    public bool IsFinished
    {
        get
        {
            return isFinished;
        }

        set
        {
            isFinished = value;
        }
    }

    public void StartTimer(float time)
    {
        this.targetTime = time;
    }

 void  Update()
    {

        targetTime -= Time.deltaTime;

        if (targetTime <= 0.0f)
        {
            IsFinished = true;
        }

    }

}
