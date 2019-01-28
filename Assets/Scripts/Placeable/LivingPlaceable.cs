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
    private Vector3 positionSave;
    [SerializeField]
    private string playerPosesser;
    [SerializeField]
    private string className = "default";
    [SerializeField]
    private float maxHP;
    [SerializeField]
    private float currentHP;
    [SerializeField]
    private int pmMax;
    [SerializeField]
    private int currentPM;
    [SerializeField]
    private float currentPA;
    [SerializeField]
    private float paMax;
    [SerializeField]
    private int force = -99999;
    [SerializeField]
    private float speed;
    [SerializeField]
    private int dexterity = -77777;
    [SerializeField]
    private float speedStack;
    [SerializeField]
    private int jump;
    [SerializeField]
    private int def = -88888;
    [SerializeField]
    private int mdef = -66666;
    [SerializeField]
    private int mstr = -555555;
    private float deathLength;
    [SerializeField]
    private List<Skill> skills;
    [SerializeField]
    private List<GameObject> weapons;
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

    //Shaders (used for the highlight)
    private Renderer rend;
    [SerializeField]
    private Shader originalShader;
    private Shader outlineShader;
    
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
    public override Player Player
    {
        get
        {
            return base.Player;
        }

        set
        {
            base.Player = value;
            if(GameManager.instance && GameManager.instance.player1!=null && GameManager.instance.player1==Player.gameObject)
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



    public List<LivingPlaceable> TargetableUnits
    {
        get
        {
            return targetableUnits;
        }

        set
        {
            foreach (LivingPlaceable living in targetableUnits)
            {
                living.UnHighlightForSkill();
            }
            foreach (LivingPlaceable living in value)
            {
                living.HighlightForSkill();
            }
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

    public string Classname
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

    public int Mstr
    {
        get
        {
            return mstr;
        }

        set
        {
            mstr = value;
        }
    }

    public int Mdef
    {
        get
        {
            return mdef;
        }

        set
        {
            mdef = value;
        }
    }

    public int Def
    {
        get
        {
            return def;
        }

        set
        {
            def = value;
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

    // Use this for initialization
    void Awake()
    {
        /*
        shouldBatch = false;
        this.className = "default";
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
        List<Effect> ListEffects3 = new List<Effect>();
        List<Effect> ListEffects4 = new List<Effect>();
        List<Effect> ListEffects5 = new List<Effect>();
        List<Effect> ListEffects6 = new List<Effect>();
        ListEffects.Add(new Push(null, this, 2, 500));
        ListEffects3.Add(new DestroyBloc());
        ListEffects2.Add(new CreateBlock(Grid.instance.prefabsList[0], new Vector3Int(0, 1, 0)));
        ListEffects4.Add(new Damage(50f));
        ListEffects5.Add(new DestroyBloc());
        ListEffects6.Add(new CreateBlock(Grid.instance.prefabsList[0], new Vector3Int(0, 1, 0)));
        Skill skill1 = new Skill(0, 1, ListEffects, SkillType.BLOCK, "push",0,4,SkillArea.CROSS);
        skill1.Save();
        skill1.effects[0].Save();
        Skill skill2 = new Skill(0, 1, ListEffects2, SkillType.BLOCK, "CreateBlock",0,5);
        Skill skill3 = new Skill(0, 1, ListEffects3, SkillType.BLOCK, "destroyBlock", 0, 3);
        Skill skill4 = new Skill(0, 1, ListEffects4, SkillType.LIVING, "damage", 0, 2);
        Skill skill5 = new Skill(0, 1, ListEffects2, SkillType.AREA, "spell2", 0, 5, SkillArea.NONE, 2);
        Skill skill6 = new Skill(0, 1, ListEffects3, SkillType.AREA, "destroyBlock", 0, 3, SkillArea.LINE, 1);
        Skills.Add(skill1);
        Skills.Add(skill2);
        Skills.Add(skill3);
        Skills.Add(skill4);
        Skills.Add(skill5);
        Skills.Add(skill6);
        this.characterSprite = Resources.Load<Sprite>("UI_Images/Characters/" + className);
        this.AreaOfMouvement = new List<NodePath>();
        targetArea = new List<Placeable>();

        targetableUnits = new List<LivingPlaceable>();
        //   this.OnWalkEffectsOnWalkEffects = new List<Effect>();
        this.OnDestroyEffects = new List<Effect>();
        this.HitablePoints = new List<HitablePoint>();
        this.OnStartTurn = new List<Effect>();
        this.OnEndTurn = new List<Effect>();
        this.AttachedEffects = new List<Effect>();
        */
        shouldBatch = false;
        this.Classname = "default";
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
        this.AreaOfMouvement = new List<NodePath>();
        targetArea = new List<Placeable>();

        targetableUnits = new List<LivingPlaceable>();
        //   this.OnWalkEffectsOnWalkEffects = new List<Effect>();
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
        rend.material.SetColor("_Color", new Color(1,1,1,0.725f));
        rend.material.SetFloat("_Outline", 0.03f);
        
    }

    public void InitCharacter(int classNumber)
    {
        className = GameManager.instance.PossibleCharacters[classNumber].className;
        Debug.Log(Classname + ".json");
        LoadFromjson(className + ".json");
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
            foreach (var effect in this.OnDestroyEffects)
            {
                EffectManager.instance.UseEffect(effect);
            }
            foreach (Transform obj in transform.Find("Inventory"))
            {
                obj.GetComponent<ObjectOnBloc>().Destroy();
            }
        }
        if (GameManager.instance.playingPlaceable == this)
        {
            GameManager.instance.EndOFTurn();
        }
        this.IsDead = true;
        this.gameObject.SetActive(false);
        Grid.instance.GridMatrix[GetPosition().x, GetPosition().y, GetPosition().z] = null;
        CounterDeaths++;
    }
    
    public void HighlightForSpawn()
    {
        rend.material.shader = outlineShader;
        rend.material.SetColor("_OutlineColor" ,Color.green);
    }

    public void UnHighlightForSpawn()
    {
        rend.material.shader = originalShader;
    }

    public void HighlightForSkill()
    {
        if (rend != null && rend.material != null)
        {
            rend.material.shader = outlineShader;
        }
    }

    public void UnHighlightForSkill()
    {
        if(rend !=null && rend.material!=null)
        { 
        rend.material.shader = originalShader;
        }
    }

    public void ChangeMaterialAreaOfMovementBatch(Material pathfinding)
    {
       
        foreach (NodePath node in AreaOfMouvement)
        {
            if(Grid.instance.GridMatrix[node.x, node.y, node.z] != null && Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial==null) //if we haven't seen this one before
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
                quadRight.transform.localScale = new Vector3(quadRight.transform.localScale.x, heightSize+0.01f, 1);
                quadRight.transform.localPosition = new Vector3(quadRight.transform.localPosition.x, 0.5f-heightSize/2+0.01f, quadRight.transform.localPosition.z);

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

            if (Grid.instance.GridMatrix[node.x, node.y, node.z]!=null && Grid.instance.GridMatrix[node.x, node.y, node.z].oldMaterial!=null)//if we haven't already reset this one
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
            if (Grid.instance.GridMatrix[placeable.GetPosition().x, placeable.GetPosition().y, placeable.GetPosition().z].oldMaterial == null) //if we haven't seen this one before
            {
                // Grid.instance.GridMatrix[node.x, node.y, node.z].GetComponent<MeshRenderer>().enabled = true;
                Grid.instance.GridMatrix[placeable.GetPosition().x, placeable.GetPosition().y, placeable.GetPosition().z].oldMaterial =
                    Grid.instance.GridMatrix[placeable.GetPosition().x, placeable.GetPosition().y, placeable.GetPosition().z].GetComponent<MeshRenderer>().material;
                Grid.instance.GridMatrix[placeable.GetPosition().x, placeable.GetPosition().y, placeable.GetPosition().z].GetComponent<MeshRenderer>().material = materialTarget;

            }
        }
        GameManager.instance.ResetAllBatches();

    }

    public void ResetAreaOfTarget()
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
        if(targetArea.Count>0)
        {
            GameManager.instance.RefreshBatch(targetArea[0]);
        }
        targetArea.Clear();
      
    }

    public void Save(string path = "Living.json")
    {
        positionSave = GetPosition();
        Stats stats = new Stats();
        stats.FillThis(this);
        string text = JsonUtility.ToJson(stats);
        foreach (Skill skill in Skills)
        {
            text+=skill.Save();
        }
        File.WriteAllText(path, text);
    }

    public static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
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
            return ;
        }
        Stats newLivingStats = JsonUtility.FromJson<Stats>(line);
        newLivingStats.FillLiving(this);
        this.characterSprite = Resources.Load<Sprite>("UI_Images/Characters/" + className);
        bool isNewSkill = true;
        Skill newSkill = null;
        
        while ((line = reader.ReadLine()) != null)
        {
            if(isNewSkill)
            {
                
                newSkill = JsonUtility.FromJson<Skill>(line);
                newSkill.AbilitySprite = Resources.Load<Sprite>("UI_Images/Abilities/" + newSkill.SkillName);
                newSkill.effects = new List<Effect>();
                isNewSkill = false;
                
            }
            else
            {
                string typename=line.Substring(0, line.IndexOf("{"));
                foreach(Type type in possible)
                {
                    if(type.ToString()==typename)
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
                        Debug.Log(a);
                        Effect eff=(Effect)JsonUtility.FromJson(a, type);
                        eff.Initialize();
                        newSkill.effects.Add(eff);
                        if(isNewSkill)
                        {
                            if(skills==null)
                            {
                                skills = new List<Skill>();
                            }
                            skills.Add(newSkill);
                        }
                      
                    }
                }
               

            }
          
        }
        reader.Close();

    }
    
    

}
