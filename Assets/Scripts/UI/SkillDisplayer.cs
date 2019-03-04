﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillDisplayer : MonoBehaviour {

    public Text skillName;
    public Text description;
    public Image image;
    public Text cooldown;
    public Text cost;
    public Text power;

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
