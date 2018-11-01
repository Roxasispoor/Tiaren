
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class representing a skill usable by player through a character of his team
/// </summary>
public class Skill
{
    private int cost;
    private int tourCooldownLeft;
    private int cooldown;
    private List<Effect> effects;
    public delegate bool DelegateCondition();
    public DelegateCondition condition;
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
    
    ///TODO makes the copy and return if succeeded launching the skill
    public bool Use(LivingPlaceable caster, List<Placeable> targets)
    {
        if(this.tourCooldownLeft>0)
        {
            return false;
        }
        if(condition!=null && !condition.Invoke())
        {
            return false;
        }
        this.tourCooldownLeft = this.cooldown;//On pourrait avoir de la cdr dans les effets afterall
        foreach(Placeable target in targets)
        {
            foreach(Effect effect in effects)
            {
                //makes the deep copy, send it to effect manager and zoo
                Effect effectToConsider = effect.Clone();
             //   effectToConsider.SetTarget(target);
            }
        }
        return true;
    }
}
