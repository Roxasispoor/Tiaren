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
    private StatDisplayer statsDisplayer;
    private SkillDisplayer skillDisplayer;

    

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

    private void Awake()
    {
        statsDisplayer = gameObject.GetComponentInParent<Canvas>().transform.Find("StatsDisplayer").GetComponent<StatDisplayer>();
        skillDisplayer = gameObject.GetComponentInParent<Canvas>().transform.Find("SkillDisplayer").GetComponent<SkillDisplayer>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.instance.State != States.TeamSelect && GameManager.instance.State != States.Spawn)
        {
            statsDisplayer.Deactivate();
            skillDisplayer.Activate(Skill);
            if (skill.oneClickUse)
            {
                skill.Preview(null);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        skillDisplayer.Deactivate();
        statsDisplayer.Activate(GameManager.instance.PlayingPlaceable);
        if (skill.oneClickUse)
        {
            skill.UnPreview(null);
        }
    }

    public void ChangeCurrentSkill()
    {
        if (GameManager.instance.State != States.Link)
        {
            if (currentSkill)
            {
                currentSkill.SetHighlight(false);
                if (currentSkill == this)
                {
                    currentSkill = null;
                    return;
                }
            }
            currentSkill = this;
            SetHighlight(true);
        }
    }

    public void UpdateButtonInfo()
    {
        buttonImage.sprite = skill.AbilitySprite;
        cost.text = skill.Cost.ToString();
        DisplayAvailability();
    }

    /// <summary>
    /// Called to update cooldowns, gray filters and yellow border
    /// </summary>
    public void DisplayAvailability()
    {
        if (skill == null)
        {
            return;
        }

        buttonImage.color = Color.white;
        if (skill.cooldownTurnLeft > 0)
        {
            cooldown.gameObject.SetActive(true);
            cooldown.text = skill.cooldownTurnLeft.ToString();
            buttonImage.color = Color.gray;
        }
        else
        {
            cooldown.gameObject.SetActive(false);
        }
        if(GameManager.instance.PlayingPlaceable.CurrentPA < skill.Cost)
        {
            buttonImage.color = Color.gray;
        }
        if (currentSkill == this)
        {
            currentSkill = null;
            SetHighlight(false);
        }
    }

    public void SetHighlight(bool active)
    {
        highlight.SetActive(active);
    }

}
