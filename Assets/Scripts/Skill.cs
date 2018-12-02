﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
/// <summary>
/// Class representing a skill usable by player through a character of his team
/// </summary>
public class Skill
{
    [SerializeField]
    private string skillName;
    [SerializeField]
    private int cost;
    [SerializeField]
    private int tourCooldownLeft;
    [SerializeField]
    private int cooldown;
    [SerializeField]
    private int maxRange;
    [SerializeField]
    private int minRange;
    [SerializeField]
    public List<Effect> effects;
    [SerializeField]
    public delegate bool DelegateCondition();
    [SerializeField]
    public DelegateCondition condition;
    [SerializeField]
    public Sprite abilitySprite;
    [SerializeField]
    private SkillType skillType;


    public SkillType SkillType
    {
        get
        {
            return skillType;
        }

        set
        {
            skillType = value;
        }
    }

    public int TourCooldownLeft
    {
        get
        {
            return tourCooldownLeft;
        }

        set
        {
            tourCooldownLeft = value;
        }
    }

    public int Cooldown
    {
        get
        {
            return cooldown;
        }

        set
        {
            cooldown = value;
        }
    }

    public int Maxrange
    {
        get
        {
            return maxRange;
        }

        set
        {
            maxRange = value;
        }
    }

    public int Minrange
    {
        get
        {
            return minRange;
        }

        set
        {
            minRange = value;
        }
    }

    public int Cost
    {
        get
        {
            return cost;
        }

        set
        {
            cost = value;
        }
    }

    public string SkillName
    {
        get
        {
            return skillName;
        }

        set
        {
            skillName = value;
        }
    }

    public Skill(int cost, int cooldown, List<Effect> effects, SkillType skillType, string skillName, int rangeMin,int rangeMax)
    {
        Cost = cost;
        Cooldown = cooldown;
        tourCooldownLeft = 0;
        this.effects = effects;
        SkillName = skillName;
        this.abilitySprite = Resources.Load<Sprite>("UI_Images/Abilities/" + SkillName);
        SkillType = skillType;
        this.maxRange = rangeMax;
        this.minRange = rangeMin;

    }

    public void Activate()
    {
        GameManager.instance.state = States.UseSkill;
        GameManager.instance.activeSkill = this;
        GameManager.instance.playingPlaceable.Player.ShowSkillEffectTarget(GameManager.instance.playingPlaceable, this);
    }

    ///TODO makes the copy and return if succeeded launching the skill
    public bool Use(LivingPlaceable caster, List<Placeable> targets)
    {
        if (this.tourCooldownLeft > 0)
        {
            return false;
        }
        if (condition != null && !condition.Invoke())
        {
            return false;
        }
        this.tourCooldownLeft = this.cooldown;//On pourrait avoir de la cdr dans les effets afterall
        foreach (Placeable target in targets)
        {
            foreach (Effect effect in effects)
            {
                //makes the deep copy, send it to effect manager and zoo
                Effect effectToConsider = effect.Clone();
                effectToConsider.Launcher = caster;
                //Double dispatch
                target.DispatchEffect(effect);

            }
        }
        return true;
    }
    public string Save()
    {
        string text = JsonUtility.ToJson(this);
        text = "\n" + text + "\n";
        foreach (Effect eff in effects)
        {
            text+=eff.Save();
            
        }
        text += ";";
        return text;
 //       File.WriteAllText(path, text);
    }

    

}
