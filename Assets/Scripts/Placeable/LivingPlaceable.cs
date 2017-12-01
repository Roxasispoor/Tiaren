using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingPlaceable : Placeable {
    
    private float pvMax;
    public float pvActuels;
    private int pmMax;
    private int pmActuels;
    private int force;
    private int speed;
    private int dexterity;
    private int miningPower;
    private int speedStack;
    private List<Competence> competences;
    private List<GameObject> armes;
    private Arme equipedArm;
    private float sinMultiplier = 0.66f;
    private int nbFoisFiredThisTurn;
    private bool estMort;
    private int nbFoisMort;
    private int tourRestantsCimetiere;
    private Vector3 shootPosition;


 /**   public LivingPlaceable(Vector3Int position,bool walkable, List<Effect> onWalkEffects, bool movable, bool destroyable, TraversableType traversableChar, TraversableType traversableBullet,
        GravityType gravityType, bool pickable, EcraseType ecrasable, List<Effect> onDestroyEffects
        , List<HitablePoint> hitablePoints, List<Effect> onDebutTour, List<Effect> onFinTour,Joueur joueur
        , float pvMax, float pvActuels, int pmMax,int pmActuels,int force,int speed,int dexterity,int miningPower,int speedstack,
        List<Competence> competences, List<GameObject> armes,int nbFoisFiredThisTurn,bool estMort
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
    this.Armes = armes;
    this.equipedArm = armes[0].GetComponent<Arme>();//la première arme est automatiquement équipée
    
    this.NbFoisFiredThisTurn = nbFoisFiredThisTurn;
    this.EstMort = estMort;
    this.NbFoisMort = nbFoisMort;
    this.TourRestantsCimetiere = tourRestantsCimetiere;
    this.shootPosition = shootPosition;



    }
    **/

    /// <summary>
    /// Crée l'effet Dégat et ajoute tous les effets de l'arme au gameEffectManager puis lance la résolution. 
    /// ne vérifie pas si on peut toucher la cible,se cibntente de lire. Ajoute le bonus de hauteur
    /// Choisi le point qui fait le plus mal
    /// </summary>
    /// <param name="cible"></param>
    /// <param name="gameEffectManager"></param>
    public Vector3 ShootDamage(Placeable cible)
    {
        
        float nbDmgs;

        if (EquipedArm.ScalesOnForce)
        {

            nbDmgs = EquipedArm.BaseDamage + EquipedArm.StatMultiplier * force;
         }
        else
        {
             nbDmgs = EquipedArm.BaseDamage + EquipedArm.StatMultiplier * dexterity;

        }
        float maxdmg = 0;
        HitablePoint maxHit= null;
        float nbDmga;
        foreach(HitablePoint hitPoint in CanHit(cible))
        { 
           
            Vector3 shotaPos = this.transform.position + shootPosition;
            Vector3 ciblaPos = cible.transform.position + hitPoint.RelativePosition;
            float sinfactor = (shotaPos.y - ciblaPos.y) /
                (shotaPos - ciblaPos).magnitude;
           
             nbDmga =nbDmgs * (1 +  sinfactor* sinMultiplier);
            if(nbDmga>maxdmg)
            {
                maxdmg = nbDmga;
                maxHit = hitPoint;
            }
            
        }        //
        Debug.Log(maxdmg);
        
        //on prépare le damage en conséquence avant?
        this.NbFoisFiredThisTurn++;

        this.GameManager.GameEffectManager.ToBeTreated.AddRange(this.EquipedArm.OnShootEffects);
        this.GameManager.GameEffectManager.ToBeTreated.Add(new Damage(cible, this, maxdmg));

        this.GameManager.GameEffectManager.Solve();
        return cible.transform.position+maxHit.RelativePosition;
    }

    /// <summary>
    /// Retourne les point du placeable sur lesquels il est possible de tirer
    /// </summary>
   
    /// <returns></returns>
    public List<HitablePoint> CanHit(Placeable placeable)
    {
        List<HitablePoint> arenvoyer=new List<HitablePoint>();

        Vector3 depart = this.transform.position + this.shootPosition;
      

        foreach (HitablePoint x in placeable.HitablePoints)
        {
            Vector3 arrivee= placeable.transform.position + x.RelativePosition;
            Vector3 direction = arrivee - depart;
           if(direction.magnitude>this.EquipedArm.Range)
            {
                x.Shootable = false;
            }
           else
            { 

            Debug.DrawRay(depart,
               direction,Color.green,100);
            RaycastHit[] hits = Physics.RaycastAll(depart,
               direction ,(depart-arrivee).magnitude+0.1f);//les arrondis i guess
            int significantItemShot = 0;
            foreach (RaycastHit hit in hits) //pas opti, un while serait mieux
            {
                
              Placeable  placeableshot = hit.transform.gameObject.GetComponent(typeof(Placeable)) as Placeable;
                if (!(placeableshot.TraversableBullet == TraversableType.ALLTHROUGH ||
                    placeableshot.TraversableBullet==TraversableType.ALLIESTHROUGH && placeableshot.Joueur != this.Joueur))
                {
                    significantItemShot++;
                }
            }
            if(significantItemShot==1)
            {
                x.Shootable = true;
                arenvoyer.Add(x);
            }
            else
            {
                x.Shootable = false;
            }
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

    public bool EstMort
    {
        get
        {
            return estMort;
        }

        set
        {
            estMort = value;
        }
    }

    public List<GameObject> Armes
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

    public Arme EquipedArm
    {
        get
        {
            return equipedArm;
        }

        set
        {
            equipedArm = value;
        }
    }

    new void OnMouseOver()
    {

        Debug.Log("AAAAAAAAAAAAAAH");
        if(Input.GetMouseButtonUp(1))
        {
            GameManager.ShotPlaceable = this;
        }

    }

    /// <summary>
    /// Méthode a appeler lors de la destruction de l'objet
    /// </summary>
    /// 
    override
    public void Detruire()
    {

        if (this.Destroyable)
        {
            foreach (var effet in this.OnDestroyEffects)
            {
                effet.Use();
            }
        }

        this.EstMort = true;
        this.gameObject.SetActive(false);
        this.GameManager.GrilleJeu.Grid[Position.x, Position.y, Position.z] = null;
        NbFoisMort++;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
