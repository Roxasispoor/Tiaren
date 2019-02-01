using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

[Serializable]
public class LivingPlaceable : Placeable
{
    /// <summary>
    /// Used to save position, only actualized at this point
    /// </summary>
    [SerializeField]
    private string className = "default";
    [SerializeField]
    private Vector3 positionSave;
    [SerializeField]
    private string playerPosesser;
    [SerializeField]
    private Attribute maxHP;
    [SerializeField]
    private Attribute currentHP;
    [SerializeField]
    private Attribute pmMax;
    [SerializeField]
    private Attribute currentPM;
    [SerializeField]
    private Attribute currentPA;
    [SerializeField]
    private Attribute paMax;
    [SerializeField]
    private Attribute force;
    [SerializeField]
    private Attribute speed;
    [SerializeField]
    private Attribute dexterity;
    [SerializeField]
    private Attribute speedStack;
    [SerializeField]
    private Attribute jump;
    [SerializeField]
    private Attribute def;
    [SerializeField]
    private Attribute mdef;
    [SerializeField]
    private Attribute mstr;
    [SerializeField]
    private Attribute deathLength;
    [SerializeField]
    private List<Skill> skills;
    [SerializeField]
    private List<GameObject> weapons;
    [SerializeField]
    private SpriteRenderer circleTeam;
    private Weapon equipedWeapon;
    [SerializeField]
    private bool isDead;
    private int counterDeaths;
    private int turnsRemainingCemetery;
    private Vector3 shootPosition;
    private int capacityInUse;
    private List<NodePath> areaOfMouvement;

    private List<Placeable> targetArea;
    private List<LivingPlaceable> targetableUnits;
    public Sprite characterSprite;

    // Variable used for the highligh
    private bool isTarget = false;
    private Renderer rend;
    //Shaders (used for the highlight)

    [SerializeField]
    private Shader originalShader;
    private Shader outlineShader;
    private Color previousColor = Color.white;

    public float MaxHP
    {
        get
        {

            return maxHP.Value;
        }

        set
        {
            maxHP.BaseValue = value;
        }
    }

    public float CurrentHP
    {
        get
        {
            return currentHP.Value;
        }

        set
        {
            currentHP.BaseValue = value;
        }
    }

