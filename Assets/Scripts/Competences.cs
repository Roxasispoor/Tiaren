
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Classe représentant une compétence utilisable par le joueur
/// </summary>
public class Skill
{
    private int cost;
    private int tourCooldownLeft;
    private int cooldown;
    private List<Effect> effects;
    public delegate bool DelegateCondition();
    public DelegateCondition condition;
    private CompetenceType competenceType;
    private GameManager gameManager;
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
