using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents something able to fill a bloc of the grid
/// </summary>
[Serializable]
public abstract class Placeable : NetIdeable
{

    /// <summary>
    /// Can we walk on it.
    /// </summary>
    public bool walkable;

    /// <summary>
    /// Is the placeable movable (for push, etc.).
    /// </summary>
    public bool movable;

    /// <summary>
    /// Is the placeable traversable by another Placeable.
    /// </summary>
    public TraversableType traversableType;

    /// <summary>
    /// Is the placeable traversable by bullets.
    /// </summary>
    public TraversableType traversableBullet;

    /// <summary>
    /// The speed used for the animation.
    /// </summary>
    private float animationSpeed = 2.5f;

    /// <summary>
    /// Previous material (used for highlighting)
    /// </summary>
    // AMELIORATION: move to cube ?
    [NonSerialized]
    public Material oldMaterial;

    /// <summary>
    /// Base material.
    /// </summary>
    // AMELIORATION: move to cube ?
    public Material baseMaterial;

    /// <summary>
    /// How it react to gravity.
    /// </summary>
    // TODO: Changed it to protected -> Need to create a Spawn prefab
    public GravityType gravityType;

    /// <summary>
    /// How it react when crushed.
    /// </summary>
    protected CrushType crushable;

    /// <summary>
    /// Used for gravity - Indicates if gravity tests have been already done on placeable.
    /// </summary>
    public bool explored;

    /// <summary>
    /// Used for gravity - Indicates if it is connected to the ground.
    /// </summary>
    public bool grounded;

    /// <summary>
    /// Current coroutine used for the movement.
    /// </summary>
    public Coroutine moveCoroutine;

    /// <summary>
    /// player who owns the placeable. players, neutral monsters, and null (independant blocs)
    /// </summary>
    public Player player;

    /// <summary>
    /// How it react to gravity.
    /// </summary>
    public GravityType GravityType
    {
        get
        {
            return gravityType;
        }
    }

    /// <summary>
    /// How it react when crushed.
    /// </summary>
    public CrushType Crushable
    {
        get
        {
            return crushable;
        }
    }

    /// <summary>
    /// player who owns the placeable. players, neutral monsters, and null (independant blocs)
    /// </summary>
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

    /// <summary>
    /// The speed used for the animation.
    /// </summary>
    public float AnimationSpeed
    {
        get
        {
            return animationSpeed;
        }
    }

    /// <summary>
    /// Return true if this Placeable is Living.
    /// </summary>
    /// <returns></returns>
    public override bool IsLiving()
    {
        return false;
    }

    /// <summary>
    /// method to call for destroying object
    /// </summary>
    public abstract void Destroy();

    public abstract void Highlight();

    public abstract void UnHighlight();

