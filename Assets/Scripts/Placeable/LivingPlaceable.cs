﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class LivingPlaceable : Placeable, IHurtable
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
    private Attribute speed;
    [SerializeField]
    private Attribute speedStack;
    [SerializeField]
    private Attribute jump;
    [SerializeField]
    private Attribute def;
    [SerializeField]
    private Attribute mdef;
    [SerializeField]
    private Attribute deathLength;
    [SerializeField]
    private List<Skill> skills;
    [SerializeField]
    private SpriteRenderer circleTeam;
    [SerializeField]
    private SkillTypes[] ChosenSkills; 
    [SerializeField]
    private bool isDead;
    private int counterDeaths;
    private int turnsRemainingCemetery;
    private Vector3 shootPosition;
    private int capacityInUse;
    private List<NodePath> areaOfMouvement;
    private List<StandardCube> range;
    private List<Placeable> targetArea;
    private List<Placeable> targetableHurtable = new List<Placeable>();

    //List of objects connected to the character (totems / traps ...)
    private List<NetIdeable> linkedObjects;
    private NetIdeable activeLink;

    public Canvas flyingInfo;
    public Sprite characterSprite;

    // Variable used for the highligh
    private bool isTarget = false;
    private Renderer rend;
    //Shaders (used for the highlight)
    public Shader originalShader;
    public Shader outlineShader;
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
            flyingInfo.transform.Find("HPBar").gameObject.GetComponent<Image>().fillAmount = value / MaxHP;
            flyingInfo.transform.Find("HPPreview").gameObject.GetComponent<Image>().fillAmount = value / MaxHP;
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
            if (GameManager.instance && GameManager.instance.player1 != null && Player != null && GameManager.instance.player1 == Player.gameObject)
            {
                PlayerPosesser = "player1";
            }
            if (GameManager.instance && GameManager.instance.player2 != null && Player != null && GameManager.instance.player2 == Player.gameObject)
            {
                PlayerPosesser = "player2";
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
            if (areaOfMouvement != null)
            {
                foreach (NodePath node in areaOfMouvement)
                {
                    StandardCube cube = Grid.instance.GetPlaceableFromVector(node.GetVector3()) as StandardCube;
                    if (cube)
                    {
                        cube.UnhighlightForMovement();
                    }
                    else
                    {
                        throw new System.ArgumentException("An element which was not standardCube in AreaOfMouvement");
                    }
                }
            }
            if (value != null)
            {
            foreach (NodePath node in value)
            {
                StandardCube cube = Grid.instance.GetPlaceableFromVector(node.GetVector3()) as StandardCube;
                if (cube)
                {
                    cube.HighlightForMovement();
                }
                else
                {
                    throw new System.ArgumentException("An element which was not standardCube in AreaOfMouvement");
                }
            }
            }
            areaOfMouvement = value;
        }
    }


    public List<Placeable> TargetableHurtable
    {
        get
        {
            return targetableHurtable;
        }

        set
        {
            if (targetableHurtable != null)
            {
                foreach (Placeable placeable in targetableHurtable)
                {
                    if (placeable.IsLiving())
                    {
                        ((LivingPlaceable)placeable).UnHighlightTarget();
                    }
                    else
                    {
                        ((Totem)placeable).UnHighlightTarget();
                    }
                }
            }

            if (value != null)
            {
                foreach (Placeable living in value)
                {
                    living.HighlightForAttacks();
                }
            }
            targetableHurtable = value;
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
            if (targetArea != null)
            {
                foreach (Placeable placeable in targetArea)
                {
                    if (!placeable.IsLiving())
                        ((StandardCube)placeable).UnhighlightForSkill();
                }
            }
            
            if (value != null)
            {
                foreach (Placeable placeable in value)
                {
                    if (!placeable.IsLiving())
                    {
                        ((StandardCube)placeable).HighlightForSkill();
                    }
                        
                }
            }
            targetArea = value;
        }
    }

    public List<StandardCube> Range
    {
        get
        {
            return range;
        }

        set
        {
            if (range != null)
            {
                foreach (StandardCube cube in range)
                {
                    cube.outline.SetActive(false);
                    cube.GetComponent<MeshRenderer>().material.SetFloat("_OutlineWidth", 0.02f);
                }
            }

            if (value != null)
            {
                foreach (StandardCube cube in value)
                {
                    cube.outline.SetActive(true);
                    cube.GetComponent<MeshRenderer>().material.SetFloat("_OutlineWidth", 0f);
                }
            }
            range = value;
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

    public string PlayerPosesser
    {
        get
        {
            return playerPosesser;
        }

        set
        {
            playerPosesser = value;
        }
    }

    public List<NetIdeable> LinkedObjects { get => linkedObjects; set => linkedObjects = value; }

    public NetIdeable ActiveLink
    { get => activeLink;

        set
        {
            activeLink = value;
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


    public override bool IsLiving()
    {
        return true;
    }

    // Use this for initialization //TO KEEP AS IS
    protected override void Awake()
    {
        if (Grid.instance.UseAwakeLiving)
        {

            shouldBatch = false;
            this.className = "default";
            this.walkable = false;
            this.movable = true;
            this.traversableType = TraversableType.ALLIESTHROUGH;
            this.traversableBullet = TraversableType.NOTHROUGH;
            this.gravityType = GravityType.SIMPLE_GRAVITY;
            this.crushable = CrushType.CRUSHDAMAGE;
            this.MaxHP = 100;
            this.CurrentHP = 100;
            this.MaxPM = 5;
            this.CurrentPM = 5;
            this.PaMax = 1;
            this.CurrentPA = 1;
            this.Jump = 1;
            this.Speed = 100;
            this.Def = 100;
            this.Mdef = 100;
            this.SpeedStack = 1 / Speed;
            this.Skills = new List<Skill>();
            this.IsDead = false;
            this.CounterDeaths = 0;
            this.TurnsRemaingingCemetery = 0;
            this.ShootPosition = new Vector3(0, 0.5f, 0);
            this.AreaOfMouvement = new List<NodePath>();

            this.characterSprite = Resources.Load<Sprite>("UI_Images/Characters/" + className);
            this.AreaOfMouvement = new List<NodePath>();
            targetArea = new List<Placeable>();
            this.linkedObjects = new List<NetIdeable>();

            ParameterChangeV2<LivingPlaceable, float>.MethodsForEffects.Add(o => o.MaxPMFlat);
            ParameterChangeV2<LivingPlaceable, float>.MethodsForEffects.Add(o => o.CurrentHP);
            ParameterChangeV2<LivingPlaceable, float>.MethodsForEffects.Add(o => o.JumpFlat);

            targetableHurtable = new List<Placeable>();
            this.AttachedEffects = new List<Effect>();

            List<Effect> ListEffects = new List<Effect>();
            List<Effect> ListEffects2 = new List<Effect>();
            List<Effect> ListEffects3 = new List<Effect>();
            List<Effect> ListEffects4 = new List<Effect>();
            List<Effect> ListEffects5 = new List<Effect>();
            List<Effect> ListEffects6 = new List<Effect>();
            List<Effect> ListEffects7 = new List<Effect>();
            List<Effect> ListEffects8 = new List<Effect>();

            ListEffects.Add(new Push(null, this, 2, 500));
            ListEffects2.Add(new CreateBlock(Grid.instance.prefabsList[0], new Vector3Int(0, 1, 0)));
            ListEffects3.Add(new DestroyBloc());
            /*
            Skill skill1 = new Skill(1, 0, ListEffects, TargetType.BLOCK, "Push", 0, 2, SkillEffect.MOVE, SkillArea.NONE);
            Skill skill2 = new Skill(1, 0, ListEffects2, TargetType.BLOCK, "Creation", 0, 4, SkillEffect.CREATE, SkillArea.TOPBLOCK);
            Skill skill3 = new Skill(1, 0, ListEffects3, TargetType.BLOCK, "Destruction", 0, 4, SkillEffect.DESTROY);

            //Knight
            /*
            ListEffects4.Add(new DamageCalculated(40, DamageCalculated.DamageScale.STR));
            ListEffects5.Add(new Damage(10, 3));
            ListEffects6.Add(new ParameterChangeV2<LivingPlaceable, float>(-1, 0));
            ListEffects6.Add(new ParameterChangeV2<LivingPlaceable, float>(0, 0, 2, true, ActivationType.BEGINNING_OF_TURN));
            ListEffects7.Add(new DamageCalculated(30, DamageCalculated.DamageScale.STR));

            Skill skill4 = new Skill(1, 1, ListEffects4, TargetType.HURTABLE, "Basic_attack", 0, 2, SkillEffect.SWORDRANGE);
            Skill skill5 = new Skill(1, 2, ListEffects5, TargetType.HURTABLE, "Bleeding", 0, 2, SkillEffect.SWORDRANGE);
            Skill skill6 = new Skill(1, 3, ListEffects6, TargetType.HURTABLE, "debuffPm", 0, 2, SkillEffect.SWORDRANGE);
            Skill skill7 = new Skill(1, 2, ListEffects7, TargetType.HURTABLE, "Spinning", 0, 2, SkillEffect.SPINNING, SkillArea.SURROUNDINGLIVING);

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
            ListEffects8.Add(new ParameterChangeV2<LivingPlaceable, float>(-1, 2));
            ListEffects8.Add(new ParameterChangeV2<LivingPlaceable, float>(0, 2, 2, true, ActivationType.BEGINNING_OF_TURN));
            
            Skill skill4 = new Skill(0, 1, ListEffects4, TargetType.HURTABLE, "Basic_attack", 2, 4);
            Skill skill5 = new Skill(0, 1, ListEffects5, TargetType.ALREADYTARGETED, "HigherGround", 0, 1);
            Skill skill6 = new Skill(0, 1, ListEffects6, TargetType.HURTABLE, "Piercing_arrow", 3, 10, SkillEffect.NONE, SkillArea.THROUGHBLOCKS);
            Skill skill7 = new Skill(0, 1, ListEffects7, TargetType.BLOCK, "Zipline", 1, 5, SkillEffect.NONE, SkillArea.TOPBLOCK);
            Skill skill8 = new Skill(0, 1, ListEffects8, TargetType.ALREADYTARGETED, "CLimbing SKills", 1, 5);

            Skills.Add(skill1);
            Skills.Add(skill2);
            Skills.Add(skill3);
            Skills.Add(skill4);
            Skills.Add(skill5);
            Skills.Add(skill6);
            Skills.Add(skill7);
            Skills.Add(skill8);

            //Mage
            /*
            ListEffects.Add(new Push(null, this, 2, 500));
            ListEffects2.Add(new CreateBlock(Grid.instance.prefabsList[0], new Vector3Int(0, 1, 0)));
            ListEffects3.Add(new DestroyBloc());
            ListEffects4.Add(new DamageCalculated(40, DamageCalculated.DamageScale.STR));
            ListEffects5.Add(new DestroyBloc(1));
            ListEffects6.Add(new CreateBlock(Grid.instance.prefabsList[0], new Vector3Int(0, 1, 0), 1));
            ListEffects7.Add(new DamageCalculated(30, DamageCalculated.DamageScale.MAG));
            ListEffects7.Add(new DestroyBloc());
            
            Skill skill4 = new Skill(1, 1, ListEffects4, TargetType.HURTABLE, "Basic_attack", 0, 2, SkillEffect.SWORDRANGE);
            Skill skill5 = new Skill(2, 1, ListEffects5, TargetType.BLOCK, "Fissure", 0, 4, SkillEffect.DESTROY, SkillArea.LINE, 1);
            Skill skill6 = new Skill(2, 1, ListEffects6, TargetType.BLOCK, "Wall", 0, 3, SkillEffect.CREATE, SkillArea.LINE, 1);
            Skill skill7 = new Skill(2, 1, ListEffects7, TargetType.AREA, "ExplosiveFireball", 2, 6, SkillEffect.DESTROY, SkillArea.MIXEDAREA, 1);

            Skills.Add(skill1);
            Skills.Add(skill2);
            Skills.Add(skill3);
            Skills.Add(skill4);
            Skills.Add(skill5);
            Skills.Add(skill6);
            Skills.Add(skill7);
            */
        }
    }
    public void InitNoClass()
    {
        base.Init();
        shouldBatch = false;
        this.className = "default";
        this.walkable = false;
        this.movable = true;
        this.traversableType = TraversableType.ALLIESTHROUGH;
        this.traversableBullet = TraversableType.NOTHROUGH;
        this.gravityType = GravityType.SIMPLE_GRAVITY;
        this.crushable = CrushType.CRUSHDAMAGE;
        this.AreaOfMouvement = new List<NodePath>();
        this.targetArea = new List<Placeable>();
        this.targetableHurtable = new List<Placeable>();
        this.AttachedEffects = new List<Effect>();
        this.Skills = new List<Skill>();
        this.IsDead = false;
        this.CounterDeaths = 0;
        this.TurnsRemaingingCemetery = 0;
        this.ShootPosition = new Vector3(0, 0.5f, 0);
        this.AreaOfMouvement = new List<NodePath>();
        this.linkedObjects = new List<NetIdeable>();
        
        
        rend = GetComponentsInChildren<Renderer>().ToArray()[1]; // The 1st renderer is the circle could not manage to find the good renderer correctly
        rend.material.shader = originalShader;
    }

    public void Init(int classNumber)
    {
        if (!Grid.instance.UseAwakeLiving)
        {
            InitNoClass();
            ClassName = GameManager.instance.PossibleCharacters[classNumber].className;
            LoadFromjson(Path.Combine(Application.streamingAssetsPath, ClassName + ".json"));
            circleTeam.color = Player.color;
        }
    }

    private void Update()
    {
        if (!Grid.instance.UseAwakeLiving && GameManager.instance.isClient)
        {
            flyingInfo.transform.LookAt(GameManager.instance.GetLocalPlayer().GetComponentInChildren<Camera>().gameObject.transform);
        }
    }

    public void ReceiveDamage(float damage)
    {
        CurrentHP -= damage;
        FloatingTextController.CreateFloatingText(damage.ToString(), transform, Color.red);
        if (CurrentHP <= 0)
        {
            Destroy();
            gameObject.SetActive(false);
        }
    }

    public void DamagePreview(int damage)
    {
        float realValue = flyingInfo.transform.Find("HPBar").gameObject.GetComponent<Image>().fillAmount;
        flyingInfo.transform.Find("HPBar").gameObject.GetComponent<Image>().fillAmount = realValue - damage / MaxHP;
    }

    public void ResetPreview()
    {
        flyingInfo.transform.Find("HPBar").gameObject.GetComponent<Image>().fillAmount = flyingInfo.transform.Find("HPPreview").gameObject.GetComponent<Image>().fillAmount;
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
        CurrentHP = 0;
        TurnsRemaingingCemetery = (int) DeathLength;
        Grid.instance.GridMatrix[GetPosition().x, GetPosition().y, GetPosition().z] = null;
        foreach (Transform obj in transform.Find("Inventory"))
        {
            obj.GetComponent<ObjectOnBloc>().Destroy();
        }
        if (AttachedEffects != null)
        {
            ResetStats();
            AttachedEffects.Clear();
        }
        if (GameManager.instance.PlayingPlaceable == this)
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            GameManager.instance.EndOFTurn();
        }

        this.IsDead = true;
        this.gameObject.SetActive(false);

    }

    public void ResetStats()
    {
        MaxHPFlat = 0;
        MaxPAFlat = 0;
        MaxPMFlat = 0;
        DeathLengthFlat = 0;
        DefFlat = 0;
        JumpFlat = 0;
        MDefFlat = 0;
        SpeedFlat = 0;
        SpeedStackFlat = 0;
        
        DefPercent = 0;
        MaxHPPercent = 0;
        MaxPAPercent = 0;
        MaxPMPercent = 0;
        MDefPercent = 0;
        SpeedPercent = 0;
        SpeedStackPercent = 0;
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

    public override void UnhighlightHovered()
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

    public override void HighlightForAttacks()
    {
        ActivateOutline(Color.red);
        previousColor = Color.red;
        isTarget = true;
    }
    public void HighlightAreaOfMovement()
    {
        Player.GetComponentInChildren<RaycastSelector>().layerMask = LayerMask.GetMask("Totems") | LayerMask.GetMask("Cube");
        Debug.Log("area of movement " + areaOfMouvement.Count);
    }

    public void ClearAreaOfMovement()
    {
        AreaOfMouvement = new List<NodePath>();
    }

    public void ResetHighlightSkill()
    {
        TargetableHurtable = new List<Placeable>();
    }

    /// <summary>
    /// Reset the targets, both cube and livingPlaceable
    /// </summary>
    public void ResetTargets()
    {
        TargetArea = null;
        TargetableHurtable = null;
        GameManager.instance.ResetAllBatches();
        Range = null;
    }

    /// <summary>
    /// Check if the okaceable is a target of that Character
    /// </summary>
    /// <param name="placeable"></param>
    /// <returns></returns>
    public bool IsPlaceableInTarget(Placeable placeable)
    {
        return targetArea != null && targetArea.Contains(placeable) || targetableHurtable != null && (null != placeable as IHurtable) && targetableHurtable.Contains(placeable);
    }

    public void SearchAndInstantianteSkills()
    {
        StreamReader reader = new StreamReader(Path.Combine(Application.streamingAssetsPath, "Skills.json"));
        string JSON = reader.ReadToEnd();
        foreach (SkillTypes types in ChosenSkills)
        {
            System.Type type;
            if (GameManager.instance.SkillDictionary.TryGetValue(types, out type))
            {
                Skills.Add((Skill)Activator.CreateInstance(type, JSON));
            }
        }
    }

    public void Save(string path)
    {
        //No joke keep it , it changes playerpossessser
        Player = Player;
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

        string fullText;
        //Read the text from directly from the test.txt file

        System.Type[] types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
        System.Type[] possible = (from System.Type type in types where type.IsSubclassOf(typeof(Effect)) && !type.IsAbstract select type).ToArray();

        if ((fullText = reader.ReadToEnd()) == null)
        {
            Debug.Log("Empty file while reading living form file!");
            return;
        }
        Stats newLivingStats = JsonUtility.FromJson<Stats>(fullText);
        newLivingStats.FillLiving(this);
        this.characterSprite = Resources.Load<Sprite>("UI_Images/Characters/" + ClassName);
        SearchAndInstantianteSkills();


        /*
        bool isNewSkill = true;
        Skill newSkill = null;
        while ((line = reader.ReadLine()) != null)
        {
            if (isNewSkill)
            {

                newSkill = JsonUtility.FromJson<Skill>(line);
                newSkill.AbilitySprite = Resources.Load<Sprite>("UI_Images/Abilities/" + newSkill.SkillName);
                newSkill.effects = new List<Effect>();
                newSkill.InitPattern();
                newSkill.InitPatternUse();
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
                            eff = JsonUtility.FromJson<ParameterChangeV2<LivingPlaceable, float>>(a);
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

        }*/
        reader.Close();
        //Actualize playerpossesser
        Player = Player;
    }
}