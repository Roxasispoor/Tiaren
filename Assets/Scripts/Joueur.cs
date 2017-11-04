using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Représente un joueur 
/// </summary>
public class Joueur : MonoBehaviour {
    private bool acted;
    private int score;
    private List<Personnage> personnages;
   

    /// <summary>
    /// Permet de savoir si le joueur a agi ce tour ci
    /// </summary>
    public bool Acted
    {
        get
        {
            return acted;
        }

        set
        {
            acted = value;
        }
    }

    /// <summary>
    /// Score du joueur
    /// </summary>
    public int Score
    {
        get
        {
            return score;
        }

        set
        {
            score = value;
        }
    }

    /// <summary>
    /// Liste des personnages du joueur
    /// </summary>
    public List<Personnage> Personnages
    {
        get
        {
            return personnages;
        }

        set
        {
            personnages = value;
        }
    }

   

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
