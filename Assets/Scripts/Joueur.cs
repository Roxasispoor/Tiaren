using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Représente un joueur 
/// </summary>
public class Joueur : MonoBehaviour {
    private bool acted;
    private int score;
    
    public List<GameObject> personnages=new List<GameObject>();
    public List<int> numeroPrefab;


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

    public List<GameObject> Personnages
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

    public List<int> NumeroPrefab
    {
        get
        {
            return numeroPrefab;
        }

        set
        {
            numeroPrefab = value;
        }
    }



    /// <summary>
    /// Liste des personnages du joueur
    /// </summary>



    // Use this for initialization
    void Start () {
        this.acted = false;
        this.score = 0;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
