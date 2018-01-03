
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Classe représentant une compétence utilisable par le joueur
/// </summary>
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
    /// <summary>
    /// Ajoute les effets au gameEffectManager et lance la résolution
    /// </summary>
    public void Use()
    {
        gameManager.GameEffectManager.ToBeTreated.AddRange(this.effects);
        gameManager.GameEffectManager.Solve();
        this.tourCooldownLeft = this.cooldown;
    }
}
