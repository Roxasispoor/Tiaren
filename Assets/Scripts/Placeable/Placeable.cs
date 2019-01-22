using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// Represents something able to fill a bloc of the grid
/// </summary>
[Serializable]
public abstract class Placeable: NetIdeable
{
    const float sizeChild = 1.02f;
    [NonSerialized]
    public Batch batch;
    public static int currentMaxId=0;
    [SerializeField]
    public int serializeNumber;
    private bool walkable;
    protected List<Effect> onWalkEffects;
    protected bool movable;
    protected bool destroyable;
    protected TraversableType tangible;
    protected TraversableType traversableBullet;
    public Color colorOfObject;
    private float animationSpeed=1.0f;
    [NonSerialized]
    public Material oldMaterial;
    public Material baseMaterial;
    protected GravityType gravityType;
    protected CrushType crushable;
    private bool explored;
    private bool grounded;
    protected List<Effect> onDestroyEffects;
    protected List<HitablePoint> hitablePoints;
    protected List<Effect> onStartTurn;
    protected List<Effect> onEndTurn;

    protected CombineInstance meshInCombined;

    // Bool to avoid doubleclick with the same input
    private static bool isClicked = false;

    /// <summary>
    /// player who owns the placeable. players, neutral monsters, and null (independant blocs)
    /// </summary>
    private Player player;
    [SerializeField]
    private bool isSpawnPoint;


   
    public virtual bool IsLiving()
    {
        return false;
    }
    public bool Walkable
    {
        get
        {
            return walkable;
        }

        set
        {
            walkable = value;
        }
    }

    public bool Movable
    {
        get
        {
            return movable;
        }

        set
        {
            movable = value;
        }
    }

   

    /// <summary>
    /// indicates if gravity tests have been already done on placeable 
    /// </summary>
    public bool Explored
    {
        get
        {
            return explored;
        }

        set
        {
            explored = value;
        }
    }

    public bool Grounded
    {
        get
        {
            return grounded;
        }

        set
        {
            grounded = value;
        }
    }


    public List<Effect> OnStartTurn
    {
        get
        {
            return onStartTurn;
        }

        set
        {
            onStartTurn = value;
        }
    }

    public List<Effect> OnEndTurn
    {
        get
        {
            return onEndTurn;
        }

        set
        {
            onEndTurn = value;
        }
    }

    public List<Effect> OnDestroyEffects
    {
        get
        {
            return onDestroyEffects;
        }

        set
        {
            onDestroyEffects = value;
        }
    }

    public List<Effect> OnWalkEffects
    {
        get
        {
            return onWalkEffects;
        }

        set
        {
            onWalkEffects = value;
        }
    }

    public List<HitablePoint> HitablePoints
    {
        get
        {
            return hitablePoints;
        }

        set
        {
            hitablePoints = value;
        }
    }
    
    public TraversableType TraversableChar
    {
        get
        {
            return tangible;
        }

        set
        {
            tangible = value;
        }
    }

    public TraversableType TraversableBullet
    {
        get
        {
            return traversableBullet;
        }

        set
        {
            traversableBullet = value;
        }
    }

    public GravityType GravityType
    {
        get
        {
            return gravityType;
        }

        set
        {
            gravityType = value;
        }
    }

    public CrushType Crushable
    {
        get
        {
            return crushable;
        }

        set
        {
            crushable = value;
        }
    }

    public virtual Player Player
    {
        get
        {
            return player;
        }

        set
        {
            player = value;
        }
    }

   

    public bool Destroyable
    {
        get
        {
            return destroyable;
        }

        set
        {
            destroyable = value;
        }
    }

    public float AnimationSpeed
    {
        get
        {
            return animationSpeed;
        }

        set
        {
            animationSpeed = value;
        }
    }


    public CombineInstance MeshInCombined
    {
        get
        {
            return meshInCombined;
        }

        set
        {
            meshInCombined = value;
        }
    }

