using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe représentant un personnage
/// </summary>
public class Personnage : LivingPlaceable {


    /// <summary>
    /// Méthode a appeler lors de la destruction du personnage
    /// </summary>
    override
    public void Detruire()
    {
        Debug.Log("maman je passe a la télé");
        base.Detruire();
       
        TourRestantsCimetiere = 2 * NbFoisMort; // Fonction qui sera peut être modifiée à l'équilibrage
    }

    // Use this for initialization
    void Awake () {

        //this.Position = new Vector3Int(0, 4, 0);
        this.Walkable = false;
        this.Movable = true;
        this.Destroyable = true;
        this.TraversableChar = TraversableType.ALLIESTHROUGH;
        this.TraversableBullet = TraversableType.NOTHROUGH;
        this.GravityType = GravityType.GRAVITE_SIMPLE;
        this.Pickable = false;
        this.Ecrasable = EcraseType.ECRASEDEATH;
        this.OnDestroyEffects = new List<Effect>();
        this.HitablePoints = new List<HitablePoint>
        {
            new HitablePoint(new Vector3(0, 0.5f, 0), 1)
        };
        this.OnDebutTour = new List<Effect>();
        this.OnFinTour = new List<Effect>();
        this.PvMax = 100;
        this.PvActuels = 100;

        this.PmMax = 3;
        this.PmActuels = 3;
        this.Force = 100;
        this.Speed = 100 ;
        this.SpeedStack = 1/Speed;
        this.Dexterity = 100;
        this.Competences = new List<Competence>();
        this.Armes = new List<GameObject>();
        this.NbFoisFiredThisTurn = 0;
        this.EstMort = false;
        this.NbFoisMort = 0;
        this.TourRestantsCimetiere = 0;
        this.ShootPosition = new Vector3(0, 0.5f, 0);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
