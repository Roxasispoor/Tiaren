using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinkDisplayer : MonoBehaviour
{
    public List<GameObject> SkillButtons;

    Totem currentLink;

    Camera playerCamera;

    private void Awake()
    {
        if (GameManager.instance.isClient)
        {
            playerCamera = GameManager.instance.GetLocalPlayer().cameraScript.GetComponent<Camera>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 postion = playerCamera.WorldToScreenPoint(currentLink.transform.position);
        gameObject.transform.position = postion;
    }

    public void InitializeLink(Totem totem)
    {
        currentLink = totem;

        for (int i = 0; i < totem.linkSkill.Count; i++)
        {
            SkillButtons[i].GetComponentInChildren<SkillInfo>().Skill = totem.linkSkill[i];
            SkillButtons[i].GetComponentInChildren<SkillInfo>().UpdateButtonInfo();
            SkillButtons[i].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            SkillButtons[i].GetComponentInChildren<Button>().onClick.AddListener(totem.linkSkill[i].Activate);
        }
    }

    public void BreakLink()
    {
        GameManager.instance.PlayingPlaceable.Player.CmdCutLink();
        currentLink = null;
        gameObject.SetActive(false);
    }
}
