using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour {

    [SerializeField]
    private Animator animator;
    private Text displayText;

    void OnEnable()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        Debug.Log(clipInfo.Length);
        Destroy(gameObject, clipInfo[0].clip.length);
        displayText = animator.GetComponent<Text>();
    }

    public void SetText(string text)
    {
        displayText.text = text;
    }

    public void  SetTextColor(Color color)
    {
        displayText.color = color;
    }
}
