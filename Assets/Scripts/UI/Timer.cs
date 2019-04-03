using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Class for timer and countdown
/// </summary>
public class Timer : MonoBehaviour
{
    private bool isFinished;
    private float maxTime;
    private float targetTime = 0;
    public Image timerDisplay;
    public Gradient gradient;

    public Timer()
    {
        this.isFinished = false;
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
        targetTime = time;
        maxTime = time;
        timerDisplay.fillAmount = 1;
        isFinished = false;
    }

    void Update()
    {
        if (targetTime > 0)
        {
            targetTime -= Time.deltaTime;
        }
        if (!IsFinished && targetTime <= 0.0f)
        {
            targetTime = 0;
            IsFinished = true;
        }
        timerDisplay.fillAmount = targetTime/maxTime;
        timerDisplay.color = gradient.Evaluate(targetTime / maxTime);
    }
  
}