    public override Player Player
    {
        get
        {
            return base.Player;
        }

        set
        {
            base.Player = value;
            if (GameManager.instance && GameManager.instance.player1 != null && GameManager.instance.player1 == Player.gameObject)
            {
                playerPosesser = "player1";
            }
            if (GameManager.instance && GameManager.instance.player2 != null && GameManager.instance.player2 == Player.gameObject)
            {
                playerPosesser = "player2";
            }

        }
    }
    public int MaxPM
    {
        get
        {
            return (int)pmMax.Value;
        }

        set
        {
            pmMax.BaseValue = value;
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


    public float Force
    {
        get
        {
            return force.Value;
        }

        set
        {
            force.BaseValue = value;
        }
    }

    public float Speed
    {
        get
        {
            return speed.Value;
        }

        set
        {
            speed.BaseValue = value;
        }
    }

    public float Dexterity
    {
        get
        {
            return dexterity.Value;
        }

        set
        {
            dexterity.BaseValue = value;
        }
    }

    public float SpeedStack
    {
        get
        {
            return speedStack.Value;
        }

        set
        {
            speedStack.BaseValue = value;
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
            return (int)currentPM.Value;
        }

        set
        {
            currentPM.BaseValue = value;
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

    public int CurrentPA
    {
        get
        {
            return (int)currentPA.Value;
        }

        set
        {
            currentPA.BaseValue = value;
        }
    }

    public int PaMax
    {
        get
        {
            return (int)paMax.Value;
        }

        set
        {
            paMax.BaseValue = value;
        }
    }

    public int Jump
    {
        get
        {
            return (int)jump.Value;
        }

        set
        {
            jump.BaseValue = value;
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


    public List<LivingPlaceable> TargetableUnits
    {
        get
        {
            return targetableUnits;
        }

        set
        {
            if (targetableUnits != null)
            {
                foreach (LivingPlaceable living in targetableUnits)
                {
                    living.UnHighlightTarget();
                }
            }

            if (value != null)
            {
                foreach (LivingPlaceable living in value)
                {
                    living.HighlightForSkill();
                }
            }
            targetableUnits = value;
        }
    }

    public float DeathLength
    {
        get
        {
            return deathLength.Value;
        }

        set
        {
            deathLength.BaseValue = value;
        }
    }

    public List<Placeable> TargetArea
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

    public float Mstr
    {
        get
        {
            return mstr.Value;
        }

        set
        {
            mstr.BaseValue = value;
        }
    }

    public float Mdef
    {
        get
        {
            return mdef.Value;
        }

        set
        {
            mdef.BaseValue = value;
        }
    }

    public float Def
    {
        get
        {
            return def.Value;
        }

        set
        {
            def.BaseValue = value;
        }
    }

    public float MaxHPFlat
    {
        get
        {
            return maxHP.FlatModif;
        }

        set
        {
            maxHP.FlatModif = value;
        }
    }

    public float CurrentHPFlat
    {
        get
        {
            return currentHP.FlatModif;
        }
        set
        {
            currentHP.FlatModif = value;
        }
    }
    public float MaxPMFlat
    {
        get
        {
            return pmMax.FlatModif;
        }
        set
        {
            pmMax.FlatModif = value;
        }
    }
    public float CurrentPMFlat
    {
        get
        {
            return currentPM.FlatModif;
        }
        set
        {
            currentPM.FlatModif = value;
        }
    }
    public float CurrentPAFlat
    {
        get
        {
            return currentPA.FlatModif;
        }
        set
        {
            currentPA.FlatModif = value;
        }
    }
    public float MaxPAFlat
    {
        get
        {
            return paMax.FlatModif;
        }
        set
        {
            paMax.FlatModif = value;
        }
    }
    public float ForceFlat
    {
        get
        {
            return force.FlatModif;
        }
        set
        {
            force.FlatModif = value;
        }
    }
    public float SpeedFlat
    {
        get
        {
            return speed.FlatModif;
        }
        set
        {
            speed.FlatModif = value;
        }
    }
    public float DexterityFlat
    {
        get
        {
            return dexterity.FlatModif;
        }
        set
        {
            dexterity.FlatModif = value;
        }
    }
    public float SpeedStackFlat
    {
        get
        {
            return speedStack.FlatModif;
        }
        set
        {
            speedStack.FlatModif = value;
        }
    }
    public float JumpFlat
    {
        get
        {
            return jump.FlatModif;
        }
        set
        {
            jump.FlatModif = value;
        }
    }
    public float DefFlat
    {
        get
        {
            return def.FlatModif;
        }
        set
        {
            def.FlatModif = value;
        }
    }
    public float MDefFlat
    {
        get
        {
            return mdef.FlatModif;
        }
        set
        {
            mdef.FlatModif = value;
        }
    }
    public float MStrFlat
    {
        get
        {
            return mstr.FlatModif;
        }
        set
        {
            mstr.FlatModif = value;
        }
    }
    public float DeathLengthFlat
    {
        get
        {
            return deathLength.FlatModif;
        }
        set
        {
            deathLength.FlatModif = value;
        }
    }

    public float MaxHPPercent
    {
        get
        {
            return maxHP.PercentModif;
        }
        set
        {
            maxHP.PercentModif = value;
        }
    }
    public float CurrentHPPercent
    {
        get
        {
            return currentHP.PercentModif;
        }
        set
        {
            currentHP.PercentModif = value;
        }
    }
    public float MaxPMPercent
    {
        get
        {
            return pmMax.PercentModif;
        }
        set
        {
            pmMax.PercentModif = value;
        }
    }
    public float CurrentPMPercent
    {
        get
        {
            return currentPM.PercentModif;
        }
        set
        {
            currentPM.PercentModif = value;
        }
    }
    public float CurrentPAPercent
    {
        get
        {
            return currentPA.PercentModif;
        }
        set
        {
            currentPA.PercentModif = value;
        }
    }
    public float MaxPAPercent
    {
        get
        {
            return paMax.PercentModif;
        }
        set
        {
            paMax.PercentModif = value;
        }
    }
    public float ForcePercent
    {
        get
        {
            return force.PercentModif;
        }
        set
        {
            force.PercentModif = value;
        }
    }
    public float SpeedPercent
    {
        get
        {
            return speed.PercentModif;
        }
        set
        {
            speed.PercentModif = value;
        }
    }
    public float DexterityPercent
    {
        get
        {
            return dexterity.PercentModif;
        }
        set
        {
            dexterity.PercentModif = value;
        }
    }
    public float SpeedStackPercent
    {
        get
        {
            return speedStack.PercentModif;
        }
        set
        {
            speedStack.PercentModif = value;
        }
    }
    public float JumpPercent
    {
        get
        {
            return jump.PercentModif;
        }
        set
        {
            jump.PercentModif = value;
        }
    }
    public float DefPercent
    {
        get
        {
            return def.PercentModif;
        }
        set
        {
            def.PercentModif = value;
        }
    }
    public float MDefPercent
    {
        get
        {
            return mdef.PercentModif;
        }
        set
        {
            mdef.PercentModif = value;
        }
    }
    public float MStrPercent
    {
        get
        {
            return mstr.PercentModif;
        }
        set
        {
            mstr.PercentModif = value;
        }
    }
    public float DeathLengthPercent
    {
        get
        {
            return deathLength.PercentModif;
        }
        set
        {
            deathLength.PercentModif = value;
        }
    }

    public string ClassName
    {
        get
        {
            return className;
        }

        set
        {
            className = value;
        }
    }

    public ObjectOnBloc[] GetObjectsOnBlockUnder()
    {
        return Grid.instance.GridMatrix[GetPosition().x, GetPosition().y - 1, GetPosition().z]
                .transform.Find("Inventory").GetComponentsInChildren<ObjectOnBloc>();
    }


    public void ChangeSpawnCharacter() //method called by the ui buttons during spawn
    {
        GameManager.instance.CharacterToSpawn = this;
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

    // Use this for initialization //TO KEEP AS IS
    private void Awake()
    {
        if(Grid.instance.UseAwakeLiving)
        {

            shouldBatch = false;
            this.className = "default";
            this.Walkable = false;
            this.Movable = true;
            this.Destroyable = true;
            this.TraversableChar = TraversableType.ALLIESTHROUGH;
            this.TraversableBullet = TraversableType.NOTHROUGH;
            this.GravityType = GravityType.SIMPLE_GRAVITY;

            this.Crushable = CrushType.CRUSHDAMAGE;
            this.OnDestroyEffects = new List<Effect>();
            this.HitablePoints = new List<HitablePoint>
            {
                new HitablePoint(new Vector3(0, 0.5f, 0), 1)
            };
            this.OnStartTurn = new List<Effect>();
            this.OnEndTurn = new List<Effect>();
            this.MaxHP = 100;
            this.CurrentHP = 100;
            this.MaxPM = 5;
            this.CurrentPM = 5;
            this.PaMax = 1;
            this.CurrentPA = 1;
            this.Jump = 1;
            this.Force = 100;
            this.Speed = 100;
            this.Def = 100;
            this.Mdef = 100;
            this.Mstr = 100;
            this.SpeedStack = 1 / Speed;
            this.Dexterity = 100;
            this.Skills = new List<Skill>();
            this.Weapons = new List<GameObject>();
            this.IsDead = false;
            this.CounterDeaths = 0;
            this.TurnsRemaingingCemetery = 0;
            this.ShootPosition = new Vector3(0, 0.5f, 0);
            this.AreaOfMouvement = new List<NodePath>();

            this.characterSprite = Resources.Load<Sprite>("UI_Images/Characters/" + className);
            this.AreaOfMouvement = new List<NodePath>();
            targetArea = new List<Placeable>();

            targetableUnits = new List<LivingPlaceable>();
            this.OnDestroyEffects = new List<Effect>();
            this.HitablePoints = new List<HitablePoint>();
            this.OnStartTurn = new List<Effect>();
            this.OnEndTurn = new List<Effect>();
            this.AttachedEffects = new List<Effect>();

            List<Effect> ListEffects = new List<Effect>();
            List<Effect> ListEffects2 = new List<Effect>();
            List<Effect> ListEffects3 = new List<Effect>();
            List<Effect> ListEffects4 = new List<Effect>();
            List<Effect> ListEffects5 = new List<Effect>();
            List<Effect> ListEffects6 = new List<Effect>();
            List<Effect> ListEffects7 = new List<Effect>();

            //Knight
            /*
            ListEffects.Add(new Push(null, this, 2, 500));
            ListEffects2.Add(new CreateBlock(Grid.instance.prefabsList[0], new Vector3Int(0, 1, 0)));
            ListEffects3.Add(new DestroyBloc());
            ListEffects4.Add(new DamageCalculated(40, DamageCalculated.DamageScale.STR));
            ListEffects5.Add(new Damage(10, 3));
            ListEffects6.Add(new ParameterChangeV2<LivingPlaceable, float>(-1, 0));
            ListEffects6.Add(new ParameterChangeV2<LivingPlaceable, float>(0, 0, 2, true, false));
            ListEffects7.Add(new DamageCalculated(30, DamageCalculated.DamageScale.STR));

            Skill skill1 = new Skill(1, 1, ListEffects, SkillType.BLOCK, "Basic_push", 0, 2, SkillEffect.MOVE, SkillArea.CROSS);
            Skill skill2 = new Skill(1, 1, ListEffects2, SkillType.BLOCK, "Basic_creation", 0, 4, SkillEffect.CREATE, SkillArea.TOPBLOCK);
            Skill skill3 = new Skill(1, 1, ListEffects3, SkillType.BLOCK, "Basic_destruction", 0, 3, SkillEffect.DESTROY);
            Skill skill4 = new Skill(1, 1, ListEffects4, SkillType.LIVING, "Basic_attack", 0, 2, SkillEffect.SWORDRANGE);
            Skill skill5 = new Skill(1, 2, ListEffects5, SkillType.LIVING, "Bleeding", 0, 1);
            Skill skill6 = new Skill(1, 3, ListEffects6, SkillType.LIVING, "debuffPm", 0, 2);
            Skill skill7 = new Skill(1, 2, ListEffects7, SkillType.SELF, "Spinning", 0, 2, SkillEffect.SPINNING, SkillArea.SURROUNDINGLIVING);

            Skills.Add(skill1);
            Skills.Add(skill2);
            Skills.Add(skill3);
            Skills.Add(skill4);
            Skills.Add(skill5);
            Skills.Add(skill6);
            Skills.Add(skill7);
            */
            //Ranger
            /*
            ListEffects.Add(new Push(null, this, 2, 500));
            ListEffects2.Add(new CreateBlock(Grid.instance.prefabsList[0], new Vector3Int(0, 1, 0)));
            ListEffects3.Add(new DestroyBloc());
            ListEffects4.Add(new DamageCalculated(30, DamageCalculated.DamageScale.STR));
            ListEffects5.Add(new MoveEffect(this, this, new Vector3Int(0, 2, 0), false));
            ListEffects5.Add(new CreateBlockRelativeEffect(Grid.instance.prefabsList[0], new Vector3Int(0, 1, 0), new Vector3Int(0, -3, 0)));
            ListEffects5.Add(new CreateBlockRelativeEffect(Grid.instance.prefabsList[0], new Vector3Int(0, 1, 0), new Vector3Int(0, -2, 0)));
            ListEffects6.Add(new PiercingDamageEffect(60, DamageCalculated.DamageScale.DEXT));
            ListEffects7.Add(new CreateZipLine(11, new Vector3Int(0, 1, 0)));

            Skill skill1 = new Skill(0, 1, ListEffects, SkillType.BLOCK, "Basic_push", 0, 2, SkillEffect.MOVE, SkillArea.CROSS);
            Skill skill2 = new Skill(0, 1, ListEffects2, SkillType.BLOCK, "Basic_creation", 0, 4, SkillEffect.CREATE, SkillArea.TOPBLOCK);
            Skill skill3 = new Skill(0, 1, ListEffects3, SkillType.BLOCK, "Basic_destruction", 0, 3, SkillEffect.DESTROY);
            Skill skill4 = new Skill(0, 1, ListEffects4, SkillType.LIVING, "Basic_attack", 0, 2, SkillEffect.SWORDRANGE);
            Skill skill5 = new Skill(0, 1, ListEffects5, SkillType.SELF, "HigherGround", 0, 1);
            Skill skill6 = new Skill(0, 1, ListEffects6, SkillType.LIVING, "Piercing_arrow", 3, 10, SkillEffect.NONE, SkillArea.THROUGHBLOCKS);
            Skill skill7 = new Skill(0, 1, ListEffects7, SkillType.BLOCK, "Range_buff", 0, 6);

            Skills.Add(skill1);
            Skills.Add(skill2);
            Skills.Add(skill3);
            Skills.Add(skill4);
            Skills.Add(skill5);
            Skills.Add(skill6);
            Skills.Add(skill7);
            */

            //Mage

            ListEffects.Add(new Push(null, this, 2, 500));
            ListEffects2.Add(new CreateBlock(Grid.instance.prefabsList[0], new Vector3Int(0, 1, 0)));
            ListEffects3.Add(new DestroyBloc());
            ListEffects4.Add(new DamageCalculated(40, DamageCalculated.DamageScale.STR));
            ListEffects5.Add(new DestroyBloc(1));
            ListEffects6.Add(new CreateBlock(Grid.instance.prefabsList[0], new Vector3Int(0, 1, 0), 1));
            ListEffects7.Add(new DamageCalculated(30, DamageCalculated.DamageScale.MAG));
            ListEffects7.Add(new DestroyBloc());

            Skill skill1 = new Skill(1, 1, ListEffects, SkillType.BLOCK, "Basic_push", 0, 2, SkillEffect.MOVE, SkillArea.CROSS);
            Skill skill2 = new Skill(1, 1, ListEffects2, SkillType.BLOCK, "Basic_creation", 0, 4, SkillEffect.CREATE, SkillArea.TOPBLOCK);
            Skill skill3 = new Skill(1, 1, ListEffects3, SkillType.BLOCK, "Basic_destruction", 0, 3, SkillEffect.DESTROY);
            Skill skill4 = new Skill(1, 1, ListEffects4, SkillType.LIVING, "Basic_attack", 0, 2, SkillEffect.SWORDRANGE);
            Skill skill5 = new Skill(0, 1, ListEffects5, SkillType.AREA, "Fissure", 0, 4, SkillEffect.DESTROY, SkillArea.LINE, 1);
            Skill skill6 = new Skill(0, 1, ListEffects6, SkillType.AREA, "Wall", 0, 3, SkillEffect.CREATE, SkillArea.LINE, 1);
            Skill skill7 = new Skill(0, 1, ListEffects7, SkillType.AREA, "ExplosiveFireball", 2, 6, SkillEffect.NONE, SkillArea.MIXEDAREA, 1);

            Skills.Add(skill1);
            Skills.Add(skill2);
            Skills.Add(skill3);
            Skills.Add(skill4);
            Skills.Add(skill5);
            Skills.Add(skill6);
            Skills.Add(skill7);
        }
    }

    public void Init(int classNumber)
    {
        if (!Grid.instance.UseAwakeLiving)
        {
            base.Init();
            shouldBatch = false;
            this.className = "default";
            this.Walkable = false;
            this.Movable = true;
            this.Destroyable = true;
            this.TraversableChar = TraversableType.ALLIESTHROUGH;
            this.TraversableBullet = TraversableType.NOTHROUGH;
            this.GravityType = GravityType.SIMPLE_GRAVITY;
            this.Crushable = CrushType.CRUSHDAMAGE;
            this.OnDestroyEffects = new List<Effect>();
            this.HitablePoints = new List<HitablePoint>
            {
                new HitablePoint(new Vector3(0, 0.5f, 0), 1)
            };
            this.OnStartTurn = new List<Effect>();
            this.OnEndTurn = new List<Effect>();
            this.AreaOfMouvement = new List<NodePath>();
            targetArea = new List<Placeable>();

            targetableUnits = new List<LivingPlaceable>();
            this.OnDestroyEffects = new List<Effect>();
            this.HitablePoints = new List<HitablePoint>();
            this.OnStartTurn = new List<Effect>();
            this.OnEndTurn = new List<Effect>();
            this.AttachedEffects = new List<Effect>();

            this.Skills = new List<Skill>();
            this.Weapons = new List<GameObject>();
            this.IsDead = false;
            this.CounterDeaths = 0;
            this.TurnsRemaingingCemetery = 0;
            this.ShootPosition = new Vector3(0, 0.5f, 0);
            this.AreaOfMouvement = new List<NodePath>();

            rend = GetComponentInChildren<Renderer>();
            originalShader = Shader.Find("Standard");
            outlineShader = Shader.Find("Outlined/Silhouetted Diffuse");
            rend.material.shader = outlineShader;
            rend.material.SetColor("_Color", Color.white - new Color(0, 0, 0, 0.175f));
            rend.material.SetFloat("_Outline", 0.02f);
            rend.material.shader = originalShader;
            ClassName = GameManager.instance.PossibleCharacters[classNumber].className;
            LoadFromjson(ClassName + ".json");
            circleTeam.color = Player.color;
            targetableUnits = new List<LivingPlaceable>();
        }
    }
    

    /// <summary>
    /// To call at the beginning of the turn of the character
    /// </summary>
    public void BeginningMyTurn()
    {
        circleTeam.color = Color.green;
    }

    /// <summary>
    /// To call at the ending of the turn of the character
    /// </summary>
    public void EndingMyTurn()
    {
        circleTeam.color = Player.color;
    }
    
    /// <summary>
    /// method to call to destroy the object 
    /// </summary>
    /// 
    override
    public void Destroy()
    {
        // warning : the SetActive value of the gameobject attached to livingplaceable is dealed with directly in AnimationHandler to avoid bug of respawn in special case
        this.IsDead = true;
        Grid.instance.GridMatrix[GetPosition().x, GetPosition().y, GetPosition().z] = null;
        CounterDeaths++;
        if (this.Destroyable)
        {
            Grid.instance.GridMatrix[GetPosition().x, GetPosition().y, GetPosition().z] = null;
            foreach (Effect effect in this.OnDestroyEffects)
            {
                EffectManager.instance.DirectAttack(effect);
            }
            foreach (Transform obj in transform.Find("Inventory"))
            {
                obj.GetComponent<ObjectOnBloc>().Destroy();
            }
            foreach (Effect effect in AttachedEffects)
            {
                effect.TurnActiveEffect = 1;
                EffectManager.instance.DirectAttack(effect);
            }
            AttachedEffects.Clear();
            if (GameManager.instance.playingPlaceable == this)
            {
                if(MoveCoroutine!=null)
                {
                    StopCoroutine(MoveCoroutine);
                }
                GameManager.instance.EndOFTurn();
            }
            this.IsDead = true;
            this.gameObject.SetActive(false);
            CounterDeaths++;
        }
        
    }

    /* public void HighlightForSpawn()
     {
         rend.material.shader = outlineShader;
         rend.material.SetColor("_OutlineColor", Color.green);
     }*/
    private void ActivateOutline(Color color)
    {
        rend.material.shader = outlineShader;
        rend.material.SetColor("_OutlineColor", color);
    }

    private void DesactivateOutline()
    {
        rend.material.shader = originalShader;
    }

    public override void Highlight()
    {
        ActivateOutline(Color.white);
    }

    public override void UnHighlight()
    {
        if (isTarget)
        {
            ActivateOutline(previousColor);
        }
        else
        {
            DesactivateOutline();
        }
    }

    public void UnHighlightTarget()
    {
        isTarget = false;
        DesactivateOutline();
    }

    public void HighlightForSpawn()
    {
        ActivateOutline(Color.green);
        previousColor = Color.green;
        isTarget = true;
    }

    public void HighlightForSkill()
    {
        ActivateOutline(Color.red);
        previousColor = Color.red;
        isTarget = true;
    }

    public void ChangeMaterialAreaOfMovementBatch(Material pathfinding)
    {

        foreach (NodePath node in AreaOfMouvement)
        {
            if (Grid.instance.GridMatrix[node.x, node.y, node.z] != null && Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial == null) //if we haven't seen this one before
            {
                // Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().enabled = true;
                Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial = Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().material;
                Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().material = pathfinding;
            }
        }
        GameManager.instance.ResetAllBatches();

    }
    public void ChangeMaterialAreaOfMovement(Material pathfinding)
    {
        Player.GetComponentInChildren<RaycastSelector>().layerMask = LayerMask.GetMask("Placeable");
        float heightSize = 0.2f;
        foreach (NodePath node in AreaOfMouvement)
        {

            GameObject quadUp = Grid.instance.GridMatrix[node.x, node.y, node.z].transform.Find("Quads").Find("QuadUp").gameObject;
            GameObject quadRight = Grid.instance.GridMatrix[node.x, node.y, node.z].transform.Find("Quads").Find("QuadRight").gameObject;
            GameObject quadLeft = Grid.instance.GridMatrix[node.x, node.y, node.z].transform.Find("Quads").Find("QuadLeft").gameObject;
            GameObject quadFront = Grid.instance.GridMatrix[node.x, node.y, node.z].transform.Find("Quads").Find("QuadFront").gameObject;
            GameObject quadBack = Grid.instance.GridMatrix[node.x, node.y, node.z].transform.Find("Quads").Find("QuadBack").gameObject;


            quadUp.SetActive(true);

            quadRight.SetActive(true);
            quadRight.transform.localScale = new Vector3(quadRight.transform.localScale.x, heightSize + 0.01f, 1);
            quadRight.transform.localPosition = new Vector3(quadRight.transform.localPosition.x, 0.5f - heightSize / 2 + 0.01f, quadRight.transform.localPosition.z);

            quadLeft.SetActive(true);
            quadLeft.transform.localScale = new Vector3(quadLeft.transform.localScale.x, heightSize + 0.01f, 1);
            quadLeft.transform.localPosition = new Vector3(quadLeft.transform.localPosition.x, 0.5f - heightSize / 2 + 0.01f, quadLeft.transform.localPosition.z);

            quadFront.SetActive(true);
            quadFront.transform.localScale = new Vector3(quadFront.transform.localScale.x, heightSize + 0.01f, 1);
            quadFront.transform.localPosition = new Vector3(quadFront.transform.localPosition.x, 0.5f - heightSize / 2 + 0.01f, quadFront.transform.localPosition.z);

            quadBack.SetActive(true);
            quadBack.transform.localScale = new Vector3(quadBack.transform.localScale.x, heightSize + 0.01f, 1);
            quadBack.transform.localPosition = new Vector3(quadBack.transform.localPosition.x, 0.5f - heightSize / 2 + 0.01f, quadBack.transform.localPosition.z);


            // Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().enabled = true;
            // Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial = Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().material;
            //Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().material = pathfinding;
        }
    }


    public void ResetAreaOfMovementBatch()
    {
        foreach (NodePath node in AreaOfMouvement)
        {

            if (Grid.instance.GridMatrix[node.x, node.y, node.z] != null && Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial != null)//if we haven't already reset this one
            {
                // Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().enabled = true;
                Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().material =
                        Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial;
                Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial = null;
            }
        }
        AreaOfMouvement.Clear();
    }

    public void ResetAreaOfMovement()
    {
        foreach (NodePath node in AreaOfMouvement)
        {
            if (Grid.instance.GridMatrix[node.x, node.y, node.z] != null && Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial == null) //if we haven't seen this one before
            {
                GameObject quadUp = Grid.instance.GridMatrix[node.x, node.y, node.z].transform.Find("Quads").Find("QuadUp").gameObject;
                GameObject quadRight = Grid.instance.GridMatrix[node.x, node.y, node.z].transform.Find("Quads").Find("QuadRight").gameObject;
                GameObject quadLeft = Grid.instance.GridMatrix[node.x, node.y, node.z].transform.Find("Quads").Find("QuadLeft").gameObject;
                GameObject quadFront = Grid.instance.GridMatrix[node.x, node.y, node.z].transform.Find("Quads").Find("QuadFront").gameObject;
                GameObject quadBack = Grid.instance.GridMatrix[node.x, node.y, node.z].transform.Find("Quads").Find("QuadBack").gameObject;

                quadUp.SetActive(false);
                quadRight.SetActive(false);
                quadLeft.SetActive(false);

                quadFront.SetActive(false);

                quadBack.SetActive(false);


            }

        }
        AreaOfMouvement.Clear();
    }

    public void ResetHighlightSkill()
    {
        TargetableUnits = new List<LivingPlaceable>();
    }

    public void ChangeMaterialAreaOfTarget(Material materialTarget)
    {
        foreach (Placeable placeable in TargetArea)
        {
            if (Grid.instance.GridMatrix[placeable.GetPosition().x, placeable.GetPosition().y, placeable.GetPosition().z].oldMaterial == null && !placeable.IsLiving()) //if we haven't seen this one before
            {
                // Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().enabled = true;
                
                Grid.instance.GridMatrix[placeable.GetPosition().x, placeable.GetPosition().y, placeable.GetPosition().z].oldMaterial =
                    Grid.instance.GridMatrix[placeable.GetPosition().x, placeable.GetPosition().y, placeable.GetPosition().z].GetComponent<MeshRenderer>().material;
                Grid.instance.GridMatrix[placeable.GetPosition().x, placeable.GetPosition().y, placeable.GetPosition().z].GetComponent<MeshRenderer>().material = materialTarget;

            }
        }
        GameManager.instance.ResetAllBatches();
    }

    /// <summary>
    /// Reset the targets, both cube and livingPlaceable
    /// </summary>
    public void ResetTargets()
    {
        foreach (Placeable plac in targetArea)
        {
            if (Grid.instance.GridMatrix[plac.GetPosition().x, plac.GetPosition().y, plac.GetPosition().z].oldMaterial != null)//if we haven't already reset this one
            {

                Grid.instance.GridMatrix[plac.GetPosition().x, plac.GetPosition().y, plac.GetPosition().z].GetComponent<MeshRenderer>().material =
                    Grid.instance.GridMatrix[plac.GetPosition().x, plac.GetPosition().y, plac.GetPosition().z].oldMaterial;
                Grid.instance.GridMatrix[plac.GetPosition().x, plac.GetPosition().y, plac.GetPosition().z].oldMaterial = null;
            }
        }
        if (targetArea.Count > 0)
        {
            GameManager.instance.RefreshBatch(targetArea[0]);
        }
        targetArea.Clear();

        GameManager.instance.ResetAllBatches();
        TargetableUnits = null;

    }

    public void Save(string path = "Living.json")
    {
        positionSave = GetPosition();
        Stats stats = new Stats();
        stats.FillThis(this);
        string text = JsonUtility.ToJson(stats);
        foreach (Skill skill in Skills)
        {
            text += skill.Save();
        }
        File.WriteAllText(path, text);
    }

    public static Stream GenerateStreamFromString(string s)
    {
        MemoryStream stream = new MemoryStream();
        StreamWriter writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    public void LoadFromString(string file)
    {
        FillLiving(new StreamReader(GenerateStreamFromString(file)));
    }
    public void LoadFromjson(string path)
    {
        StreamReader reader = new StreamReader(path);
        FillLiving(reader);
    }
    public void FillLiving(StreamReader reader)
    {

        string line;
        //Read the text from directly from the test.txt file

        System.Type[] types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
        System.Type[] possible = (from System.Type type in types where type.IsSubclassOf(typeof(Effect)) && !type.IsAbstract select type).ToArray();

        if ((line = reader.ReadLine()) == null)
        {
            Debug.Log("Empty file while reading living form file!");
            return;
        }
        Stats newLivingStats = JsonUtility.FromJson<Stats>(line);
        newLivingStats.FillLiving(this);
        this.characterSprite = Resources.Load<Sprite>("UI_Images/Characters/" + ClassName);
        bool isNewSkill = true;
        Skill newSkill = null;

        while ((line = reader.ReadLine()) != null)
        {
            if (isNewSkill)
            {

                newSkill = JsonUtility.FromJson<Skill>(line);
                newSkill.AbilitySprite = Resources.Load<Sprite>("UI_Images/Abilities/" + newSkill.SkillName);
                newSkill.effects = new List<Effect>();
                isNewSkill = false;

            }
            else
            {
                string typename = line.Substring(0, line.IndexOf("{"));

                if (typename.StartsWith("ParameterChangeV2"))
                {
                    string T = line.Substring(line.IndexOf("[") + 1, line.IndexOf(",") - line.IndexOf("[") - 1);
                    string TProperty = line.Substring(line.IndexOf(",") + 1, line.IndexOf("]") - line.IndexOf(",") - 1);
                    string a = line.Substring(line.IndexOf("{"));

                    if (line[line.Length - 1] == ';')
                    {
                        a = a.Remove(a.Length - 1);
                        isNewSkill = true;
                    }
                    Debug.Log(a);
                    Effect eff = null;
                    if (T == "LivingPlaceable")
                    {
                        if (TProperty == "System.Single")
                        {
                            eff = (Effect)JsonUtility.FromJson<ParameterChangeV2<LivingPlaceable, float>>(a);
                        }
                    }
                    if (eff == null)
                    {
                        Debug.LogError("no parameterChange with those types");
                    }
                    eff.Initialize(this);
                    newSkill.effects.Add(eff);
                    if (isNewSkill)
                    {
                        if (skills == null)
                        {
                            skills = new List<Skill>();
                        }
                        skills.Add(newSkill);
                    }
                }
                else
                {
                    foreach (Type type in possible)
                    {
                        if (type.ToString() == typename)
                        {
                            // MethodInfo method = typeof(JsonUtility).GetMethod("FromJson");
                            //MethodInfo generic = method.MakeGenericMethod(type);
                            //object[] objectArray = new[] { line};
                            string a = line.Substring(line.IndexOf("{"));

                            if (line[line.Length - 1] == ';')
                            {
                                a = a.Remove(a.Length - 1);
                                isNewSkill = true;
                            }
                            //Debug.Log(a);
                            Effect eff = (Effect)JsonUtility.FromJson(a, type);
                            eff.Initialize(this);
                            newSkill.effects.Add(eff);
                            if (isNewSkill)
                            {
                                if (skills == null)
                                {
                                    skills = new List<Skill>();
                                }
                                skills.Add(newSkill);
                            }

                        }
                    }
                }


            }

        }
        reader.Close();

    }
}
