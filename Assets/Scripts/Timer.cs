using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Permet de lancer des comptes a rebours
/// </summary>
public class Timer : MonoBehaviour {
    private bool isFinished;
    private float targetTime;
    public Text textToUpdate;

  
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
        this.targetTime = time;
    }

 void  Update()
    {

        targetTime -= Time.deltaTime;

        if (!IsFinished && targetTime <= 0.0f)
        {
            IsFinished = true;
        }
        if(textToUpdate!=null)
        {
            textToUpdate.text = targetTime.ToString();
        }
    }

}
