using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingUI : MonoBehaviour
{
    public float totalTime;
    private float currentTime;
    private bool pulseStarted = false;
    public Image pulsingImage;

    public void StartPulse()
    {
        gameObject.SetActive(true);
        currentTime = totalTime;
        pulseStarted = true;
        StartCoroutine(FadeAway());
    }

    IEnumerator FadeAway()
    {
        while (pulseStarted)
        {
            currentTime -= Time.deltaTime;
            Color color = pulsingImage.color;
            color.a = currentTime / totalTime;
            pulsingImage.color = color;
            if (pulsingImage.color.a < 0.5)
            {
                gameObject.SetActive(false);
                pulseStarted = false;
            }
            yield return 0;
        }
    }
}
