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
    private float targetTime = 0;
    public Text textToUpdate;
    public Slider TimerSlider;

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
        TimerSlider.maxValue = time;
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
        if (textToUpdate != null)
        {
            textToUpdate.text = "Time Left : " + targetTime.ToString();
            TimerSlider.value = targetTime;
        }
    }
  
}
