using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private Skill skill;

    public static SkillInfo currentSkill = null;

    public Text cost;
    public Text cooldown;
    public Image buttonImage;

    [SerializeField]
    private GameObject highlight;
    

    public Skill Skill
    {
        get
        {
            return skill;
        }

        set
        {
            skill = value;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.instance.State != States.TeamSelect && GameManager.instance.State != States.Spawn)
        {
            gameObject.GetComponentInParent<Canvas>().transform.Find("StatsDisplayer").GetComponent<StatDisplayer>().Deactivate();
            gameObject.GetComponentInParent<Canvas>().transform.Find("SkillDisplayer").GetComponent<SkillDisplayer>().Activate(Skill);
        }
        if (Skill.SkillName == "Zip")
        {
            Debug.Log("METTRE ICI LA PREVIEW DE LA ZIPLINE!!!");
            //Skill.effects[0].GetTarget().GetComponent<ZipLine>().Outline();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponentInParent<Canvas>().transform.Find("SkillDisplayer").GetComponent<SkillDisplayer>().Deactivate();
        gameObject.GetComponentInParent<Canvas>().transform.Find("StatsDisplayer").GetComponent<StatDisplayer>().Activate(GameManager.instance.PlayingPlaceable);
        if (Skill.SkillName == "Zip")
        {
            //Skill.effects[0].GetTarget().GetComponent<ZipLine>().Outline();
        }
    }

    public void BecomeCurrentSkill()
    {
        if (currentSkill)
            SkillInfo.currentSkill.SetHighlight(false);
        SkillInfo.currentSkill = this;
        SetHighlight(true);
    }

    public void UpdateButtonInfo()
    {
        buttonImage.sprite = skill.AbilitySprite;
        cost.text = skill.Cost.ToString();
        DisplayAvailability();
    }

    public void DisplayAvailability()
    {
        buttonImage.color = Color.white;
        if (skill.cooldownTurnLeft > 0)
        {
            cooldown.text = skill.cooldownTurnLeft.ToString();
            buttonImage.color = Color.gray;
        }
        if(GameManager.instance.PlayingPlaceable.CurrentPA < skill.Cost)
        {
            buttonImage.color = Color.gray;
        }
    }

    public void SetHighlight(bool active)
    {
        highlight.SetActive(active);
    }

}
