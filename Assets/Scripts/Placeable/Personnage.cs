using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personnage : LivingPlaceable {

    
    

    //constructeur classique de personnages
    public Personnage(bool walkable, List<Effect> onWalkEffects, bool movable, bool destroyable, TraversableType traversableChar, TraversableType traversableBullet,
        GravityType gravityType, bool pickable, EcraseType ecrasable, List<Effect> onDestroyEffects
        , List<HitablePoint> hitablePoints, List<Effect> onDebutTour, List<Effect> onFinTour, Joueur joueur
        , float pvMax, float pvActuels, int pmMax, int pmActuels, int force, int speed, int dexterity, int miningPower, int speedstack,
        List<Competence> competences, List<Arme> armes, int nbFoisFiredThisTurn, bool estMort
        , int nbFoisMort, int tourRestantsCimetiere, Vector3 shootPosition) :
         base( walkable,onWalkEffects,  movable,  destroyable,  traversableChar,  traversableBullet,
         gravityType,  pickable,  ecrasable,  onDestroyEffects
        ,  hitablePoints, onDebutTour, onFinTour,joueur
        ,  pvMax,  pvActuels,  pmMax,  pmActuels,  force,  speed,  dexterity,  miningPower,  speedstack,
        competences, armes,  nbFoisFiredThisTurn,  estMort
        ,  nbFoisMort,  tourRestantsCimetiere,  shootPosition)
    {

    }






    /// <summary>
    /// Copie l'objet
    /// </summary>
    /// <returns>Retourne une copie de l'objet</returns>
    new public virtual LivingPlaceable Clone()
    {
        var copy = (Personnage)base.Clone();
        return copy;
    }

    /// <summary>
    /// Méthode a appeler lors de la destruction du personnage
    /// </summary>
   new  protected virtual void Destroy()
    {
        base.Destroy();
       
        TourRestantsCimetiere = 2 * NbFoisMort; // Fonction qui sera peut être modifiée à l'équilibrage
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