    public bool IsSpawnPoint
    {
        get
        {
            return isSpawnPoint;
        }

        set
        {
            isSpawnPoint = value;
        }
    }




    /// <summary>
    /// Copy object
    /// </summary>
    /// <returns>Return a copy of the object</returns>
    public virtual Placeable Cloner()
    {
        var copy = (Placeable)this.MemberwiseClone();
        return copy;
    }

    /// <summary>
    /// method to call for destroying object
    /// </summary>
    public virtual void Destroy()
    {
        if (this.Destroyable)
        {
            foreach (var effect in this.OnDestroyEffects)
            {
                EffectManager.instance.UseEffect(effect);
            }
        }
        Destroy(this);
        Destroy(this.gameObject);
    }
    public void Highlight()
    {
        if (IsLiving()) return;
        if (GameManager.instance.activeSkill != null && GameManager.instance.activeSkill.SkillType == SkillType.BLOCK)
        {
            GameObject quadUp = transform.Find("QuadUp").gameObject;
            GameObject quadRight = transform.Find("QuadRight").gameObject;
            GameObject quadLeft = transform.Find("QuadLeft").gameObject;
            GameObject quadFront = transform.Find("QuadFront").gameObject;
            GameObject quadBack = transform.Find("QuadBack").gameObject;

            quadUp.SetActive(true);

            quadRight.SetActive(true);
            quadRight.transform.localScale = new Vector3(quadRight.transform.localScale.x, sizeChild, 1);
            quadRight.transform.localPosition = new Vector3(quadRight.transform.localPosition.x, 0, quadRight.transform.localPosition.z);

            quadLeft.SetActive(true);
            quadLeft.transform.localScale = new Vector3(quadLeft.transform.localScale.x, sizeChild, 1);
            quadLeft.transform.localPosition = new Vector3(quadLeft.transform.localPosition.x, 0, quadLeft.transform.localPosition.z);

            quadFront.SetActive(true);
            quadFront.transform.localScale = new Vector3(quadFront.transform.localScale.x, sizeChild, 1);
            quadFront.transform.localPosition = new Vector3(quadFront.transform.localPosition.x, 0, quadFront.transform.localPosition.z);

            quadBack.SetActive(true);
            quadBack.transform.localScale = new Vector3(quadBack.transform.localScale.x, sizeChild, 1);
            quadBack.transform.localPosition = new Vector3(quadBack.transform.localPosition.x, 0, quadBack.transform.localPosition.z);

        }
        foreach (Transform fils in transform)
        {

            fils.gameObject.SetActive(true);
            fils.gameObject.GetComponent<MeshRenderer>().material = GameManager.instance.highlightingMaterial;
        }
    }
    public void UnHighlight()
    {
        if (IsLiving()) return;

        //Put back the default material
        foreach (Transform fils in transform)
        {
            fils.gameObject.GetComponent<MeshRenderer>().material = GameManager.instance.pathFindingMaterial;
        }
        //If we are in move mode doesn't belong to path we desactivate it
        if (GameManager.instance.state!=States.Move  ||
            !GameManager.instance.playingPlaceable.AreaOfMouvement.Exists(new NodePath(GetPosition().x, GetPosition().y, GetPosition().z, 0, null).Equals))

        {


            foreach (Transform fils in transform)
            {

                fils.gameObject.SetActive(false);

            }
        }
       
    }

