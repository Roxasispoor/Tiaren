using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillDisplayer : MonoBehaviour {

    public TMP_Text skillName;
    public TMP_Text description;
    public Image image;
    public TMP_Text cooldown;
    public TMP_Text cost;
    public TMP_Text power;

    public void Activate(Skill skill)
    {
        gameObject.SetActive(true);
        skillName.text = skill.SkillName;
        description.text = skill.Description;
        cost.text = "Cost : " + skill.Cost;
        cooldown.text = "Cooldown : " + skill.Cooldown;
        image.sprite = skill.AbilitySprite;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
