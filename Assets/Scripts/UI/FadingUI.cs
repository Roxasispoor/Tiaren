using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadingUI : MonoBehaviour
{
    public float totalTime;
    private float currentTime;
    private bool pulseStarted = false;

    public void StartPulseImage(Image pulsingImage)
    {
        gameObject.SetActive(true);
        currentTime = totalTime;
        pulseStarted = true;
        StartCoroutine(FadeAway(pulsingImage));
    }

    IEnumerator FadeAway(Image pulsingImage)
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
        yield return 0;
    }

    public void StartPulseText(TMP_Text pulsingText)
    {
        gameObject.SetActive(true);
        currentTime = totalTime;
        pulseStarted = true;
        StartCoroutine(FadeAway(pulsingText));
    }

    IEnumerator FadeAway(TMP_Text pulsingText)
    {
        while (pulseStarted)
        {
            currentTime -= Time.deltaTime;
            Color color = pulsingText.color;
            color.a = currentTime / totalTime;
            pulsingText.color = color;
            if (pulsingText.color.a < 0.5)
            {
                gameObject.SetActive(false);
                pulseStarted = false;
            }
            yield return 0;
        }
    }
}
