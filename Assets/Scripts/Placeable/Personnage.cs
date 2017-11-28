using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Personnage : LivingPlaceable {

   

    //constructeur classique de personnages
    public Personnage(Vector3Int position,bool walkable, List<Effect> onWalkEffects, bool movable, bool destroyable, TraversableType traversableChar, TraversableType traversableBullet,
        GravityType gravityType, bool pickable, EcraseType ecrasable, List<Effect> onDestroyEffects
        , List<HitablePoint> hitablePoints, List<Effect> onDebutTour, List<Effect> onFinTour, Joueur joueur
        , float pvMax, float pvActuels, int pmMax, int pmActuels, int force, int speed, int dexterity, int miningPower, int speedstack,
        List<Competence> competences, List<Arme> armes, int nbFoisFiredThisTurn, bool estMort
        , int nbFoisMort, int tourRestantsCimetiere, Vector3 shootPosition) :
         base(position, walkable,onWalkEffects,  movable,  destroyable,  traversableChar,  traversableBullet,
         gravityType,  pickable,  ecrasable,  onDestroyEffects
        ,  hitablePoints, onDebutTour, onFinTour,joueur
        ,  pvMax,  pvActuels,  pmMax,  pmActuels,  force,  speed,  dexterity,  miningPower,  speedstack,
        competences, armes,  nbFoisFiredThisTurn,  estMort
        ,  nbFoisMort,  tourRestantsCimetiere,  shootPosition)
    {

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
            new HitablePoint(new Vector3(0, 0, 0), 1)
        };
        this.OnDebutTour = new List<Effect>();
        this.OnFinTour = new List<Effect>();
        this.PvMax = 100;
        this.PmMax = 3;
        this.PmActuels = 3;
        this.Force = 100;
        this.Speed = 100;
        this.Dexterity = 100;
        this.SpeedStack = 0;
        this.Competences = new List<Competence>();
        this.Armes = new List<Arme>();
        this.NbFoisFiredThisTurn = 0;
        this.EstMort = false;
        this.NbFoisMort = 0;
        this.TourRestantsCimetiere = 0;
        this.ShootPosition = new Vector3(0, 0, 0);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