    public void OnMouseOverWithLayer()
    {
        if (GameManager.instance.State == States.Spawn) // AMELIORATION: This part should be in LivingPlaceable and not here
        {
            if (GameManager.instance.GetLocalPlayer().Isready != true) {
                if (Input.GetMouseButtonUp(0))
                {

                    if (!this.IsLiving() && ((StandardCube)this).isSpawnPoint == true // AMELIORATION: Maybe could be remove
                                                                                      // Check if it is a spawn point for that player
                            && Grid.instance.GetSpawnPlayer(GameManager.instance.GetLocalPlayer()).Contains(GetPosition() + Vector3Int.up)
                            && GameManager.instance.CharacterToSpawn != null)
                    {
                        Debug.Log("You have authority to ask to spawn");

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
                    else if (player == GameManager.instance.GetLocalPlayer())
                    {
                        if (!GameManager.instance.CharacterToSpawn)
                        {
                            GameManager.instance.CharacterToSpawn = (LivingPlaceable)this;
                        }
                        else
                        {
                            /*
                            //Placeable temp = Grid.instance.GridMatrix[this.GetPosition().x, this.GetPosition().y - 1, this.GetPosition().z];
                            Grid.instance.GridMatrix[GameManager.instance.CharacterToSpawn.GetPosition().x, GameManager.instance.CharacterToSpawn.GetPosition().y,
                                GameManager.instance.CharacterToSpawn.GetPosition().z] = temp;
                            Grid.instance.GridMatrix[this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z] = GameManager.instance.CharacterToSpawn;
                            temp.transform.position = new Vector3(GameManager.instance.CharacterToSpawn.GetPosition().x, GameManager.instance.CharacterToSpawn.GetPosition().y,
                                GameManager.instance.CharacterToSpawn.GetPosition().z);
                            GameManager.instance.CharacterToSpawn.transform.position = new Vector3(this.GetPosition().x, this.GetPosition().y + 1, this.GetPosition().z);
                            */

                            Vector3Int temp = GetPosition();
                            Grid.instance.GridMatrix[GameManager.instance.CharacterToSpawn.GetPosition().x, GameManager.instance.CharacterToSpawn.GetPosition().y,
                                GameManager.instance.CharacterToSpawn.GetPosition().z] = this;
                            this.transform.position = GameManager.instance.CharacterToSpawn.transform.position;
                            Grid.instance.GridMatrix[temp.x, temp.y, temp.z] = GameManager.instance.CharacterToSpawn;
                            GameManager.instance.CharacterToSpawn.transform.position = temp;


                            GameManager.instance.CharacterToSpawn = null;
                        }
                    }
                }
            }
        }
        else if (GameManager.instance.State == States.Move)
        {
            // Debug.Log(EventSystem.current.IsPointerOverGameObject());
            if (Input.GetMouseButtonUp(0) && this.walkable)
            {
                if (GameManager.instance.playingPlaceable.Player.isLocalPlayer && !GameManager.instance.playingPlaceable.Player.GetComponent<Player>().isWinner)
                {
                    Debug.Log("You have authority to ask for a deplacment move");
                    //Vector3 destination = this.GetPosition();
                    Vector3[] path = GameManager.instance.GetPathFromClicked(this);//Check and move on server
                    GameManager.instance.playingPlaceable.Player.CmdMoveTo(path);
                    // GameManager.instance.CheckIfAccessible(this);
                }
            }
        }
        else if (GameManager.instance.State == States.UseSkill)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Skill skill = GameManager.instance.activeSkill;
                if (GameManager.instance.playingPlaceable.Player.isLocalPlayer && !GameManager.instance.playingPlaceable.Player.GetComponent<Player>().isWinner
                    && skill != null && (GameManager.instance.PlayingPlaceable.TargetArea.Contains(this) ||
                    IsLiving() && GameManager.instance.PlayingPlaceable.TargetableUnits.Contains((LivingPlaceable)this))

                    /*&& (skill.SkillType == SkillType.LIVING && IsLiving() 
                    || (skill.SkillType == SkillType.BLOCK || skill.SkillType == SkillType.AREA) && !IsLiving())*/)


                {

                    Debug.Log("You have authority to ask to act on " + netId + " On position" + GetPosition() + "Time : " + Time.time);
                    List<Placeable> area = GameManager.instance.playingPlaceable.player.GetComponentInChildren<RaycastSelector>().Area;
                    if (area == null && skill.SkillArea != SkillArea.SURROUNDINGLIVING)
                    {
                        GameManager.instance.playingPlaceable.player.OnUseSkill(Player.SkillToNumber(GameManager.instance.playingPlaceable, skill), netId, new int[0], 0);
                    }
                    else if (skill.SkillArea == SkillArea.SURROUNDINGLIVING || skill.SkillArea == SkillArea.MIXEDAREA)
                    {
                        List<LivingPlaceable> Playerlist = GameManager.instance.playingPlaceable.TargetableUnits;
                        int j = 0;
                        int[] netidlist;

                        if (skill.SkillArea == SkillArea.MIXEDAREA)
                        {
                            netidlist = new int[Playerlist.Count + area.Count];
                            for (j = 0; j < area.Count; j++)
                            {
                                netidlist[j] = area[j].netId;
                            }
                        }
                        else
                        {
                            netidlist = new int[Playerlist.Count];
                        }

                        for (int i = j; i < netidlist.Length; i++)
                        {
                            netidlist[i] = Playerlist[i - j].netId;
                        }

                        GameManager.instance.playingPlaceable.player.OnUseSkill(Player.SkillToNumber(GameManager.instance.playingPlaceable, skill), netId, netidlist,
                            GameManager.instance.playingPlaceable.player.GetComponentInChildren<RaycastSelector>().State);
                    }
                    else
                    {
                        int[] netidlist = new int[area.Count];
                        for (int i = 0; i < netidlist.Length; i++)
                        {
                            netidlist[i] = area[i].netId;
                        }
                        GameManager.instance.playingPlaceable.player.OnUseSkill(Player.SkillToNumber(GameManager.instance.playingPlaceable, skill), netId, netidlist,
                            GameManager.instance.playingPlaceable.player.GetComponentInChildren<RaycastSelector>().State);
                    }
                    //GameManager.instance.activeSkill.Use(GameManager.instance.playingPlaceable, new List<Placeable>(){this});
                }
            }

        }
    }

    protected virtual void Awake()
    {

        baseMaterial = GetComponent<Renderer>().material;
    }

    /// <summary>
    /// To call when creating a Placeable to initialize needed values
    /// </summary>
    public virtual void Init()
    {

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
