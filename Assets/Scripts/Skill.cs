
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
    private GameManager gameManager;
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

    public GameManager GameManager
    {
        get
        {
            return gameManager;
        }

        set
        {
            gameManager = value;
        }
    }
    ///TODO
    public void Use()
    {
        
        this.tourCooldownLeft = this.cooldown;
    }
}
