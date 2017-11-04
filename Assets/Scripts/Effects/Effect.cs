using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * Effects are added to the GameEffectManager that will treat them. Effects should be kept simple and be independant of one another.
 * */
public abstract class Effect : MonoBehaviour {
    private Placeable cible;
    private Placeable lanceur;
    private int tourEffetActif; //-1 = inactif 0=on arrête. On utilisera int.MaxValue/2 quand c'est indépendant
    protected Effect()
    {

    }
    protected Effect (Placeable cible, Placeable lanceur)
    {
        this.cible = cible;
        this.lanceur = lanceur;
    }
    public Placeable Cible
    {
        get
        {
            return cible;
        }

        set
        {
            cible = value;
        }
    }

    public Placeable Lanceur
    {
        get
        {
            return lanceur;
        }

        set
        {
            lanceur = value;
        }
    }

    public int TourEffetActif
    {
        get
        {
            return tourEffetActif;
        }

        set
        {
            tourEffetActif = value;
        }
    }

    // Use this for initialization
    public abstract void Use();
    
}