    public void OnMouseOverWithLayer()
    {
        if (GameManager.instance.state == States.Spawn)
        {
            if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonUp(0) && !isClicked)
            {
                isClicked = true;

                if (this.IsSpawnPoint == true)
                {
                    Debug.Log("You have authority to ask to spawn");
                    if (GameManager.instance.CharacterToSpawn == null)
                    {
                        LivingPlaceable charaClicked = (LivingPlaceable)Grid.instance.GridMatrix[this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z];

                        if (charaClicked.player == GameManager.instance.GetLocalPlayer())
                            GameManager.instance.CharacterToSpawn = charaClicked;
                    }
                    else
                    {
                        if (Grid.instance.GridMatrix[this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z] == null)
                        {
                            Grid.instance.GridMatrix[this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z] = GameManager.instance.CharacterToSpawn;
                            Grid.instance.GridMatrix[GameManager.instance.CharacterToSpawn.GetPosition().x, GameManager.instance.CharacterToSpawn.GetPosition().y,
                                GameManager.instance.CharacterToSpawn.GetPosition().z] = null;
                            GameManager.instance.CharacterToSpawn.transform.position = new Vector3(this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z);
                        }
                        else if (Grid.instance.GridMatrix[this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z].IsLiving() 
                            && Grid.instance.GridMatrix[this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z].player == GameManager.instance.GetLocalPlayer())
                        {
                            Placeable temp = Grid.instance.GridMatrix[this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z];
                            Grid.instance.GridMatrix[GameManager.instance.CharacterToSpawn.GetPosition().x, GameManager.instance.CharacterToSpawn.GetPosition().y,
                                GameManager.instance.CharacterToSpawn.GetPosition().z] = temp;
                            Grid.instance.GridMatrix[this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z] = GameManager.instance.CharacterToSpawn;
                            temp.transform.position = new Vector3(GameManager.instance.CharacterToSpawn.GetPosition().x, GameManager.instance.CharacterToSpawn.GetPosition().y,
                                GameManager.instance.CharacterToSpawn.GetPosition().z);
                            GameManager.instance.CharacterToSpawn.transform.position = new Vector3(this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z);
                        }
                        GameManager.instance.CharacterToSpawn = null;
                    }
                }
              
            } else
            {
                isClicked = false;
            }
        }
        else if (GameManager.instance.state == States.Move)
        {
            // Debug.Log(EventSystem.current.IsPointerOverGameObject());
            if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonUp(0) && this.walkable)
            {
                if (GameManager.instance.playingPlaceable.Player.isLocalPlayer && !GameManager.instance.playingPlaceable.Player.GetComponent<Player>().isWinner
                    && this.GetPosition() + new Vector3Int(0, 1, 0) != GameManager.instance.playingPlaceable.GetPosition())

                {
                    Debug.Log("You have authority to ask for a move");
                    //Vector3 destination = this.GetPosition();
                    Vector3[] path = GameManager.instance.GetPathFromClicked(this);//Check and move on server
                    GameManager.instance.playingPlaceable.Player.CmdMoveTo(path);
                    // GameManager.instance.CheckIfAccessible(this);
                }
            }
        }
        else if (GameManager.instance.state == States.UseSkill)
        {
            if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonUp(0))
            {
                if (GameManager.instance.playingPlaceable.Player.isLocalPlayer && !GameManager.instance.playingPlaceable.Player.GetComponent<Player>().isWinner
                    && (GameManager.instance.activeSkill.SkillType == SkillType.LIVING && IsLiving() || (GameManager.instance.activeSkill.SkillType == SkillType.BLOCK || GameManager.instance.activeSkill.SkillType == SkillType.AREA) && !IsLiving()))

                {
                    Debug.Log("You have authority to ask to act");
                    GameManager.instance.playingPlaceable.player.CmdUseSkill(GameManager.instance.playingPlaceable.Skills.FindIndex(GameManager.instance.activeSkill.Equals), netId);
                    //GameManager.instance.activeSkill.Use(GameManager.instance.playingPlaceable, new List<Placeable>(){this});
                }
            }
        }
    }
    /// <summary>
    /// allows shoot and shifting
    /// </summary>
    protected void OnMouseOver()
    {
       
    }

    protected void Awake()
    {

        baseMaterial = GetComponent<Renderer>().material;
    }

    /// <summary>
    /// On dispatch selon Living et placeable
    /// </summary>
    /// <param name="effect"></param>
    public override void DispatchEffect(Effect effect)
    {
        effect.TargetAndInvokeEffectManager(this);
    }
}
