
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Competence {
    private int cost;
    private int tourCooldownLeft;
    private int cooldown;
    private List<Effect> effects;
    public delegate bool DelegateCondition();
    public DelegateCondition condition;
    private CompetenceType competenceType;
    private Game gameManager;
    public CompetenceType CompetenceType
    {
        get
        {
            return competenceType;
        }

        set
        {
            competenceType = value;
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

    public Game GameManager
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

    public void Use()
    {
        gameManager.GameEffectManager.ToBeTreated.AddRange(this.effects);
        
        this.tourCooldownLeft = this.cooldown;
    }
}
