using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingPlaceable : Placeable {
    
    private float pvMax;
    private float pvActuels;
    private int pmMax;
    private int pmActuels;
    private int force;
    private int speed;
    private int dexterity;
    private int miningPower;
    private int speedStack;
    private List<Competence> competences;
    private List<Arme> armes;
    private Arme equipedArm;
    private float cosMultiplier = 0.66f;
    private int nbFoisFiredThisTurn;
    private bool estMort;
    private int nbFoisMort;
    private int tourRestantsCimetiere;
    private Vector3 shootPosition;


    public LivingPlaceable(Vector3Int position,bool walkable, List<Effect> onWalkEffects, bool movable, bool destroyable, TraversableType traversableChar, TraversableType traversableBullet,
        GravityType gravityType, bool pickable, EcraseType ecrasable, List<Effect> onDestroyEffects
        , List<HitablePoint> hitablePoints, List<Effect> onDebutTour, List<Effect> onFinTour,Joueur joueur
        , float pvMax, float pvActuels, int pmMax,int pmActuels,int force,int speed,int dexterity,int miningPower,int speedstack,
        List<Competence> competences, List<Arme> armes,int nbFoisFiredThisTurn,bool estMort
        ,int nbFoisMort,int tourRestantsCimetiere,Vector3 shootPosition) : 
        base(position, walkable, onWalkEffects, movable, destroyable, traversableChar,  traversableBullet,
         gravityType,  pickable,  ecrasable, onDestroyEffects
        ,  hitablePoints,  onDebutTour,  onFinTour,joueur)
    {
    this.pvMax=pvMax;
    this.pvActuels=pvActuels;
    this.pmMax=pmMax;
    this.PmActuels=pmActuels;
    this.Force=force;
    this.Speed=speed;
    this.Dexterity=dexterity;
    this.miningPower=miningPower;
    this.speedStack = speedstack;
    this.competences = competences;
    this.armes = armes;
    this.equipedArm = armes[0];//la première arme est automatiquement équipée
    
    this.NbFoisFiredThisTurn = nbFoisFiredThisTurn;
    this.estMort = estMort;
    this.NbFoisMort = nbFoisMort;
    this.TourRestantsCimetiere = tourRestantsCimetiere;
    this.shootPosition = shootPosition;



    }
    /// <summary>
    /// Crée l'effet Dégat et ajoute tous les effets de l'arme au gameEffectManager puis lance la résolution. 
    /// ne vérifie pas si on peut toucher la cible. Ajoute le bonus de hauteur
    /// </summary>
    /// <param name="cible"></param>
    /// <param name="gameEffectManager"></param>
    public void Shoot(Placeable cible, GameEffectManager gameEffectManager)
    {
        
        float nbDmgs;

        if (equipedArm.ScalesOnForce)
        {

            nbDmgs = equipedArm.BaseDamage + equipedArm.StatMultiplier * force;
         }
        else
        {
             nbDmgs = equipedArm.BaseDamage + equipedArm.StatMultiplier * dexterity;

        }

        nbDmgs *= 1 + Mathf.Cos((this.transform.position.z - cible.transform.position.z) / Vector3.Distance(cible.transform.position, this.transform.position)) * cosMultiplier;
        //
               
        new Damage(cible, this, nbDmgs);
        //on prépare le damage en conséquence avant?

        gameEffectManager.ToBeTreated.AddRange(this.equipedArm.OnShootEffects);
        gameEffectManager.Solve();
    }

    /// <summary>
    /// Retourne les point du placeable sur lesquels il est possible de tirer
    /// </summary>
   
    /// <returns></returns>
    public List<HitablePoint> CanHit(Placeable placeable)
    {
        List<HitablePoint> arenvoyer=new List<HitablePoint>();

        Vector3 depart = this.transform.position + this.shootPosition;
        //hits = Physics.SphereCastAll(transform.position,equipedArm.Range,transform.forward);
        foreach (HitablePoint x in placeable.HitablePoints)
        {
            Vector3 arrivee= placeable.transform.position + x.RelativePosition;
            RaycastHit[] hits = Physics.RaycastAll(depart,
                depart - arrivee,(depart-arrivee).magnitude);
            int significantItemShot = 0;
            foreach (RaycastHit hit in hits) //pas opti, un while serait mieux
            {
                
              Placeable  placeableshot = hit.transform.gameObject.GetComponent(typeof(Placeable)) as Placeable;
                if (placeableshot.TraversableBullet == TraversableType.ALLTHROUGH ||
                    placeableshot.TraversableBullet==TraversableType.ALLIESTHROUGH && placeableshot.Joueur != this.Joueur)
                {
                    significantItemShot++;
                }
            }
            if(significantItemShot==1)
            {
                arenvoyer.Add(x);
            }
        }

        return arenvoyer;
    }


    public float PvMax
    {
        get
        {
            return pvMax;
        }

        set
        {
            pvMax = value;
        }
    }

    public float PvActuels
    {
        get
        {
            return pvActuels;
        }

        set
        {
            pvActuels = value;
        }
    }

    public int PmMax
    {
        get
        {
            return pmMax;
        }

        set
        {
            pmMax = value;
        }
    }

    public List<Competence> Competences
    {
        get
        {
            return competences;
        }

        set
        {
            competences = value;
        }
    }

    public List<Arme> Armes
    {
        get
        {
            return armes;
        }

        set
        {
            armes = value;
        }
    }

    public int Force
    {
        get
        {
            return force;
        }

        set
        {
            force = value;
        }
    }

    public int Speed
    {
        get
        {
            return speed;
        }

        set
        {
            speed = value;
        }
    }

    public int Dexterity
    {
        get
        {
            return dexterity;
        }

        set
        {
            dexterity = value;
        }
    }

    public int SpeedStack
    {
        get
        {
            return speedStack;
        }

        set
        {
            speedStack = value;
        }
    }

    public int TourRestantsCimetiere
    {
        get
        {
            return tourRestantsCimetiere;
        }

        set
        {
            tourRestantsCimetiere = value;
        }
    }

    public int NbFoisMort
    {
        get
        {
            return nbFoisMort;
        }

        set
        {
            nbFoisMort = value;
        }
    }

    public int PmActuels
    {
        get
        {
            return pmActuels;
        }

        set
        {
            pmActuels = value;
        }
    }

    public int NbFoisFiredThisTurn
    {
        get
        {
            return nbFoisFiredThisTurn;
        }

        set
        {
            nbFoisFiredThisTurn = value;
        }
    }

    public Vector3 ShootPosition
    {
        get
        {
            return shootPosition;
        }

        set
        {
            shootPosition = value;
        }
    }


    /// <summary>
    /// Copie l'objet
    /// </summary>
    /// <returns>Retourne une copie de l'objet</returns>
    new protected virtual LivingPlaceable Clone()
    {
        var copy = (LivingPlaceable)base.Clone();
        return copy;
    }

    /// <summary>
    /// Méthode a appeler lors de la destruction de l'objet
    /// </summary>
    /// 
    
    new protected virtual void Destroy()
    {
        base.Destroy();
        this.estMort = true;
        NbFoisMort++;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
