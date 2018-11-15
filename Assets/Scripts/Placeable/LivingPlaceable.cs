using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingPlaceable : Placeable
{

    private float maxHP;
    private float currentHP;
    private int pmMax;
    private int currentPM;
    private float currentPA;
    private float paMax;
    private int force;
    private float speed;
    private int dexterity;
    private float speedStack;
    private int jump;
    private float deathLength;

    private List<Skill> skills;
    private List<GameObject> weapons;
    private Weapon equipedWeapon;
    private bool isDead;
    private int counterDeaths;
    private int turnsRemainingCemetery;
    private Vector3 shootPosition;
    private int capacityInUse;
    private List<NodePath> areaOfMouvement;

    private List<Vector3Int> targetArea;
    private List<LivingPlaceable> targetableUnits;
    public Sprite characterSprite;


    public float MaxHP
    {
        get
        {
            return maxHP;
        }

        set
        {
            maxHP = value;
        }
    }

    public float CurrentHP
    {
        get
        {
            return currentHP;
        }

        set
        {
            currentHP = value;
        }
    }

    public int MaxPM
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

    public List<Skill> Skills
    {
        get
        {
            return skills;
        }

        set
        {
            skills = value;
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

    public float Speed
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

    public float SpeedStack
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

    public int TurnsRemaingingCemetery
    {
        get
        {
            return turnsRemainingCemetery;
        }

        set
        {
            turnsRemainingCemetery = value;
        }
    }

    public int CounterDeaths
    {
        get
        {
            return counterDeaths;
        }

        set
        {
            counterDeaths = value;
        }
    }

    public int CurrentPM
    {
        get
        {
            return currentPM;
        }

        set
        {
            currentPM = value;
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

    public bool IsDead
    {
        get
        {
            return isDead;
        }

        set
        {
            isDead = value;
        }
    }

    public List<GameObject> Weapons
    {
        get
        {
            return weapons;
        }

        set
        {
            weapons = value;
        }
    }

    public Weapon EquipedWeapon
    {
        get
        {
            return equipedWeapon;
        }

        set
        {
            equipedWeapon = value;
        }
    }

    public int CapacityinUse
    {
        get
        {
            return capacityInUse;
        }

        set
        {
            capacityInUse = value;
        }
    }

    public float CurrentPA
    {
        get
        {
            return currentPA;
        }

        set
        {
            currentPA = value;
        }
    }

    public float PaMax
    {
        get
        {
            return paMax;
        }

        set
        {
            paMax = value;
        }
    }

    public int Jump
    {
        get
        {
            return jump;
        }

        set
        {
            jump = value;
        }
    }

    public List<NodePath> AreaOfMouvement
    {
        get
        {
            return areaOfMouvement;
        }

        set
        {
            areaOfMouvement = value;
        }
    }

    public List<Vector3Int> TargetArea
    {
        get
        {
            return targetArea;
        }

        set
        {
            targetArea = value;
        }
    }

    public List<LivingPlaceable> TargetableUnits
    {
        get
        {
            return targetableUnits;
        }

        set
        {
            targetableUnits = value;
        }
    }

    public float DeathLength
    {
        get
        {
            return deathLength;
        }

        set
        {
            deathLength = value;
        }
    }


    /// <summary>
    /// Create the effect damage and all effects of weapon to the gameEffectManager, then launch resolution
    /// doesn't check if target can me touched, just read. Add bonus for height. Pick the point that hurts most
    /// </summary>
    /// <param name="target"></param>
    /// <param name="gameEffectManager"></param>
    public Vector3 ShootDamage(Placeable target)
    {

        float nbDmgs;

        if (EquipedWeapon.ScalesOnForce)
        {

            nbDmgs = EquipedWeapon.BaseDamage + EquipedWeapon.StatMultiplier * force;
        }
        else
        {
            nbDmgs = EquipedWeapon.BaseDamage + EquipedWeapon.StatMultiplier * dexterity;

        }
        float maxDamage = 0;
        HitablePoint maxHit = null;
        float nbDamagea;
        foreach (HitablePoint hitPoint in CanHit(target))
        {

            Vector3 shotaPos = this.transform.position + shootPosition;
            Vector3 ciblaPos = target.transform.position + hitPoint.RelativePosition;
            float sinFactor = (shotaPos.y - ciblaPos.y) /
                (shotaPos - ciblaPos).magnitude;

            Vector3 vect1 = this.transform.forward;
            Vector3 vect2 = (ciblaPos - shotaPos);
            vect1.y = 0;
            vect2.y = 0;
            vect1.Normalize();
            vect2.Normalize();

            float sinDirection = Vector3.Cross(vect1, vect2).magnitude;
            nbDamagea = nbDmgs; //* (1 + sinFactor * sinMultiplier - sinDirection * sinMultiplier2);
            if (nbDamagea > maxDamage)
            {
                maxDamage = nbDamagea;
                maxHit = hitPoint;
            }

        }

        //TODO use gameEffectManager
        return target.transform.position + maxHit.RelativePosition;
    }

    public override void DispatchEffect(Effect effect)
    {
        effect.TargetAndInvokeEffectManager(this);
    }

    /// <summary>
    /// return points of placeable where player can shoot
    /// </summary>
    /// <returns></returns>
    public List<HitablePoint> CanHit(Placeable placeable)
    {
        List<HitablePoint> pointsToSend = new List<HitablePoint>();

        Vector3 starting = this.transform.position + this.shootPosition;


        foreach (HitablePoint x in placeable.HitablePoints)
        {
            Vector3 arrival = placeable.transform.position + x.RelativePosition;
            Vector3 direction = arrival - starting;
            if (direction.magnitude > this.EquipedWeapon.Range)
            {
                x.Shootable = false;
            }
            else
            {
                
                RaycastHit[] hits = Physics.RaycastAll(starting,
                   direction, (starting - arrival).magnitude + 0.1f);//les arrondis i guess
                int significantItemShot = 0;
                foreach (RaycastHit hit in hits) //a while would be better
                {

                    Placeable placeableShot = hit.transform.gameObject.GetComponent(typeof(Placeable)) as Placeable;
                    if (!(placeableShot.TraversableBullet == TraversableType.ALLTHROUGH ||
                        placeableShot.TraversableBullet == TraversableType.ALLIESTHROUGH && placeableShot.Player != this.Player))
                    {
                        significantItemShot++;
                    }
                }
                if (significantItemShot == 1)
                {
                    x.Shootable = true;
                    pointsToSend.Add(x);
                }
                else
                {
                    x.Shootable = false;
                }
            }
        }

        return pointsToSend;
    }


    public override bool IsLiving()
    {
        return true;
    }

    // Use this for initialization
    void Awake()
    {
        this.name = "default";
        this.Walkable = false;
        this.Movable = true;
        this.Destroyable = true;
        this.TraversableChar = TraversableType.ALLIESTHROUGH;
        this.TraversableBullet = TraversableType.NOTHROUGH;
        this.GravityType = GravityType.SIMPLE_GRAVITY;

        this.Crushable = CrushType.CRUSHDEATH;
        this.OnDestroyEffects = new List<Effect>();
        this.HitablePoints = new List<HitablePoint>
        {
            new HitablePoint(new Vector3(0, 0.5f, 0), 1)
        };
        this.OnStartTurn = new List<Effect>();
        this.OnEndTurn = new List<Effect>();
        this.maxHP = 100;
        this.CurrentHP = 100;
        this.MaxPM = 3;
        this.CurrentPM = 3;
        this.PaMax = 1;
        this.CurrentPA = 1;
        this.Jump = 1;
        this.Force = 100;
        this.Speed = 100;
        this.SpeedStack = 1 / Speed;
        this.Dexterity = 100;
        this.Skills = new List<Skill>();
        this.Weapons = new List<GameObject>();
        this.IsDead = false;
        this.CounterDeaths = 0;
        this.TurnsRemaingingCemetery = 0;
        this.ShootPosition = new Vector3(0, 0.5f, 0);
        this.AreaOfMouvement = new List<NodePath>();
        List<Effect> ListEffects = new List<Effect>();
        List<Effect> ListEffects2 = new List<Effect>();
        ListEffects.Add(new Push(null, this, 2, 500));
        ListEffects2.Add(new CreateBlock(Grid.instance.prefabsList[0], new Vector3Int(0, 1, 0)));
        Skill skill1 = new Skill(0, 1, ListEffects, SkillType.BLOCK, "push",0,1);
        Skill skill2 = new Skill(0, 1, ListEffects2, SkillType.BLOCK, "spell2",0,1);
        Skills.Add(skill1);
        Skills.Add(skill2);
        this.characterSprite = Resources.Load<Sprite>("UI_Images/Characters/" + name);
        this.AreaOfMouvement = new List<NodePath>();
        targetArea = new List<Vector3Int>();
        targetableUnits = new List<LivingPlaceable>();
}

    
    void OnMouseOver()
    {


        if (Input.GetMouseButtonUp(1))
        {
            GameManager.instance.ShotPlaceable = this;
        }


    }

    /// <summary>
    /// method to call to destroy the object 
    /// </summary>
    /// 
    override
    public void Destroy()
    {

        if (this.Destroyable)
        {
            foreach (var effet in this.OnDestroyEffects)
            {
                effet.Use();
            }
        }

        this.IsDead = true;
        this.gameObject.SetActive(false);
        Grid.instance.GridMatrix[GetPosition().x, GetPosition().y, GetPosition().z] = null;
        CounterDeaths++;
       //TODO gérer le temps de respawn
    }

    public void ChangeMaterialAreaOfMovement(Material pathfinding)
    {
       
        foreach (NodePath node in AreaOfMouvement)
        {
            if(Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial==null) //if we haven't seen this one before
            { 
           // Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().enabled = true;
            Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial = Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().material;
            Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().material = pathfinding;
            }
        }
        GameManager.instance.ResetAllBatches();
        
    }

    public void ResetAreaOfMovement()
    {
        foreach (NodePath node in AreaOfMouvement)
        {
            if (Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial!=null)//if we haven't already reset this one
            { 
           // Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().enabled = true;
            Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().material = Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial;
            Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial = null;
            }
        }
        AreaOfMouvement.Clear();
    }

    public void ChangeMaterialAreaOfTarget(Material materialTarget)
    {

        foreach (NodePath node in AreaOfMouvement)
        {
            if (Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial == null) //if we haven't seen this one before
            {
                // Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().enabled = true;
                Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial = Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().material;
                Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().material = materialTarget;
            }
        }
        GameManager.instance.ResetAllBatches();

    }

    public void ResetAreaOfTarget()
    {
        foreach (Vector3Int node in targetArea)
        {
            if (Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial != null)//if we haven't already reset this one
            {
               
                Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().material = Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial;
                Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial = null;
            }
        }
        targetArea.Clear();
    }
}
